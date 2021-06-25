using System;
using System.Collections.Generic;
using Interpreter;

namespace MyProgram
{

    class print
        : IFunction
    {
        public Varible invoke(GcMachine m, int numParams) //Note that the interface has changed
        {
            Varible v;
            for (int i = 1; i <= numParams; ++i) //param have ID from 1 to numParams(possibly 0)
            {
                v = m.GetLocalByID(i); //How to access each param, do not specify ID less than 1 or greater than numParams
                if (v.type == VarType.TYPE_NUMBER)
                {
                    System.Console.Write(v.num.ToString()); //How to access number
                }
                else if (v.type == VarType.TYPE_STRING)
                {
                    System.Console.Write(m.GetString(v.pointer)); //For string ,you must use getString method to convert a string-pointer to actual string
                }
                else
                    throw new RuntimeException("Type mismatch"); //You can throw any runtime exception too.
            }
            System.Console.Write("\n");
            return new Varible(0.0); //You can return any number here.
        }
    }


    interface Command
    {
        string GetName();
        bool Run(GcMachine m, string[] param);
    }

    class run1
        : Command
    {
        public string GetName()
        {
            return "run";
        }
        public bool Run(GcMachine m, string[] param)
        {
            m.Execute();
            return true;
        }
    }

    class stack
        : Command
    {
        public string GetName()
        {
            return "stack";
        }
        public bool Run(GcMachine m, string[] param)
        {
            List<StackInfo> L = m.GetCallStack();
            foreach (StackInfo s in L)
            {
                string st = s.funcName + " (Line" + s.line + "): (";
                foreach (Varible v in s.parmList)
                {
                    if (v.type == VarType.TYPE_NUMBER)
                        st += v.num.ToString();
                    else if (v.type == VarType.TYPE_STRING)
                        st = st + "\"" + m.GetString(v.pointer).Replace("\n", "").Replace("\r", "") + "\"";
                    else
                        st += "(function)";
                    st = st + ",";
                }
                st += ")";
                System.Console.WriteLine(st);
            }
            return false;
        }
    }

    class bp
        : Command
    {
        public string GetName()
        {
            return "bp";
        }
        public bool Run(GcMachine m, string[] param)
        {
            m.AddBreakpoint(Convert.ToInt32(param[1]), true);
            return false;
        }
    }

    class blist
        : Command
    {
        public string GetName()
        {
            return "blist";
        }
        public bool Run(GcMachine m, string[] param)
        {
            Dictionary<int,bool> L = m.GetBreakpointList();
            foreach (KeyValuePair<int, bool> kv in L)
            {
                System.Console.WriteLine(kv.Key.ToString());
            }
            return false;
        }
    }

    class br
        : Command
    {
        public string GetName()
        {
            return "br";
        }
        public bool Run(GcMachine m, string[] param)
        {
            m.RemoveBreakpoint(Convert.ToInt32(param[1]));
            return false;
        }
    }

    class stepin
        :Command
    {
        public string GetName()
        {
            return "stepin";
        }
        public bool Run(GcMachine m, string[] param)
        {
            m.StepIn();
            return true;
        }
    }

    class stepover
        : Command
    {
        public string GetName()
        {
            return "stepover";
        }
        public bool Run(GcMachine m, string[] param)
        {
            m.StepOver();
            return true;
        }
    }

    class stepout
        : Command
    {
        public string GetName()
        {
            return "stepout";
        }
        public bool Run(GcMachine m, string[] param)
        {
            m.StepOut();
            return true;
        }
    }

    class global
        :Command
    {
        public string GetName()
        {
            return "global";
        }
        public bool Run(GcMachine m, string[] param)
        {
            if(!m.IsGlobalExists(param[1]))
            {
                System.Console.WriteLine("Varible " + param[1] + "has not been initialized yet.");
            } 
            else
            {
                Varible v = m.GetGlobal(param[1]);
                string st = "Varible " + param[1] + ":";
                if (v.type == VarType.TYPE_NUMBER)
                    st += v.num.ToString();
                else if (v.type == VarType.TYPE_STRING)
                    st = st + "\"" + m.GetString(v.pointer).Replace("\n", "").Replace("\r", "") + "\"";
                else
                    st += "(function)";
                System.Console.WriteLine(st);
            }
            return false;
        }
    }

    class local
        :Command
    {
        public string GetName()
        {
            return "local";
        }
        public bool Run(GcMachine m, string[] param)
        {
            if (m.GetLocalIDByName(param[1]) == -1)
            {
                System.Console.WriteLine("Local varible " + param[1] + "do not exist.");
            }
            else
            {
                Varible v = m.GetLocalByID(m.GetLocalIDByName(param[1]));
                string st = "Local varible " + param[1] + ":";
                if (v.type == VarType.TYPE_NUMBER)
                    st += v.num.ToString();
                else if (v.type == VarType.TYPE_STRING)
                    st = st + "\"" + m.GetString(v.pointer).Replace("\n", "").Replace("\r", "") + "\"";
                else
                    st += "(function)";
                System.Console.WriteLine(st);
            }
            return false;
        }
        
    }

    class Program
    {
        /*static void Main(string[] args)
        {
            FileStream fs;
            Machine m = new Machine();
            try
            {
                if (args.Length > 0)
                    fs = new FileStream(args[0]);
                else
                    fs = new FileStream("script.txt");

                

                //Step 1 Compile you code
                m.compile(fs);
                //Step 2 Register any functions you have
                m.setGlobal("print", new Varible(new print()));
                //Setp 3 Execute your code
                //m.execute();
            }
            catch (CompileException e)
            {
                string str = "Line " + e.Line.ToString() + " Error: " + e.Description + "\n";
                System.Console.Write(str);
            }
            catch (RuntimeException e)
            {
                string str = "Runtime Error: " + e.Description + "\n";
                System.Console.Write(str);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            List<Command> commandList = new List<Command>();
            commandList.Add(new bp());
            commandList.Add(new run1());
            commandList.Add(new stack());
            commandList.Add(new br());
            commandList.Add(new blist());
            commandList.Add(new stepin());
            commandList.Add(new stepout());
            commandList.Add(new stepover());
            commandList.Add(new global());
            commandList.Add(new local());

            bool running = false;
            do
            {
                string s = System.Console.ReadLine();
                string[] slist = s.Split(' ');
                foreach(Command c in commandList)
                {
                    if (c.getName() == slist[0])
                        running = c.run(m,slist);
                }

                if (running)
                {
                    System.Threading.Thread.Sleep(100);

                    while (m.Status == MachineStatus.MACHINE_STATUS_RUNNING)
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                    if (m.Status == MachineStatus.MACHINE_STATUS_DEAD)
                    {
                        System.Console.Write("At Line " + m.CurrentLine.ToString() + " Runtime error: " + m.LastErrorMessage);
                    }
                    if (m.Status == MachineStatus.MACHINE_STATUS_PAUSED)
                    {
                        System.Console.WriteLine("Break at line " + m.CurrentLine);
                    }
                }
            } while (m.Status == MachineStatus.MACHINE_STATUS_PAUSED);
            
            System.Console.ReadKey();
        }*/
    }
    
}
