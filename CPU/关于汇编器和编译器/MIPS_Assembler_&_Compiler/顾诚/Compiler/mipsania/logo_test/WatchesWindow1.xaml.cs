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
using System.Threading;
using Interpreter;

namespace LogoScriptIDE
{
    /// <summary>
    /// Interaction logic for WatchesWindow.xaml
    /// </summary>
    public partial class WatchesWindow1 : Window
    {
        public class WatchesItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Remarks { get; set; }
        }

        public WatchesWindow1()
        {
            InitializeComponent();
            ui_watchesgrid.ItemsSource = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            m_watchesItem = new List<WatchesItem>();
            m_oldwatchesItem = new List<WatchesItem>();
            m_itemString = new List<String>();
            WatchesItem i = new WatchesItem();
            i.Name = "";
            i.Remarks = "<Not Exist>";
            m_watchesItem.Add(i);
            ui_watchesgrid.ItemsSource = m_watchesItem;
            m_Changed = 0;
            reviseValue = -1;
            enableButtons();
        }
        
        public void Update(MipsMachine m)
        {
            //if (m_watchesItem == null)
            //    return;
            //if (m != null &&
            //    (m.Status == MachineStatus.MACHINE_STATUS_PAUSED ||
            //    m.Status == MachineStatus.MACHINE_STATUS_DEAD))
            //{
            //    int j = 0;
            //    foreach (WatchesItem w in m_watchesItem)
            //    {
            //        if (m.CompileComplete && !string.IsNullOrEmpty(w.Name) && (m.IsGlobalExists(w.Name) || m.GetLocalIDByName(w.Name)!=-1))
            //        {
            //            Varible getV;
            //            int index = m.GetLocalIDByName(w.Name);
            //            if (index == -1)
            //                getV = m.GetGlobal(w.Name);
            //            else
            //                getV = m.GetLocalByID(index);
            //            // Revise Value
            //            if (reviseValue == j)
            //            {
            //                Varible update;
            //                switch (getV.type)
            //                {
            //                    case VarType.TYPE_NUMBER:
            //                        update = new Varible(Convert.ToDouble(w.Value));
            //                        break;
            //                    case VarType.TYPE_STRING:
            //                        update = new Varible(w.Value);
            //                        break;
            //                    default:
            //                        update = new Varible("<To Be Continued>");
            //                        break;
            //                }
            //                if (index == -1)
            //                    m.SetGlobal(w.Name, update);
            //                else
            //                    m.SetLocalByID(index, update);
            //                w.Remarks = "Revise value successfully";
            //                //MessageBox.Show("Revise value successfully");
            //                break;
            //            }
            //            else
            //            {
            //                w.Value = getV.ToString(m);
            //                w.Remarks = "Update successfully";
            //            }
            //        }
            //        else w.Remarks = "<Not Exist>";
            //        ++j;
            //    }
            //    ui_watchesgrid.ItemsSource = null;
            //    ui_watchesgrid.ItemsSource = m_watchesItem;
            //    if (reviseValue > -1) { 
            //        m_Changed = 1; reviseValue= -1;
            //    }
            //}
        }

        private string preValue="";
        public bool ableButtons;
        public int m_Changed, reviseValue;
        private List<WatchesItem> m_watchesItem, m_oldwatchesItem;
        private List<String> m_itemString;

        public void enableButtons()
        {
            ableButtons = true;
            ui_addLine.IsEnabled = true;
            ui_deleteLine.IsEnabled = true;
        }

        public void disableButtons()
        {
            ableButtons = false;
            ui_addLine.IsEnabled = false;
            ui_deleteLine.IsEnabled = false;
        }

        private void ui_addLine_Click(object sender, RoutedEventArgs e)
        {    
            WatchesItem i = new WatchesItem();
            i.Name = "";
            i.Value = "";
            i.Remarks = "<Not Exist>";
            m_watchesItem.Add(i);
            m_Changed = 1;
        }

        private void ui_deleteLine_Click(object sender, RoutedEventArgs e)
        {
            if (m_watchesItem.Count > 0)
            {
                m_watchesItem.RemoveAt((m_watchesItem.Count - 1));
                m_Changed = 1;
            }
        }

        private void ui_watchesgrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            // MessageBox.Show(e.Row.GetIndex().ToString());
            if (e.EditAction == DataGridEditAction.Commit)
            {
                grid.CellEditEnding -= ui_watchesgrid_CellEditEnding;
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                e.Cancel = true;
                // Judge if revise the value
                string newValue = (e.EditingElement as TextBox).Text;
                if (e.Column.DisplayIndex == 1 && newValue != preValue) // the index of "Value" Column is 1
                    reviseValue = e.Row.GetIndex();
                else
                    reviseValue = -1;

                m_Changed = 2;
                grid.CellEditEnding += ui_watchesgrid_CellEditEnding;
            }
        }

        private void ui_watchesgrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            preValue = (e.Column.GetCellContent(e.Row) as TextBlock).Text;  
        }

        // 
    }
}
