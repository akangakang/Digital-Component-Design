using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Interpreter;

namespace LogoScriptIDE
{
    /// <summary>
    /// Interaction logic for StackWindow.xaml
    /// </summary>
    public partial class MemWindow : Window
    {
        //private static readonly int DefaultStringBuilderLength = 256;

        public class Stack
        {
            public string Address { get; set; }
            public string Hex { get; set; }
            public string Dec { get; set; }
        }

        public void Update()
        {
            ui_grid.ItemsSource = GetStackSource();
        }

        private List<Stack> GetStackSource()
        {

            //List<StackInfo> L = m.GetCallStack();
            
            List<Stack> m_stack = new List<Stack>();
            //for (int i = 0; i < m.Registers.Count; ++i)
            //{
            //    Stack s = new Stack();
            //    s.Register = i;
            //    int num = m.Registers[i];
            //    s.Dec = num.ToString();
            //    s.Hex = num.ToString("X8");
            //    m_stack.Add(s);
            //}
            //int depth = L.Count;
            //foreach (StackInfo s in L)
            //{
            //    StringBuilder sb = new StringBuilder(DefaultStringBuilderLength);
            //    sb.Append(s.funcName);
            //    sb.Append('(');

            //    if (s.parmList.Count > 0 && s.parmList != null)
            //    {
            //        for (int i = 0; i < s.parmList.Count; i++)
            //        {
            //            if (i > 0) sb.Append(", ");
            //            /*if (s.parmList[i].type == VarType.TYPE_NUMBER)
            //            {
            //                sb.Append(s.parmList[i].num);
            //            }
            //            else if (s.parmList[i].type == VarType.TYPE_STRING)
            //            {
            //                sb.Append('\"');
            //                sb.Append(
            //                    m.GetString(s.parmList[i].pointer)
            //                    .Replace("\n", "\\n")
            //                    .Replace("\r", "\\r")
            //                    .Replace("\b", "\\b")
            //                    .Replace("\t", "\\t")
            //                    .Replace("\"", "\\\"")
            //                    .Replace("\\", "\\\\"));
            //                sb.Append('\"');
            //            }
            //            else
            //            {
            //                sb.Append("[Function Pointer]");
            //            }*/
            //            sb.Append(s.parmList[i].ToString(m));
            //        }
            //    }
                
            //    sb.Append(')');

            //    Stack stack = new Stack();
            //    stack.Function = sb.ToString();
            //    stack.Line = s.line;
            //    stack.Depth = depth;
            //    --depth;
            //    m_stack.Add(stack);
            //}
            return m_stack;
        }

        public MemWindow()
        {
            InitializeComponent();
            Clear();
            Update();
        }

        private void Clear()
        {
            ui_grid.ItemsSource = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ui_grid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public void Save()
        {
        }

        public Dictionary<uint, uint> GetDictionay()
        {
            Dictionary<uint, uint> result = new Dictionary<uint, uint>();
            foreach (var item in ui_grid.ItemsSource)
            {
                Stack s = item as Stack;
                result.Add(FromString(s.Address), FromString(s.Hex));
            }
            return result;
        }

        private uint FromString(string s)
        {
            s = s.ToLower();
            if (s[1] == 'x') s = s.Substring(2);
            uint result = 0;
            for (int i = 0; i < s.Length; ++i)
            {
                result *= 16;
                if (char.IsDigit(s[i]))
                    result += (uint)(s[i] - '0');
                else
                    result += (uint)(s[i] - 'a' + 10);
            }
            return result;
        }


        private string ToString(uint n)
        {
            return n.ToString();
        }
    }
}
