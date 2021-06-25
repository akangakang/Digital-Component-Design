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
    public partial class StackWindow : Window
    {
        //private static readonly int DefaultStringBuilderLength = 256;

        public class Stack
        {
            public int Register { get; set; }
            public string Hex { get; set; }
            public string Dec { get; set; }
        }

        public void Update(MipsMachine m)
        {
            if (m != null &&
                (m.Status == MachineStatus.MACHINE_STATUS_PAUSED ||
                m.Status == MachineStatus.MACHINE_STATUS_DEAD) &&
                m.CompileComplete)
                ui_grid.ItemsSource = GetStackSource(m);
            else
                ui_grid.ItemsSource = null;
        }

        private List<Stack> GetStackSource(MipsMachine m)
        {

            //List<StackInfo> L = m.GetCallStack();
            
            List<Stack> m_stack = new List<Stack>();
            for (int i = 0; i < m.Registers.Count; ++i)
            {
                Stack s = new Stack();
                s.Register = i;
                int num = m.Registers[i];
                s.Dec = num.ToString();
                s.Hex = num.ToString("X8");
                m_stack.Add(s);
            }
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

        public StackWindow()
        {
            InitializeComponent();
            Clear();
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

        private void ui_grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ui_grid.UnselectAll();
            ui_grid.UnselectAllCells();
        }
    }
}
