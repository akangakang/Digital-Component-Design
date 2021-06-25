using System.Collections.Generic;

namespace Interpreter
{
    namespace Inner
    {
        namespace CodeGenerator
        {
            abstract class ParseTreeNode
            {
                public abstract int getInstruction(Instruction[] buffer, int startPos);
                public ParseTreeNode(GcMachine m, int line)
                {
                    _m = m;
                    _line = line;
                }

                protected void ThrowException(string description)
                {
                    throw new CompileException(_line, description);
                }

                protected void SymbolSetCodeStartPos(int startPos)
                {
                    _m.parser_setLineCode(_line, startPos);
                }

                protected GcMachine _m;
                protected int _line;
            }

            abstract class Statement
                :ParseTreeNode
            {
                public Statement(GcMachine m, int line)
                    :base (m,line)
                {
                }
            }

            abstract class Expression
                : ParseTreeNode
            {
                public virtual void left()
                {
                    ThrowException("Cannot be a left value.");
                }

                public Expression(GcMachine m, int line)
                    : base(m,line)
                {
                }
            }

            #region Expressions
            class exp_compare
                : Expression
            {
                private Expression _left;
                private Expression _right;
                private int _compareMode;

                public exp_compare(GcMachine m, int line, Expression left, Expression right, int compareMode)
                    : base(m,line)
                {
                    _left = left;
                    _right = right;
                    _compareMode = compareMode;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    pos += _left.getInstruction(buffer, startPos + pos);
                    pos += _right.getInstruction(buffer, startPos + pos);
                    ins.opCode = OpCode.CMP;
                    ins.pointer = _compareMode;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_not
                : Expression
            {
                private Expression _exp;

                public exp_not(GcMachine m, int line, Expression exp)
                    : base(m, line)
                {
                    _exp = exp;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    pos += _exp.getInstruction(buffer, startPos + pos);
                    ins.opCode = OpCode.NOT;
                    ins.pointer = 0;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_and
                : Expression
            {
                private Expression _left;
                private Expression _right;

                public exp_and(GcMachine m, int line, Expression left, Expression right)
                    : base(m, line)
                {
                    _left = left;
                    _right = right;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    pos += _left.getInstruction(buffer, startPos + pos);

                    ins.opCode = OpCode.JUMP;
                    ins.pointer = 3;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;

                    ins.opCode = OpCode.PUSHNUM;
                    buffer[startPos + pos] = ins;
                    pos++;

                    ins.opCode = OpCode.JUMPA;
                    ins.pointer = _right.getInstruction(buffer, startPos + pos + 1) + 1;
                    buffer[startPos + pos] = ins;
                    pos++;

                    pos += (ins.pointer - 1);

                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_or
                : Expression
            {
                private Expression _left;
                private Expression _right;

                public exp_or(GcMachine m, int line, Expression left, Expression right)
                    : base(m, line)
                {
                    _left = left;
                    _right = right;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Expression exp = new exp_not(_m, _line,
                        new exp_and(_m, _line, new exp_not(_m, _line, _left), new exp_not(_m, _line, _right))
                    );
                    int pos = exp.getInstruction(buffer, startPos);
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_number
                : Expression
            {
                private double _num;

                public exp_number(GcMachine m, int line, double num)
                    : base(m, line)
                {
                    _num = num;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    ins.opCode = OpCode.PUSHNUM;
                    ins.pointer = 0;
                    ins.num = _num;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_string
                : Expression
            {
                private int _str;

                public exp_string(GcMachine m, int line, int str)
                    : base(m, line)
                {
                    _str = str;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    ins.opCode = OpCode.PUSHSTRING;
                    ins.pointer = _str;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_var
                : Expression
            {
                private int _var;
                private bool _left;

                public exp_var(GcMachine m, int line, int var)
                    : base(m, line)
                {
                    _var = var;
                    _left = false;
                }

                public override void left()
                {
                    _left = true;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    if (_left)
                        ins.opCode = OpCode.SETGLOBAL;
                    else
                        ins.opCode = OpCode.GETGLOBAL;
                    ins.pointer = _var;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_local_var
                : Expression
            {
                private int _var;
                private bool _left;

                public exp_local_var(GcMachine m, int line, int var)
                    : base(m, line)
                {
                    _var = var;
                    _left = false;
                }

                public override void left()
                {
                    _left = true;
                }


                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    if (_left)
                        ins.opCode = OpCode.SETLOCAL;
                    else
                        ins.opCode = OpCode.GETLOCAL;
                    ins.pointer = _var;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_new_table
                : Expression
            {
                public exp_new_table(GcMachine m, int line)
                    : base(m, line)
                {
                    tableInitList = null;
                }

                public void AddInitStmt(Statement stmt)
                {
                    if (tableInitList == null)
                        tableInitList = new List<Statement>();
                    tableInitList.Add(stmt);
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    ins.opCode = OpCode.TABLEOP;
                    ins.pointer = OpCode.TABLE_NEW;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;
                    if (tableInitList != null)
                    {
                        foreach (var stmt in tableInitList)
                            pos += stmt.getInstruction(buffer, startPos + pos);
                    }
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }

                private List<Statement> tableInitList;
            }

            class exp_table_item
                : Expression
            {
                private Expression _table;
                private Expression _index;
                private bool _left;

                public exp_table_item(GcMachine m, int line, Expression table, Expression index)
                    : base(m, line)
                {
                    _table = table;
                    _index = index;
                    _left = false;
                }

                public override void left()
                {
                    _left = true;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    pos += _table.getInstruction(buffer, startPos + pos);
                    pos += _index.getInstruction(buffer, startPos + pos);

                    ins.opCode = OpCode.TABLEOP;
                    if (_left)
                        ins.pointer = OpCode.TABLE_SETITEM;
                    else
                        ins.pointer = OpCode.TABLE_GETITEM;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_table_item_init
                : Expression
            {
                private Expression _index;
                private bool _left;

                public exp_table_item_init(GcMachine m, int line, Expression index)
                    : base(m, line)
                {
                    _index = index;
                    _left = false;
                }

                public override void left()
                {
                    _left = true;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    pos += _index.getInstruction(buffer, startPos + pos);

                    ins.opCode = OpCode.TABLEOP;
                    if (!_left)
                        ThrowException("Unexpected Table Initialize"); //Should never reach here
                    ins.pointer = OpCode.TABLE_SETITEM_INIT;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_arith
                : Expression
            {
                private Expression _left;
                private Expression _right;
                private int _arithType;

                public exp_arith(GcMachine m, int line, Expression left, Expression right, int arithType)
                    : base(m, line)
                {
                    _left = left;
                    _right = right;
                    _arithType = arithType;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    pos += _left.getInstruction(buffer, startPos + pos);
                    pos += _right.getInstruction(buffer, startPos + pos);

                    ins.opCode = OpCode.ARITH;
                    ins.pointer = _arithType;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;

                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_functioncall
                : Expression
            {
                private List<Expression> _exprList;
                private Expression _func;
                /*private int _funcName;
                private bool _local;*/

                public exp_functioncall(GcMachine m, int line, List<Expression> exprList, Expression func)
                    : base(m, line)
                {
                    _exprList = exprList;
                    _func = func;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;

                    for (int i = 0; i < _exprList.Count; ++i)
                    {
                        pos += _exprList[i].getInstruction(buffer, startPos + pos);
                    }


                    pos += _func.getInstruction(buffer, startPos + pos);

                    ins.opCode = OpCode.CALL;
                    ins.pointer = _exprList.Count;
                    ins.num = 0;
                    buffer[startPos + pos] = ins;
                    pos++;

                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            #endregion

            #region Statements

            class stat_nop
                : Statement
            {
                public stat_nop(GcMachine m, int line)
                    : base(m, line)
                {
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    SymbolSetCodeStartPos(startPos);
                    return 0;
                }
            }

            class stat_function
                : Statement
            {
                private Statement _body;
                private int _numparam;

                public stat_function(GcMachine m, int line, Statement body, int numparam)
                    : base(m, line)
                {
                    _body = body;
                    _numparam = numparam;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    int pos = 0; Instruction ins;
                    ins.pointer = _numparam; ins.num = 0;
                    ins.opCode = OpCode.ADJLOCAL;
                    buffer[startPos + pos] = ins;
                    pos++;

                    pos += _body.getInstruction(buffer, startPos + pos);
                    
                    ins.pointer = 0; ins.num = 0;
                    ins.opCode = OpCode.PUSHNUM;
                    buffer[startPos + pos] = ins;
                    pos++;
                    
                    ins.pointer = 0; ins.num = 0;
                    ins.opCode = OpCode.RET;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }

            }

            class stat_exp
                : Statement
            {
                private Expression _exp;

                public stat_exp(GcMachine m, int line, Expression exp)
                    : base(m, line)
                {
                    _exp = exp;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    int pos = 0; Instruction ins;
                    pos += _exp.getInstruction(buffer, startPos + pos);

                    ins.pointer = 1; ins.num = 0;
                    ins.opCode = OpCode.POP;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class stat_seq
                : Statement
            {
                private List<Statement> _statList;

                public stat_seq(GcMachine m, int line, List<Statement> statList)
                    : base(m, line)
                {
                    _statList = statList;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    int pos = 0;
                    for (int i = 0; i < _statList.Count; ++i)
                    {
                        pos += _statList[i].getInstruction(buffer, startPos + pos);
                    }

                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class stat_return
                : Statement
            {
                private Expression _exp;

                public stat_return(GcMachine m, int line, Expression exp)
                    : base(m, line)
                {
                    _exp = exp;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    int pos = 0; Instruction ins;
                    pos += _exp.getInstruction(buffer, startPos + pos);

                    ins.pointer = 0; ins.num = 0;
                    ins.opCode = OpCode.RET;
                    buffer[startPos + pos] = ins;
                    pos++;
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class exp_assignment
                : Expression
            {
                private Expression _exp;
                private Expression _left;

                public exp_assignment(GcMachine m, int line, Expression left, Expression exp)
                    : base(m, line)
                {
                    _left = left;
                    _exp = exp;
                    _left.left();
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    int pos = 0;
                    pos += _exp.getInstruction(buffer, startPos + pos);
                    pos += _left.getInstruction(buffer, startPos + pos);
                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class stat_if
                : Statement
            {
                private Expression _exp;
                private Statement _doIfTrue;
                private Statement _doIfFalse;

                public stat_if(GcMachine m, int line, Expression exp, Statement doIfTrue, Statement doIfFalse)
                    : base(m, line)
                {
                    _exp = exp;
                    _doIfFalse = doIfFalse;
                    _doIfTrue = doIfTrue;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    Instruction ins; int pos = 0;
                    pos += _exp.getInstruction(buffer, startPos + pos);

                    int action_if_true_pos = pos + 1 + _doIfFalse.getInstruction(buffer, startPos + (pos + 1));
                    ins.opCode = OpCode.JUMP;
                    ins.num = 0;
                    ins.pointer = action_if_true_pos - pos + 1;
                    buffer[startPos + pos] = ins;
                    pos++;

                    pos = action_if_true_pos;
                    int end_pos = pos + 1 + _doIfTrue.getInstruction(buffer, startPos + (pos + 1));
                    ins.opCode = OpCode.JUMPA;
                    ins.pointer = end_pos - pos;
                    buffer[startPos + pos] = ins;
                    pos++;

                    pos = end_pos;

                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class stat_break
                : Statement
            {

                public stat_break(GcMachine m, int line)
                    : base(m, line)
                {
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    int pos = 0; Instruction ins;

                    ins.pointer = 0; ins.num = 0;
                    ins.opCode = OpCode.PSEUDO_BREAK;
                    buffer[startPos + pos] = ins;
                    pos++;

                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class stat_continue
                : Statement
            {
                public stat_continue(GcMachine m, int line)
                    : base(m, line)
                {
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    int pos = 0; Instruction ins;

                    ins.pointer = 0; ins.num = 0;
                    ins.opCode = OpCode.PSEUDO_CONTINUE;
                    buffer[startPos + pos] = ins;
                    pos++;

                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            class stat_while
                : Statement
            {
                private Expression _exp;
                private Statement _body;

                public stat_while(GcMachine m, int line, Expression exp, Statement body)
                    : base(m, line)
                {
                    _exp = new exp_not(m, line, exp);
                    _body = body;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    int pos = 0; Instruction ins;
                    ins.pointer = 0; ins.num = 0;

                    pos += _exp.getInstruction(buffer, startPos + pos);
                    int body_length = _body.getInstruction(buffer, startPos + (pos + 1));
                    //jump to end if false
                    ins.opCode = OpCode.JUMP;
                    ins.pointer = body_length + 2;
                    buffer[startPos + pos] = ins;
                    pos++;
                    pos += body_length;

                    ins.opCode = OpCode.JUMPA;
                    ins.pointer = -pos;
                    buffer[startPos + pos] = ins;
                    pos++;

                    //Scan any break and continue instructions
                    for (int i = 0; i < pos; ++i)
                    {
                        if (buffer[startPos + i].opCode == OpCode.PSEUDO_BREAK)
                        {
                            buffer[startPos + i].opCode = OpCode.JUMPA;
                            buffer[startPos + i].pointer = pos - i;
                        }
                        else if (buffer[startPos + i].opCode == OpCode.PSEUDO_CONTINUE)
                        {
                            buffer[startPos + i].opCode = OpCode.JUMPA;
                            buffer[startPos + i].pointer = -i;
                        }
                    }

                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }


            class stat_for
                : Statement
            {
                private Statement _init;
                private Expression _exp;
                private Statement _step;
                private Statement _body;

                public stat_for(GcMachine m, int line,Statement init, Expression exp, Statement step,Statement body)
                    : base(m, line)
                {
                    _init = init;
                    _exp = new exp_not(m, line, exp);
                    _step = step;
                    _body = body;
                }

                public override int getInstruction(Instruction[] buffer, int startPos)
                {
                    int pos = 0; Instruction ins;
                    ins.pointer = 0; ins.num = 0;

                    int init_size = _init.getInstruction(buffer, startPos + pos);
                    pos += init_size;

                    pos += _exp.getInstruction(buffer, startPos + pos);
                    int body_length = _body.getInstruction(buffer, startPos + (pos + 1));
                    int step_length = _step.getInstruction(buffer, startPos + (pos + 1) + body_length);
                    //jump to end if false
                    ins.opCode = OpCode.JUMP;
                    ins.pointer = body_length + step_length + 2;
                    buffer[startPos + pos] = ins;
                    pos++;
                    pos = pos + body_length + step_length;

                    ins.opCode = OpCode.JUMPA;
                    ins.pointer = -(pos - init_size);
                    buffer[startPos + pos] = ins;
                    pos++;

                    //Scan any break and continue instructions
                    for (int i = 0; i < pos - init_size; ++i)
                    {
                        if (buffer[startPos + init_size + i].opCode == OpCode.PSEUDO_BREAK)
                        {
                            buffer[startPos + init_size + i].opCode = OpCode.JUMPA;
                            buffer[startPos + init_size + i].pointer = pos - i - init_size;
                        }
                        else if (buffer[startPos + init_size + i].opCode == OpCode.PSEUDO_CONTINUE)
                        {
                            buffer[startPos + init_size + i].opCode = OpCode.JUMPA;
                            buffer[startPos + init_size + i].pointer = pos - i - init_size - step_length - 1;
                        }
                    }

                    SymbolSetCodeStartPos(startPos);
                    return pos;
                }
            }

            #endregion
        }
    }
}