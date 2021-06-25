using System.Collections.Generic;
using Interpreter.Inner.CodeGenerator;

namespace Interpreter
{
    namespace Inner
    {
        class Parser
        {
            public Parser(GcMachine _m, Lexer _l)
            {
                m = _m; l = _l;
            }

            public void parse()
            {
                int pos = 1; int size;
                accept();
                do
                {
                    if (lookAhead.TokenType == TokenType.TOKEN_EOF) break;

                    int line = l.currentLine;
                    string funcName;

                    Statement body = functionDef();
                    Statement func = new stat_function(m, line, body, m.parser_getLocalCount());

                    funcName = m.parser_getCurrentFunction();

                    size = func.getInstruction(m.code, pos);
                    m.parser_register_function(l.currentLine, funcName);

                    if (funcName.Contains(":"))
                    {
                        var tableFunc = funcName.Split(':');
                        if (!m.IsGlobalExists(tableFunc[0]))
                            m.SetGlobal(tableFunc[0], new Varible(new Dictionary<Varible, Varible>(VarCompare.GetInstance())));
                        var table = m.GetGlobal(tableFunc[0]);
                        if (table.type != VarType.TYPE_TABLE)
                            throw new CompileException(line, "Not a table"); //Should never reach here
                        table.table[new Varible(tableFunc[1])] = new Varible(pos);
                    }
                    else
                        m.SetGlobal(funcName, new Varible(pos));

                    pos += size;

                } while (!l.eof());
                m.CodeLength = pos;
            }

            private void paramList()
            {
                char eod = ')';
                while (lookAhead.TokenType != eod)
                {
                    Token id = match(TokenType.TOKEN_ID);
                    m.parser_addLocal(id.strToken);
                    if (lookAhead.TokenType != eod)
                        match(',');
                }
            }

            private Statement localList()
            {
                List<Statement> localAssignments = new List<Statement>();
                char eod = ';'; int line;
                int localLine = l.currentLine;
                while (lookAhead.TokenType != eod)
                {
                    line = l.currentLine;
                    Token id = match(TokenType.TOKEN_ID);
                    m.parser_addLocal(id.strToken);

                    if (lookAhead.TokenType == '=')
                    {
                        accept(); 
                        localAssignments.Add( 
                            new stat_exp(m,line, new exp_assignment(
                                m,line,new exp_local_var(m,line,m.GetLocalIDByName(id.strToken)),expr() )
                                )
                            );
                    }
                    if (lookAhead.TokenType != eod)
                    {
                        match(',');
                    }
                }
                return new stat_seq(m, localLine, localAssignments);
            }

            private List<Expression> args(bool includeThis,Expression thisExpr)
            {
                List<Expression> expList = new List<Expression>();
                if (includeThis)
                    expList.Add(thisExpr);
                while (lookAhead.TokenType != ')')
                {
                    expList.Add(expr());
                    if (lookAhead.TokenType != ')')
                        match(',');
                }
                return expList;
            }

            private Expression leftVal(out Expression thisVal,out bool includeThis)
            {
                int line = l.currentLine;
                Token id; int local;
                Expression retVal;
                Expression index;
                includeThis = false;
                thisVal = null;

                id = match(TokenType.TOKEN_ID);
                local = m.GetLocalIDByName(id.strToken);

                if (local != -1)
                    retVal = new exp_local_var(m, l.currentLine, local);
                else
                    retVal = new exp_var(m, l.currentLine, m.parser_checkStringMap(id.strToken, true));

                while (lookAhead.TokenType == '[' || lookAhead.TokenType == '.')
                {
                    if (lookAhead.TokenType == '[')
                    {
                        accept(); index = expr(); match(']');
                        retVal = new exp_table_item(m, line, retVal, index);
                    }
                    if (lookAhead.TokenType == '.')
                    {
                        accept(); id = match(TokenType.TOKEN_ID);
                        index = new exp_string(m, line, m.parser_checkStringMap(id.strToken, true));
                        retVal = new exp_table_item(m, line, retVal, index);
                    }
                }

                if (lookAhead.TokenType == ':')
                {
                    includeThis = true;
                    thisVal = retVal;
                    accept(); id = match(TokenType.TOKEN_ID);
                    index = new exp_string(m, line, m.parser_checkStringMap(id.strToken, true));
                    retVal = new exp_table_item(m, line, retVal, index);
                }

                return retVal;
            }

            private Expression newTable()
            {
                Token id;
                int line;
                exp_new_table table = new exp_new_table(m, l.currentLine);
                while (lookAhead.TokenType == '.' || lookAhead.TokenType == '[')
                {
                    if (lookAhead.TokenType == '.')
                    {
                        accept();
                        id = match(TokenType.TOKEN_ID); 
                        line = l.currentLine; match('=');
                        table.AddInitStmt(new stat_exp(m, line,
                            new exp_assignment(m, line, new exp_table_item_init
                            (m, line, new exp_string(m, line, m.parser_checkStringMap(id.strToken, true))),
                            expr()
                            )));
                    }
                    else //lookAhead.TokenType == '['
                    {
                        accept();
                        Expression index = expr();
                        match(']');
                        line = l.currentLine; match('=');
                        table.AddInitStmt(new stat_exp(m, line,
                            new exp_assignment(m, line, new exp_table_item_init(
                                m, line, index),
                                expr()
                                )));
                    }
                    if (lookAhead.TokenType != ']')
                        match(',');
                }
                return table;
            }

