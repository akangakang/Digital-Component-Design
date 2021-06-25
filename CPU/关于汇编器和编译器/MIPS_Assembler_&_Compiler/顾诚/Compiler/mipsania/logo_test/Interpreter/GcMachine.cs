using System;
using System.Collections.Generic;
using Interpreter.Inner;

namespace Interpreter
{
    namespace Inner
    {
        enum MachineFlags
        {
            MACHINE_FLAGS_NONE = 0,
            MACHINE_FLAGS_STEP_IN = 1,
            MACHINE_FLAGS_STEP_OUT = 2,
            MACHINE_FLAGS_STEP_OVER = 3
        }
    }
    public enum MachineStatus
    {
        MACHINE_STATUS_PAUSED = 1,
        MACHINE_STATUS_RUNNING = 2,
        MACHINE_STATUS_DEAD = 3,
        MACHINE_STATUS_FINISHED = 4
    }
    
    public class GcMachine
    {
        private const int stackSize = 131072;
        private const int codeSize = 131072;
        private const int maxLines = 65536;
        public Varible[] userStack;
        public int[] systemStack;
        private int userStackTop;
        private int systemStackTop;
        private int userStackBase;

        public Instruction[] code;
        private int[] lineCode;
        private int[] codeLine;
        private SortedDictionary<int, string> functionTable;
        private Dictionary<string, Varible> globalMap;
        private List<string> stringMap;
        private Dictionary<string, List<string>> locals;
        private List<string> curFuncLocals;
        private string parser_currentFunction;
        private string errorMessage;
        private int stepOutStackFrame;
        private bool compileComplete;
        private IFunction inSystemFunction;

        private int pc;
        private MachineFlags flags;
        private int codeLength;
        private bool cmpState;
        private bool wantPause;
        private Dictionary<int,bool> breakpointList;

        private System.Threading.EventWaitHandle executeEvent;

        public int CodeLength
        {
            get
            {
                return codeLength;
            }
            set
            {
                codeLength = value;
            }
        }
        private MachineStatus status;

        public MachineStatus Status
        {
            get
            {
                return status;
            }
        }

        public string LastErrorMessage
        {
            get
            {
                return errorMessage;
            }
        }

        public int CurrentLine
        {
            get
            {
                return symbol_getLineByCode(pc);
            }
        }

        public int CurrentFunction
        {
            get
            {
                return symbol_getFunctionByLine(symbol_getLineByCode(pc));
            }
        }

        public bool CompileComplete
        {
            get
            {
                return compileComplete;
            }
        }

        public GcMachine()
        {
            userStack = new Varible[stackSize];
            systemStack = new int[stackSize];
            userStackTop = 0;
            systemStackTop = 0;
            userStackBase = 0;
            code = new Instruction[codeSize];
            globalMap = new Dictionary<string,Varible>();
            stringMap = new List<string>();
            locals = new Dictionary<string, List<string>>();
            wantPause = false;
            flags = MachineFlags.MACHINE_FLAGS_NONE;
            breakpointList = new Dictionary<int, bool>();
            functionTable = new SortedDictionary<int,string>();
            inSystemFunction = null;
            lineCode = new int[maxLines];
            executeEvent = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset);
            for (int i = 0; i < maxLines; ++i)
            {
                lineCode[i] = -1;
            }
            status = MachineStatus.MACHINE_STATUS_PAUSED;
            compileComplete = false;
        }
        #region Used only by parser

        public void parser_setCurrentFunction(string func)
        {
            if (!locals.ContainsKey(func))
            {
                locals[func] = new List<string>();
            }
            curFuncLocals = locals[func];
            parser_currentFunction = func;
        }

        public string parser_getCurrentFunction()
        {
            return parser_currentFunction;
        }

        public int parser_checkStringMap(string name, bool add)
        {
            if (!stringMap.Contains(name))
            {
                if (!add)
                    return -1;
                stringMap.Add(name);
            }
            return stringMap.IndexOf(name);
        }

        public void parser_addLocal(string name)
        {
            if (!curFuncLocals.Contains(name))
                curFuncLocals.Add(name);
        }

        public int parser_getLocalCount()
        {
            return curFuncLocals.Count;
        }

        public void parser_setLineCode(int line, int codeStartPos)
        {
            if (lineCode[line] == -1 ||
                lineCode[line] > codeStartPos)
                lineCode[line] = codeStartPos;
        }
        public void parser_register_function(int startLine, string name)
        {
            functionTable[startLine] = name;
        }

