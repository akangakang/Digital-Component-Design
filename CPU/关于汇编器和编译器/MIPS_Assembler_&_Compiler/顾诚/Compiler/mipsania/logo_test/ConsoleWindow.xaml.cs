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
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        public ConsoleWindow()
        {
            InitializeComponent();
            Write("LogoScript Demo\n");
            Write("BY: ak48disk, BIUBIUBIU, 姜帆爱杨幂, 妹纸诚觅交大男 & bra_na_na\n");
        }

        StringBuilder m_sb = new StringBuilder();
        public void Write(string str)
        {
            m_sb.Append(str);
        }

        private System.Windows.Threading.DispatcherTimer m_timer;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_timer = new System.Windows.Threading.DispatcherTimer(
                System.Windows.Threading.DispatcherPriority.Send);
            m_timer.Tick += new EventHandler(TimeTick);
            m_timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            m_timer.Start();
        }

        private void TimeTick(object sender, EventArgs e)
        {
            if (m_sb.Length != 0)
            {
                ui_textBox.AppendText(m_sb.ToString());
                m_sb = new StringBuilder();
                ui_textBox.ScrollToEnd();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