            private Expression factor()
            {
                Expression retVal;
                Expression thisVal;
                Token id;
                bool includeThis;
                switch (lookAhead.TokenType)
                {
                    case '(':
                        accept(); retVal = expr(); match(')');
                        break;
                    case TokenType.TOKEN_ID:
                        int line = l.currentLine;
                        Expression lVal = leftVal(out thisVal,out includeThis);
                        if (lookAhead.TokenType == '(' || includeThis) // function call
                        {
                            match('('); List<Expression> argss = args(includeThis,thisVal); match(')');
                            retVal = new exp_functioncall(m, line, argss, lVal);
                        }
                        else
                        {
                            retVal = lVal;
                        }
                        break;
                    case TokenType.TOKEN_NUMBER:
                        id = match(TokenType.TOKEN_NUMBER);
                        return new exp_number(m, l.currentLine, id.numToken);
                    case '[':
                        accept();
                        retVal = newTable();
                        match(']');
                        break;
                        //return new exp_new_table(m, l.currentLine);
                    case '!':
                        accept();
                        return new exp_not(m, l.currentLine, factor());
                    case '-':
                        accept();
                        return new exp_arith(m, l.currentLine, new exp_number(m, l.currentLine, 0.0), factor(), OpCode.ARITH_SUB);
                    case TokenType.TOKEN_STRING:
                        id = match(TokenType.TOKEN_STRING);
                        return new exp_string(m, l.currentLine, m.parser_checkStringMap(id.strToken, true));
                    default:
                        throw new CompileException(l.currentLine, "Unexpected Token " + TokenType.getTokenTypeString(lookAhead.TokenType));
                }
                return retVal;
            }

            private Expression expr1(int priority)
            {
                Expression retVal;
                Expression right;
                int compareMode;
                int line = l.currentLine;
                if (priority >= 0)
                    retVal = expr1(priority - 1);
                else
                    return factor();
                while (inPriority(lookAhead.TokenType, priority))
                {
                    switch (lookAhead.TokenType)
                    {
                        case '=':
                            accept();
                            right = expr1(priority); // Right associtive
                            retVal = new exp_assignment(m, line, retVal, right);
                            break;
                        case '+':
                            accept();
                            right = expr1(priority - 1);
                            retVal = new exp_arith(m, line, retVal, right, OpCode.ARITH_ADD);
                            break;
                        case '-':
                            accept();
                            right = expr1(priority - 1);
                            retVal = new exp_arith(m, line, retVal, right, OpCode.ARITH_SUB);
                            break;
                        case '*':
                            accept();
                            right = expr1(priority - 1);
                            retVal = new exp_arith(m, line, retVal, right, OpCode.ARITH_MUL);
                            break;
                        case '/':
                            accept();
                            right = expr1(priority - 1);
                            retVal = new exp_arith(m, line, retVal, right, OpCode.ARITH_DIV);
                            break;
                        case '<':
                        case '>':
                        case TokenType.TOKEN_GEQ:
                        case TokenType.TOKEN_EQUAL:
                        case TokenType.TOKEN_LEQ:
                        case TokenType.TOKEN_NEQU:
                            compareMode = getCompareMode();
                            accept();
                            right = expr1(priority - 1);
                            retVal = new exp_compare(m, line, retVal, right, compareMode);
                            break;
                        case TokenType.TOKEN_LOGICAL_AND:
                            accept();
                            right = expr1(priority - 1);
                            retVal = new exp_and(m, line, retVal, right);
                            break;
                        case TokenType.TOKEN_LOGICAL_OR:
                            accept();
                            right = expr1(priority - 1);
                            retVal = new exp_or(m, line, retVal, right);
                            break;
                        default:
                            return retVal;
                    }

                }
                return retVal;
            }

            private Expression expr()
            {
                return expr1(5);
            }

            private Statement stmts()
            {
                List<Statement> statList = new List<Statement>();
                int line = l.currentLine;
                while (lookAhead.TokenType != '}')
                {
                    statList.Add(stmt());
                }
                statList.Add(new stat_nop(m, l.currentLine)); //for symbol table
                return new stat_seq(m, line, statList);
            }

