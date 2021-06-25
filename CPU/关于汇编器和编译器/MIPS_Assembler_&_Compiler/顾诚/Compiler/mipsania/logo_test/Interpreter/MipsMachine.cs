using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Interpreter
{
    public class MipsMachine
    {
        private MachineStatus status = MachineStatus.MACHINE_STATUS_PAUSED;
        public MachineStatus Status
        {
            get { return status; }
            set { status = value; }
        }
        private bool compileComplete = false;
        public bool CompileComplete { get { return compileComplete; } }
        private int pc = -1;
        public int CurrentLine { get { return instructions[pc].OriginLineNum; } }
        public string LastErrorMessage { get; set; }

        private Dictionary<int, bool> breakpoints = new Dictionary<int, bool>();
        public Dictionary<int, bool> GetBreakpointList()
        {
            Dictionary<int, bool> result = new Dictionary<int, bool>();
            foreach (var item in breakpoints)
            {
                result.Add(instructions[item.Key].OriginLineNum, item.Value);
            }
            return result;
        }

        public void RemoveBreakpoint(int line)
        {
            int exact = lineNumOriginToExact[line];
            if (breakpoints.ContainsKey(exact))
                breakpoints.Remove(exact);
        }

        public void AddBreakpoint(int line, bool persistent)
        {
            int exact = lineNumOriginToExact[line];
            if (!breakpoints.ContainsKey(exact))
                breakpoints.Add(exact, persistent);
            else if (!breakpoints[exact] && persistent)
                breakpoints[exact] = true;
        }

        public void Execute() {
            status = MachineStatus.MACHINE_STATUS_RUNNING;
            System.Threading.Thread t = new System.Threading.Thread(ExecuteThread);
            //executeEvent.Reset();
            t.Start();
            //executeEvent.WaitOne(System.Threading.Timeout.Infinite);
        }

        private void ExecuteThread()
        {
            while (!needPause)
            {
                int oldpc = pc;
                ExecuteLine();
                if (pc == oldpc || pc == instructions.Count)
                {
                    if (pc == instructions.Count)
                    {
                        pc = instructions.Count - 1;
                    }
                    status = MachineStatus.MACHINE_STATUS_FINISHED;
                    needPause = false;
                    break;
                }
                if (breakpoints.ContainsKey(pc))
                {
                    if (!breakpoints[pc])
                        breakpoints.Remove(pc);
                    break;
                }
            }
            status = MachineStatus.MACHINE_STATUS_PAUSED;
            needPause = false;
        }

        private bool needPause = false;
        public void Pause()
        {
            needPause = true;
        }
        public void StepIn()
        {
            ExecuteLine();
        }
        public void StepOut()
        {
            ExecuteLine();
        }
        public void StepOver()
        {
            if (pc + 1 < instructions.Count)
                breakpoints.Add(pc + 1, false);
            Execute();
        }

        private List<int> registers = new List<int>();
        public List<int> Registers
        {
            get { return registers; }
        }
        private Dictionary<int, int> memory = new Dictionary<int, int>();
        public Dictionary<int, int> Memory
        {
            get { return memory; }
        }

        private void ExecuteLine()
        {
            if (pc == -1 || pc >= instructions.Count)
            {
                pc = 0;
                return;
            }

            Instruction inst = instructions[pc];

            if (inst.Format.Key == "add")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] + registers[inst.Register[2]];
                pc++;
            }
            else if (inst.Format.Key == "sub")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] - registers[inst.Register[2]];
                pc++;
            }
            else if (inst.Format.Key == "and")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] & registers[inst.Register[2]];
                pc++;
            }
            else if (inst.Format.Key == "or")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] | registers[inst.Register[2]];
                pc++;
            }
            else if (inst.Format.Key == "xor")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] ^ registers[inst.Register[2]];
                pc++;
            }
            else if (inst.Format.Key == "sll")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] << inst.Immediate;
                pc++;
            }
            else if (inst.Format.Key == "srl")
            {
                registers[inst.Register[0]] = (int)((uint)registers[inst.Register[1]] >> inst.Immediate);
                pc++;
            }
            else if (inst.Format.Key == "sra")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] >> inst.Immediate;
                pc++;
            }
            else if (inst.Format.Key == "jr")
            {
                pc = registers[inst.Register[0]];
            }
            else if (inst.Format.Key == "addi")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] + inst.Immediate;
                pc++;
            }
            else if (inst.Format.Key == "andi")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] & inst.Immediate;
                pc++;
            }
            else if (inst.Format.Key == "ori")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] | inst.Immediate;
                pc++;
            }
            else if (inst.Format.Key == "xori")
            {
                registers[inst.Register[0]] = registers[inst.Register[1]] ^ inst.Immediate;
                pc++;
            }
            else if (inst.Format.Key == "lw")
            {
                int address = registers[inst.Register[1]] + inst.Immediate;
                if (memory.ContainsKey(address))
                    registers[inst.Register[0]] = memory[address];
                else
                    registers[inst.Register[0]] = 0;
                pc++;
            }
            else if (inst.Format.Key == "sw")
            {
                int address = registers[inst.Register[1]] + inst.Immediate;
                if (memory.ContainsKey(address))
                    memory[address] = registers[inst.Register[0]];
                else
                    memory.Add(address, registers[inst.Register[0]]);
                pc++;
            }
            else if (inst.Format.Key == "beq")
            {
                if (registers[inst.Register[0]] == registers[inst.Register[1]])
                {
                    pc = pc + 1 + inst.Immediate;
                }
                else
                {
                    pc++;
                }
            }
            else if (inst.Format.Key == "bne")
            {
                if (registers[inst.Register[0]] != registers[inst.Register[1]])
                {
                    pc = pc + 1 + inst.Immediate;
                }
                else
                {
                    pc++;
                }
            }
            else if (inst.Format.Key == "lui")
            {
                registers[inst.Register[0]] = inst.Immediate << 16;
                pc++;
            }
            else if (inst.Format.Key == "j")
            {
                pc = inst.Immediate;
            }
            else if (inst.Format.Key == "jal")
            {
                registers[31] = pc+1;
                pc = inst.Immediate;
            }
        }

        public class InstructionFormat
        {
            public InstructionFormat(string key, string format, string[] inputs, int [] outputs)
            {
                Key = key;
                Format = format;
                Inputs = inputs;
                Outputs = outputs;
            }
            public string Key;
            public string Format;
            public string[] Inputs;
            public int[] Outputs;
        }
        private List<InstructionFormat> instructionFormat = new List<InstructionFormat>();
        public MipsMachine()
        {
            instructionFormat.Add(new InstructionFormat(
                "add", "000000xxx00000100000", new string[] { "r0", "r1", "r2" }, new [] {2,3,1}));
            instructionFormat.Add(new InstructionFormat(
                "sub", "000000xxx00000100010", new string[] { "r0", "r1", "r2" }, new[] { 2,3,1 }));
            instructionFormat.Add(new InstructionFormat(
                "and", "000000xxx00000100100", new string[] { "r0", "r1", "r2" }, new[] { 2,3,1 }));
            instructionFormat.Add(new InstructionFormat(
                "or", "000000xxx00000100101", new string[] { "r0", "r1", "r2" }, new[] { 2,3,1 }));
            instructionFormat.Add(new InstructionFormat(
                "xor", "000000xxx00000100110", new string[] { "r0", "r1", "r2" }, new[] { 2,3,1 }));
            instructionFormat.Add(new InstructionFormat(
                "sll", "00000000000xxx000000", new string[] { "r0", "r1", "i5" }, new[] {2,1,3 }));
            instructionFormat.Add(new InstructionFormat(
                "srl", "00000000000xxx000010", new string[] { "r0", "r1", "i5" }, new[] { 2,1,3 }));
            instructionFormat.Add(new InstructionFormat(
                "sra", "00000000000xxx000011", new string[] { "r0", "r1", "i5" }, new[] { 2,1,3 }));
            instructionFormat.Add(new InstructionFormat(
                "jr", "000000x000000000000000001000", new string[] { "r0" }, new[] { 1 }));

            instructionFormat.Add(new InstructionFormat(
                "addi", "001000xxx", new string[] { "r0", "r1", "i16" }, new[] { 2, 1, 3 }));
            instructionFormat.Add(new InstructionFormat(
                "andi", "001100xxx", new string[] { "r0", "r1", "i16" }, new[] { 2, 1, 3 }));
            instructionFormat.Add(new InstructionFormat(
                "ori", "001101xxx", new string[] { "r0", "r1", "i16" }, new[] { 2, 1, 3 }));
            instructionFormat.Add(new InstructionFormat(
                "xori", "001110xxx", new string[] { "r0", "r1", "i16" }, new[] { 2, 1, 3 }));
            instructionFormat.Add(new InstructionFormat(
                "lw", "100011xxx", new string[] { "r0", "i16", "r1" }, new[] { 3, 1, 2 }));
            instructionFormat.Add(new InstructionFormat(
                "sw", "101011xxx", new string[] { "r0", "i16", "r1" }, new[] { 3, 1, 2 }));
            instructionFormat.Add(new InstructionFormat(
                "beq", "000100xxx", new string[] { "r0", "r1", "i16" },new[] { 1, 2, 3}));
            instructionFormat.Add(new InstructionFormat(
                "bne", "000101xxx", new string[] { "r0", "r1", "i16" }, new[] { 1, 2, 3 }));
            instructionFormat.Add(new InstructionFormat(
                "lui", "00111100000xx", new string[] { "r0", "i16" }, new[] { 1, 2}));

            instructionFormat.Add(new InstructionFormat(
                "j", "000010x", new string[] { "i26" }, new[] { 1}));
            instructionFormat.Add(new InstructionFormat(
                "jal", "000011x", new string[] { "i26" }, new[] { 1 }));
            instructionFormat.Add(new InstructionFormat(
                "nop", "00000000000000000000000000000000", new string[] {}, new int[] {}));

            for (int i = 0; i < 32; ++i)
                registers.Add(0);
        }

        private class Instruction
        {
            public int OriginLineNum;
            public int ExactLineNum;
            public List<int> Register = new List<int>();
            public int Immediate;
            public string ImmediateLabel;
            public InstructionFormat Format;
            public bool Breakpoint = false;
            public string ToBinary()
            {
                string s = Format.Format;
                for (int oi = 0; oi < Format.Outputs.Length; ++oi)
                {
                    int i = Format.Outputs[oi] - 1;
                    int index = s.IndexOf('x');
                    s = s.Remove(index, 1);
                    string trans;
                    if (Format.Inputs[i][0] == 'r') {
                        trans = NumToStringBinary(Register[Int32.Parse(Format.Inputs[i].Substring(1))], 5);
                    } else {
                        trans = NumToStringBinary(Immediate, Int32.Parse(Format.Inputs[i].Substring(1)));
                    }
                    s = s.Insert(index, trans);
                }
                return s;
            }

            public string ToHex()
            {
                string b = ToBinary();
                UInt32 num = 0;
                for (int i = 0; i < b.Length; ++i)
                {
                    num = num * 2 + (UInt32)(b[i] - '0');
                }
                return num.ToString("X8");
            }

            public string NumToStringBinary(int num, int length)
            {
                string s = "";
                bool negative = (num < 0);
                num = Math.Abs(num);
                if (negative) num = -(num) + 1;
                for (int i = 0; i < length; ++i)
                {
                    if (num % 2 == 0)
                        s = s + "0";
                    else
                        s = s + "1";
                    num /= 2;
                }
                if (negative)
                {
                    s = s.Replace('0', 'x');
                    s = s.Replace('1', '0');
                    s = s.Replace('x', '1');
                }
                string result = "";
                for (int i = s.Length - 1; i >= 0; --i)
                    result += s[i];
                return result;
            }


        }
        private List<Instruction> instructions = new List<Instruction>();
        private Dictionary<string, int> labelToExactLineNum = new Dictionary<string, int>();
        private Dictionary<int, int> lineNumOriginToExact = new Dictionary<int, int>();

        public void Compile(IStream source)
        {
            StringBuilder sb = new StringBuilder();
            while (!source.eof())
                sb.Append(source.next());
            string originCode = sb.ToString().Replace("\r", "");
            string[] lines = originCode.Split('\n');

            HashSet<string> labelCandidate = new HashSet<string>();
            instructions.Clear();
            labelToExactLineNum.Clear();

            for (int i = 0; i < lines.Length; ++i)
            {
                try
                {
                    string line = lines[i];
                    int indexComment = line.IndexOf('#');
                    if (indexComment >= 0)
                        line = line.Remove(indexComment);
                    line = line.Trim().ToLower().Replace('\t', ' ');
                    lines[i] = line;

                    while (true)
                    {
                        int labelIndex = line.IndexOf(':');
                        if (labelIndex < 0)
                            break;
                        string label = line.Remove(labelIndex).Trim();
                        labelCandidate.Add(label);
                        line = line.Substring(labelIndex + 1).Trim();
                    }

                    Instruction inst = new Instruction();
                    for (int j = 0; j < instructionFormat.Count; ++j)
                    {
                        if (line.IndexOf(instructionFormat[j].Key + " ") != 0)
                            continue;

                        inst.OriginLineNum = i + 1;
                        inst.ExactLineNum = instructions.Count;
                        SetInstruction(inst, instructionFormat[j], line);
                        instructions.Add(inst);

                        foreach (var lbl in labelCandidate)
                        {
                            if (labelToExactLineNum.ContainsKey(lbl))
                                throw new Exception("sb");
                            labelToExactLineNum.Add(lbl, instructions.Count - 1);
                        }
                        labelCandidate.Clear();
                        break;
                    }
                }
                catch (Exception e)
                {
                    throw new CompileException(i + 1, "您太二了！第一遍扫描时就出错了！目测与label无关。");
                }
            }

            for (int i = 0; i < instructions.Count; ++i)
            {
                try
                {
                    Instruction inst = instructions[i];
                    if (inst.ImmediateLabel != null)
                    {
                        string lbl = inst.ImmediateLabel;
                        if (inst.Format.Key == "beq" || inst.Format.Key == "bne")
                        {
                            int deltaLine = labelToExactLineNum[lbl] - inst.ExactLineNum;
                            inst.Immediate = deltaLine - 1;
                        }
                        else if (inst.Format.Key == "j" || inst.Format.Key == "jal")
                        {
                            inst.Immediate = labelToExactLineNum[lbl];
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new CompileException(instructions[i].OriginLineNum, "您太二了！第二遍扫描时出错了！目测是与label有关的错误！");
                }
            }

            int ol = 0;
            for (int i = 0; i < instructions.Count; ++i)
            {
                while (ol <= instructions[i].OriginLineNum)
                {
                    lineNumOriginToExact.Add(ol, i);
                    ++ol;
                }
            }

            ToMif("inst.mif", lines);
            LoadMem("mem.txt");

            compileComplete = true;
            //string info = "\n";
            //for (int i = 0; i < instructions.Count; ++i)
            //{
            //    info += instructions[i].ToBinary() + "\t" + i.ToString("X") + " : " + instructions[i].ToHex() + "\t" + lines[instructions[i].OriginLineNum] + "\n";
            //}
            //throw new CompileException(0, info);
        }

        private void LoadMem(string path)
        {
            try
            {
                TextReader tr = new StreamReader(path);
                string data = tr.ReadToEnd();
                string[] strs = data.Replace('\n', ' ').Replace('\r', ' ').Replace('\t',' ' ).Split(' ');
                bool first = true;
                int key = 0;
                for (int i = 0; i < strs.Length; ++i)
                {
                    string numStr = strs[i].Trim().ToLower();
                    if (numStr == "") continue;
                    bool negative = false;
                    if (numStr[0] == '-')
                    {
                        negative = true;
                        numStr = numStr.Substring(1);
                    }
                    int num = 0;
                    if (numStr.Length > 2 && numStr[1] == 'x')
                    {
                        numStr = numStr.Substring(2);

                        for (int j = 0; j < numStr.Length; ++j)
                        {
                            num = num * 16;
                            if (char.IsDigit(numStr[j]))
                                num += numStr[j] - '0';
                            else
                                num += 10 + numStr[j] - 'a';
                        }
                    }
                    else
                    {
                        num = Int32.Parse(numStr);
                    }
                    if (negative) num = -num;

                    if (first)
                    {
                        key = num;
                        first = false;
                    }
                    else
                    {
                        memory.Add(key, num);
                        first = true;
                    }
                }

                TextWriter tw = new StreamWriter("data.mif");
                tw.Write("DEPTH=64;\r\nWIDTH = 32;\r\nADDRESS_RADIX = HEX;\r\nDATA_RADIX = HEX;\r\nCONTENT\r\nBEGIN\r\n");
                foreach (var item in memory)
                {
                    tw.Write("{0} : {1};\r\n", item.Key.ToString("X"), item.Value.ToString("X8"));
                }
                tw.Write("END;\r\n");
                tw.Close();
            }
            catch (Exception) { }
        }

        private void ToMif(string path, string[] lines)
        {
            TextWriter tw = new StreamWriter(path);
            tw.Write("DEPTH=64;\r\nWIDTH = 32;\r\nADDRESS_RADIX = HEX;\r\nDATA_RADIX = HEX;\r\nCONTENT\r\nBEGIN\r\n");
            for (int i = 0; i < instructions.Count; ++i)
            {
                tw.Write("{0} : {1}; % {2} % \r\n", i.ToString("X"), instructions[i].ToHex(), lines[instructions[i].OriginLineNum - 1]);
            }
            tw.Write("END;\r\n");
            tw.Close();

        }

        private void SetInstruction(Instruction inst, InstructionFormat format, string line)
        {
            string[] split = line.Substring(format.Key.Length).Replace('(', ',').Replace(")", "").Replace('\t', ' ').Split(',');
            int formatIndex = 0;
            inst.Format = format;
            for (int i = 0; i < split.Length; ++i)
            {
                if (formatIndex >= format.Inputs.Length) return;
                string str = split[i].Trim();
                if (format.Inputs[formatIndex][0] == 'r')
                {
                    if (str == "$ra")
                        str = "$31";
                    inst.Register.Add(Int32.Parse(str.Substring(1)));
                }
                else if (format.Inputs[formatIndex][0] == 'i')
                {
                    if (Char.IsDigit(str[0]) || str[0] == '-')
                    {
                        bool neg = false;
                        if (str[0] == '-')
                        {
                            neg = true;
                            str = str.Substring(1);
                        }

                        if (str.IndexOf("0x") == 0)
                        {
                            string num = str.Substring(2);
                            inst.Immediate = 0;
                            for (int j = 0; j < num.Length; ++j)
                            {
                                inst.Immediate *= 16;
                                if (Char.IsDigit(num[j]) || num[j] == '-')
                                    inst.Immediate += num[j] - '0';
                                else
                                    inst.Immediate += num[j] - 'a' + 10;
                            }
                        }
                        else
                            inst.Immediate = Int32.Parse(str);

                        if (neg) inst.Immediate = -inst.Immediate;

                        inst.ImmediateLabel = null;
                    }
                    else
                    {
                        inst.ImmediateLabel = str;
                    }
                }
                ++formatIndex;
            }
        }
    }
}