        private int symbol_getFirstCodeByLine(int line)
        {
            return lineCode[line];
        }

        private void symbol_build()
        {
            codeLine = new int[codeLength];
            SortedDictionary<int, int> s = new SortedDictionary<int, int>();
            for (int i = 0; i < codeLength; ++i)
            {
                codeLine[i] = 1;
            }
            for (int i = 1; i < maxLines; ++i)
            {
                if (lineCode[i] != -1)
                {
                    if (!s.ContainsKey(lineCode[i]))
                        s.Add(lineCode[i], i);
                    else
                        s[lineCode[i]] =  i;
                }
            }
            foreach (KeyValuePair<int, int> kv in s)
            {
                for (int i = kv.Key; i < codeLength; ++i)
                {
                    codeLine[i] = kv.Value;
                }
            }

        }
        private int symbol_getLineByCode(int code)
        {
            return codeLine[code];
        }
        private int symbol_getFunctionByLine(int line)
        {
            string last = "";
            foreach (KeyValuePair<int, string> kvp in functionTable)
            {
                if (kvp.Key > line)
                    return parser_checkStringMap(kvp.Value, true);
                last = kvp.Value;
            }
            return parser_checkStringMap(last, true); 
        }
        #endregion

        public int GetLocalIDByName(string name)
        {
            if (compileComplete)
            {
                parser_setCurrentFunction(GetString(symbol_getFunctionByLine(CurrentLine)));
            }
            if (!curFuncLocals.Contains(name))
                    return -1;
            else
                return curFuncLocals.IndexOf(name) + 1;
        }


        public void SetGlobal(string name, Varible var)
        {
            globalMap[name] = var;
        }

        public Varible GetGlobal(string name)
        {
            if (!globalMap.ContainsKey(name))
                throw new RuntimeException("Global varible " + name + " have not been initialized yet");
            return globalMap[name];
        }

        public bool IsGlobalExists(string name)
        {
            return globalMap.ContainsKey(name);
        }

        public void SetLocalByID(int id, Varible var)
        {
            userStack[userStackBase + id - 1] = var;
        }

        public Varible GetLocalByID(int id)
        {
            return userStack[userStackBase + id - 1];
        }

        private void setPCMain()
        {
            if (!IsGlobalExists("main"))
                throw new CompileException(1, "Global function \"main\" not found");
            Varible t = GetGlobal("main");
            if (t.type != VarType.TYPE_FUNCTION)
                throw new CompileException(1,"Function main type mismatch");
            //Jump to PC
            code[0].opCode = OpCode.JUMPA;
            code[0].pointer = GetGlobal("main").pointer;
            //pc = GetGlobal("main").pointer;
        }

        public string GetString(int id)
        {
            return stringMap[id];
        }

        public void Pause()
        {
            wantPause = true;
        }

        public void AddBreakpoint(int line, bool persistent)
        {
            while (line < maxLines && symbol_getFirstCodeByLine(line) == -1)
                ++line;
            if (line == maxLines) return;
            if (breakpointList.ContainsKey(line) && breakpointList[line])
                return; 
            breakpointList[line] = persistent;
            code[symbol_getFirstCodeByLine(line)].opCode |= 0x80;
        }

        public void RemoveBreakpoint(int line)
        {
            if (breakpointList.ContainsKey(line))
                breakpointList.Remove(line);
            code[symbol_getFirstCodeByLine(line)].opCode &= 0x7F;
        }

        public Dictionary<int, bool> GetBreakpointList()
        {
            return breakpointList;
        }

        public List<StackInfo> GetCallStack()
        {
            List<StackInfo> L = new List<StackInfo>();
            StackInfo s;
            int maxStackIndex = stackSize - 1;
            int t = systemStackTop > maxStackIndex ?
                maxStackIndex - maxStackIndex % 3 : systemStackTop;
            int oldPC = 0;
            int usb = userStackBase;
            s.line = symbol_getLineByCode(pc);
            s.funcName = GetString(symbol_getFunctionByLine(s.line));
            if (inSystemFunction != null) s.funcName = "<System Function>:" + inSystemFunction.GetType().ToString();
            while (t > 0)
            {
                s.parmList = new List<Varible>();
                for (int i = 0; i < systemStack[t - 1]; ++i)
                {
                    s.parmList.Add(userStack[usb + i]);
                }
                L.Add(s);
                usb = systemStack[t - 2];
                oldPC = systemStack[t - 3];
                s.line = symbol_getLineByCode(oldPC);
                s.funcName = GetString(symbol_getFunctionByLine(s.line));
                t -= 3;
            }
            s.parmList = new List<Varible>();
            L.Add(s);

            return L;
        }

