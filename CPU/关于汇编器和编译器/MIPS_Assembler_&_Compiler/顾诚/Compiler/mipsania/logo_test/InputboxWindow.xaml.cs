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

namespace LogoScriptIDE
{
    /// <summary>
    /// Interaction logic for InputboxWindow.xaml
    /// </summary>
    public partial class InputboxWindow : Window
    {
        public InputboxWindow()
        {
            InitializeComponent();
        }

        public double getNum()
        {
            return inputNum;
        }

        public int inputStatus()
        {
            return inputDone;
        }

        private void ui_inputDone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inputNum = Double.Parse(Convert.ToString(ui_inputText.Text));
            }
            catch
            {
                inputDone = -1;
                Hide();
                return;
            }
            inputDone = 1;
            Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
        private double inputNum;
        private int inputDone; // -1: something wrong   0: not done     1: done

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            inputDone = 0;
            inputNum = 0;
        }
    }
}