            private Statement stmt()
            {
                Statement retVal;
                Statement do_if_true;
                Statement do_if_false;
                Expression cmp;
                Statement forExpr1; Expression forExpr2; Statement forExpr3;
                int line = l.currentLine;
                switch (lookAhead.TokenType)
                {
                    case '{':
                        match('{');
                        retVal = stmts();
                        match('}');
                        break;
                    case TokenType.TOKEN_IF:
                        accept(); match('(');
                        cmp = expr();
                        match(')');
                        do_if_true = stmt();
                        if (lookAhead.TokenType == TokenType.TOKEN_ELSE)
                        {
                            accept();
                            do_if_false = stmt();
                        }
                        else
                            do_if_false = new stat_nop(m, line);
                        retVal = new stat_if(m, line, cmp, do_if_true, do_if_false);
                        break;
                    case TokenType.TOKEN_WHILE:
                        accept(); match('(');
                        cmp = expr();
                        match(')');
                        retVal = new stat_while(m, line, cmp, stmt());
                        break;
                    case TokenType.TOKEN_RETURN:
                        accept();
                        retVal = new stat_return(m, line, expr());
                        match(';');
                        break;
                    case TokenType.TOKEN_LOCAL:
                        accept();
                        retVal = localList();
                        match(';');
                        break;
                    case TokenType.TOKEN_BREAK:
                        accept();
                        retVal = new stat_break(m, line);
                        match(';');
                        break;
                    case TokenType.TOKEN_CONTINUE:
                        accept();
                        retVal = new stat_continue(m, line);
                        match(';');
                        break;
                    case TokenType.TOKEN_FOR:
                        accept(); match('(');
                        if (lookAhead.TokenType == ';')
                            forExpr1 = new stat_nop(m, l.currentLine);
                        else
                            forExpr1 = new stat_exp(m, l.currentLine, expr());
                        match(';');

                        if (lookAhead.TokenType == ';')
                            forExpr2 = new exp_number(m, l.currentLine, 1.0);
                        else
                            forExpr2 = expr();
                        match(';');

                        if (lookAhead.TokenType == ')')
                            forExpr3 = new stat_nop(m, l.currentLine);
                        else
                            forExpr3 = new stat_exp(m, l.currentLine, expr());
                        match(')');
                        
                        retVal = new stat_for(m, line, forExpr1, forExpr2, forExpr3, stmt());
                        break;
                    default:
                        retVal = new stat_exp(m, line, expr());
                        match(';');
                        break;
                }
                return retVal;
            }

            private Statement functionDef()
            {
                match(TokenType.TOKEN_FUNCTION);
                Token id = match(TokenType.TOKEN_ID);
                Token funcToken;
                if (lookAhead.TokenType == ':')
                {
                    accept();
                    funcToken = match(TokenType.TOKEN_ID);
                    m.parser_setCurrentFunction(id.strToken + ":" + funcToken.strToken);
                    match('('); m.parser_addLocal("this"); paramList(); match(')');
                }
                else
                {
                    m.parser_setCurrentFunction(id.strToken);
                    match('('); paramList(); match(')');
                }
                
                return stmt();
            }

            private void accept()
            {
                lookAhead = l.next();
            }

            private Token match(int Token_type)
            {
                Token retVal = lookAhead;
                if (lookAhead.TokenType == Token_type)
                    accept();
                else
                    throw new CompileException(l.currentLine, "Except " + TokenType.getTokenTypeString(Token_type)
                        + " near " + TokenType.getTokenTypeString(lookAhead.TokenType));
                return retVal;
            }

            private int getCompareMode()
            {
                switch (lookAhead.TokenType)
                {
                    case '<':
                        return CompareMode.COMPARE_LESS;
                    case '>':
                        return CompareMode.COMPARE_GREATER;
                    case TokenType.TOKEN_GEQ:
                        return CompareMode.COMPARE_GREATER | CompareMode.COMPARE_EQUAL;
                    case TokenType.TOKEN_EQUAL:
                        return CompareMode.COMPARE_EQUAL;
                    case TokenType.TOKEN_LEQ:
                        return CompareMode.COMPARE_LESS | CompareMode.COMPARE_EQUAL;
                    case TokenType.TOKEN_NEQU:
                        return CompareMode.COMPARE_GREATER | CompareMode.COMPARE_LESS;
                };
                return 0;
            }

            private bool inPriority(int TokenType, int priority)
            {
                int i = 0;
                while (priorities[priority, i] != -1)
                {
                    if (priorities[priority, i] == TokenType) return true;
                    ++i;
                }
                return false;
            }

            private static int[,] priorities = 
            {
	            {'*','/',-1,-1,-1,-1,-1},
	            {'+','-',-1,-1,-1,-1,-1},
	            {'>','<',TokenType.TOKEN_EQUAL,TokenType.TOKEN_GEQ,TokenType.TOKEN_LEQ,TokenType.TOKEN_NEQU,-1},
	            {TokenType.TOKEN_LOGICAL_AND,-1,-1,-1,-1,-1,-1 },
	            {TokenType.TOKEN_LOGICAL_OR,-1,-1,-1,-1,-1,-1},
                {'=',-1,-1,-1,-1,-1,-1},
            };

            private GcMachine m;
            private Lexer l;
            private Token lookAhead;
        }
    }
}