        public void StepIn()
        {
            stepOutStackFrame = systemStackTop;
            flags = MachineFlags.MACHINE_FLAGS_STEP_IN;
            _execute();
        }

        public void StepOver()
        {
            stepOutStackFrame = systemStackTop;
            if (pc == 0)
                flags = MachineFlags.MACHINE_FLAGS_STEP_IN;
            else
                flags = MachineFlags.MACHINE_FLAGS_STEP_OVER;
            _execute();
        }

        public void StepOut()
        {
            stepOutStackFrame = systemStackTop;
            flags = MachineFlags.MACHINE_FLAGS_STEP_OUT;
            _execute();
        }

        public void Execute()
        {
            flags = MachineFlags.MACHINE_FLAGS_NONE;
            _execute();
        }

        private void _execute()
        {
            System.Threading.Thread t = new System.Threading.Thread(executeThread);
            executeEvent.Reset();
            t.Start();
            executeEvent.WaitOne(System.Threading.Timeout.Infinite);
            
        }

        private void executeThread()
        {
            int currentLine = CurrentLine;
            wantPause = false;
            try
            {
                status = MachineStatus.MACHINE_STATUS_RUNNING;
                executeEvent.Set();
                executeInner();
                status = MachineStatus.MACHINE_STATUS_FINISHED;
            }
            catch (BreakpointException)
            {
                status = MachineStatus.MACHINE_STATUS_PAUSED;
                int line = symbol_getLineByCode(pc);
                if (breakpointList.ContainsKey(line) && breakpointList[line] == false)
                    RemoveBreakpoint(line);
            }
            catch (RuntimeException e)
            {
                status = MachineStatus.MACHINE_STATUS_DEAD;
                errorMessage = e.Description;
            }
            catch (Exception e)
            {
                status = MachineStatus.MACHINE_STATUS_DEAD;
                errorMessage = e.Message;
            }
        }

        public void Compile(IStream source)
        {
            Lexer l = new Lexer(source);
            Parser p = new Parser(this, l);
            Library.OpenLibrary(this);
            p.parse();
            symbol_build();
            setPCMain();
            compileComplete = true;
        }

        private void executeInner()
        {
            Varible t;
            Varible __parent = new Varible("__parent");
            int n;
            double num;
            inSystemFunction = null;
            int lastLine = symbol_getLineByCode(pc);
            int currFunc = CurrentFunction;
            short opCode = Convert.ToInt16(code[pc].opCode & 0x7F);
            while (pc < codeLength)
	        {
		        switch(opCode)
		        {
		            case OpCode.PUSHNUM:
                        userStack[userStackTop].num = code[pc].num;
                        userStack[userStackTop].type = VarType.TYPE_NUMBER;
                        userStackTop ++;
			            break;
		            case OpCode.POP:
			            userStackTop -= code[pc].pointer;
			            break;
		            case OpCode.GETLOCAL:
			            userStack[userStackTop] = userStack[userStackBase + code[pc].pointer - 1];
                        userStackTop ++;
			            break;
		            case OpCode.SETLOCAL:
			            userStack[userStackBase + code[pc].pointer - 1] = userStack[userStackTop - 1];
			            //userStackTop --;
			            break;
		            case OpCode.GETGLOBAL:
                        userStack[userStackTop] = GetGlobal(stringMap[code[pc].pointer]);
                        userStackTop ++;
			            break;
		            case OpCode.SETGLOBAL:
                        SetGlobal(stringMap[code[pc].pointer],userStack[userStackTop - 1]);
			            //userStackTop --;
			            break;
		            case OpCode.CALL:
                        systemStack[systemStackTop] = pc;
                        systemStackTop ++;
                        systemStack[systemStackTop] = userStackBase;
                        systemStackTop ++;
                        systemStack[systemStackTop] = code[pc].pointer;
                        systemStackTop++;
			            t = userStack[--userStackTop]; // new_address
			            userStackBase = userStackTop - code[pc].pointer;
			            if (t.type == VarType.TYPE_FUNCTION)
			            {
				            pc = t.pointer - 1;
                            //opCode = code[pc].opCode;
				            break;
                        }
                        else if (t.type == VarType.TYPE_C_FUNCTION)
                        {
                            //C function
                            inSystemFunction = t.func;
                            userStack[userStackTop] = t.func.invoke(this, code[pc].pointer);
                            userStackTop++;
                            inSystemFunction = null;
                            goto retInstruction;
                            // Fall through to RET 
                        }
                        else
                        {
                            userStackBase = systemStack[systemStackTop - 2];
                            systemStackTop -= 3;
                            throw new RuntimeException("Unable to call " + t.num.ToString());
                        }
		            case OpCode.RET:
                    retInstruction:
                        if (systemStackTop == 0)
                            return;
			            t = userStack[userStackTop - 1]; //retval
			            userStackTop = userStackBase;
                        systemStackTop--; //pop num param
                        userStackBase = systemStack[--systemStackTop];
                        pc = systemStack[--systemStackTop];
                        opCode = code[pc].opCode;

                        userStack[userStackTop++] = t;
                        if (systemStackTop < stepOutStackFrame && 
                            (flags == MachineFlags.MACHINE_FLAGS_STEP_OUT ||
                            flags == MachineFlags.MACHINE_FLAGS_STEP_OVER || 
                            flags == MachineFlags.MACHINE_FLAGS_STEP_IN)
                            )
                        {
                            ++pc;
                            throw new BreakpointException();
                        }
			            break;
		            case OpCode.CMP:
                        if (userStack[userStackTop - 1].type != VarType.TYPE_NUMBER || userStack[userStackTop - 2].type != VarType.TYPE_NUMBER)
                            throw new RuntimeException("Unable to comapre " + VarType.getTypeString(userStack[userStackTop - 1].type) + " and " 
                                + VarType.getTypeString(userStack[userStackTop - 2].type) );
			            num = userStack[userStackTop - 2].num - userStack[userStackTop - 1].num;
			            if ( Math.Abs(num) < 1e-10 )
			            {
				            cmpState = (code[pc].pointer & CompareMode.COMPARE_EQUAL) != 0;
			            } else
			            {
				            if (num < 0)
                                cmpState = (code[pc].pointer & CompareMode.COMPARE_LESS) != 0;
				            else
                                cmpState = (code[pc].pointer & CompareMode.COMPARE_GREATER) != 0;
			            }
                        --userStackTop;
                        if (cmpState)
			            {
				            userStack[userStackTop - 1].num = 1.0;
			            }
			            else
			            {
                            userStack[userStackTop - 1].num = 0.0;
			            }
			            break;
		            case OpCode.JUMP:
                        cmpState = !(Math.Abs(userStack[--userStackTop].num) < 1e-10);
                        if (!cmpState)
                            break;
                        else
                        {
                            pc += (code[pc].pointer - 1);
                            //opCode = code[pc].opCode;
                            break;
                        }
		            case OpCode.JUMPA:
                        pc += (code[pc].pointer - 1);
                        //opCode = code[pc].opCode;
			            break;
		            case OpCode.ADJLOCAL:
			            n = userStackTop - userStackBase;
			            while(n > code[pc].pointer)
			            {
                            --userStackTop;
				            --n;
			            }
			            while(n < code[pc].pointer)
			            {
                            userStack[userStackTop] = new Varible(0.0);
                            ++userStackTop;
				            ++n;
			            }
			            break;
		            case OpCode.NOT:
                        if (Math.Abs(userStack[userStackTop-1].num) < 1e-10)
                            userStack[userStackTop - 1].num = 1;
			            else
                            userStack[userStackTop - 1].num = 0;
			            break;
		            case OpCode.ARITH:
                        if (code[pc].pointer == OpCode.ARITH_ADD && userStack[userStackTop - 1].type == VarType.TYPE_STRING &&
                            userStack[userStackTop - 2].type == VarType.TYPE_STRING)
                        {
                            userStack[userStackTop - 2].str = userStack[userStackTop - 2].str + userStack[userStackTop - 1].str;
                            userStackTop--;
                            break;
                        }
                        if (userStack[userStackTop - 1].type != VarType.TYPE_NUMBER || userStack[userStackTop - 2].type != VarType.TYPE_NUMBER)
                            throw new RuntimeException("Unable to perform arithmatic on " + VarType.getTypeString(userStack[userStackTop - 1].type) + " and "
                                + VarType.getTypeString(userStack[userStackTop - 2].type) );
			            switch (code[pc].pointer)
			            {
                            case OpCode.ARITH_ADD:
                                num = userStack[userStackTop - 2].num + userStack[userStackTop - 1].num;
				                break;
                            case OpCode.ARITH_SUB:
                                num = userStack[userStackTop - 2].num - userStack[userStackTop - 1].num;
				                break;
                            case OpCode.ARITH_MUL:
                                num = userStack[userStackTop - 2].num * userStack[userStackTop - 1].num;
				                break;
                            case OpCode.ARITH_DIV:
                                num = userStack[userStackTop - 2].num / userStack[userStackTop - 1].num;
				                break;
			            default:
                                throw new RuntimeException("Unknown arith type " + code[pc].pointer.ToString());
			            }
                        userStackTop--;
                        userStack[userStackTop - 1].num = num;
			            break;
                    case OpCode.PUSHSTRING:
                        userStack[userStackTop].str = GetString(code[pc].pointer);
                        userStack[userStackTop].type = VarType.TYPE_STRING;
                        userStackTop ++;
			            break;
                    case OpCode.TABLEOP:
                        switch (code[pc].pointer)
                        {
                            case OpCode.TABLE_NEW:
                                userStack[userStackTop].table = new Dictionary<Varible,Varible>(VarCompare.GetInstance());
                                userStack[userStackTop].type = VarType.TYPE_TABLE;
                                userStackTop ++;
                                break;
                            case OpCode.TABLE_GETITEM:
                                Dictionary<Varible, Varible> table;
                                int i = 0;
                                if (userStack[userStackTop - 2].type != VarType.TYPE_TABLE)
                                {
                                    if (userStack[userStackTop - 2].type == VarType.TYPE_STRING)
                                    {
                                        if (IsGlobalExists("String") && GetGlobal("String").type == VarType.TYPE_TABLE)
                                            table = GetGlobal("String").table;
                                        else
                                            throw new RuntimeException("Not a table");
                                    }
                                    else
                                        throw new RuntimeException("Not a table");
                                }
                                else
                                    table = userStack[userStackTop - 2].table;
                                while (!table.ContainsKey(userStack[userStackTop - 1]) && table.ContainsKey(__parent) &&
                                    table[__parent].type == VarType.TYPE_TABLE)
                                {
                                    table = table[__parent].table;
                                    ++i;
                                    if (i > 256)
                                        throw new RuntimeException("Max levels of __parent exceeded");
                                }
                                if (!table.ContainsKey(userStack[userStackTop - 1]) && IsGlobalExists("Table") &&
                                    GetGlobal("Table").type == VarType.TYPE_TABLE) 
                                    table = GetGlobal("Table").table;
                                if (!table.ContainsKey(userStack[userStackTop - 1]))
                                    throw new RuntimeException("Key do not exist");
                                userStack[userStackTop - 2] = table[userStack[userStackTop - 1]];
                                userStackTop--;
                                break;
                            case OpCode.TABLE_SETITEM:
                                if (userStack[userStackTop - 2].type != VarType.TYPE_TABLE)
                                    throw new RuntimeException("Not a table");
                                userStack[userStackTop - 2].table[userStack[userStackTop - 1]] = userStack[userStackTop - 3];
                                userStackTop -= 2; //3
                                break;
                            case OpCode.TABLE_SETITEM_INIT:
                                if (userStack[userStackTop - 3].type != VarType.TYPE_TABLE)
                                    throw new RuntimeException("Not a table");
                                userStack[userStackTop - 3].table[userStack[userStackTop - 1]] = userStack[userStackTop - 2];
                                userStackTop -= 1; //
                                break;
                            default:
                                throw new RuntimeException("Unknown table op");
                        }
                        break;
		            case OpCode.PSEUDO_BREAK:
		            case OpCode.PSEUDO_CONTINUE:
                        throw new RuntimeException("Unexpected break or continue outside while loop");
		            default:
                        if ((code[pc].opCode & 0x80) != 0)
                            throw new BreakpointException();
                        else
                            throw new RuntimeException("Unknown instruction");
		        }
		        ++pc;
                opCode = code[pc].opCode;
                if (wantPause)
                    throw new BreakpointException();
                if (flags == MachineFlags.MACHINE_FLAGS_STEP_IN)
                {
                    if (lastLine != symbol_getLineByCode(pc))
                        throw new BreakpointException();
                }
                if (flags == MachineFlags.MACHINE_FLAGS_STEP_OVER)
                {
                    if (systemStackTop == stepOutStackFrame &&  CurrentFunction == currFunc && 
                        lastLine != symbol_getLineByCode(pc))
                        throw new BreakpointException();
                }
	        }
	        return;
        }
    }
}
