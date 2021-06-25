using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Microsoft.Win32;
using Interpreter;

namespace LogoScriptIDE
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Member Variables

        private EditorWindow m_editorWindow;
        //private CanvasWindow m_canvasWindow;
        private WatchesWindow m_watchesWindow;
        private StackWindow m_stackWindow;
        private ConsoleWindow m_consoleWindow;
        private MemWindow m_memWindow;
        //private Turtle m_turtle;
        private MipsMachine m_machine = new MipsMachine();
        public MipsMachine GetMachine()
        {
            return m_machine;
        }

        #endregion

        #region Initialization

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (StreamReader sr = new StreamReader(autoConfigPath))
                    m_auto.Load(sr);
            }
            catch { }

            createWindows();
            createFunctions();
            m_editorWindow.KeyFunctions.Clear();
            foreach (var pair in m_functions)
                m_editorWindow.KeyFunctions.Add(pair.Key);
            LoadAutoConfig();

            m_timer = new System.Windows.Threading.DispatcherTimer(
                System.Windows.Threading.DispatcherPriority.Send);
            m_timer.Tick += new EventHandler(TimerTick);
            m_timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            m_timer.Start();
        }

        private void createWindows()
        {
            //m_canvasWindow = new CanvasWindow();
            m_editorWindow = new EditorWindow();
            m_consoleWindow = new ConsoleWindow();
            m_stackWindow = new StackWindow();
            m_watchesWindow = new WatchesWindow();
            m_memWindow = new MemWindow();
        }

        private Dictionary<string, IFunction> m_functions
            = new Dictionary<string,IFunction>();
        private void createFunctions()
        {
            m_functions.Clear();
            m_functions.Add("PRINT", new FuncPrint(m_consoleWindow));

            //m_functions.Add("FD", new FuncFD(m_canvasWindow.Turtle));
            //m_functions.Add("BK", new FuncBK(m_canvasWindow.Turtle));

            //m_functions.Add("LT", new FuncLT(m_canvasWindow.Turtle));
            //m_functions.Add("RT", new FuncRT(m_canvasWindow.Turtle));
            //m_functions.Add("RESETA", new FuncRESETA(m_canvasWindow.Turtle));

            //m_functions.Add("PU", new FuncPU(m_canvasWindow.Turtle));
            //m_functions.Add("PD", new FuncPD(m_canvasWindow.Turtle));

            //m_functions.Add("RESET", new FuncRESET(m_canvasWindow.Turtle));

            //m_functions.Add("SETXY", new FuncSETXY(m_canvasWindow.Turtle));
            //m_functions.Add("SETSIZE", new FuncSETSIZE(m_canvasWindow.Turtle));

            //m_functions.Add("SETPC", new FuncSETPC(m_canvasWindow.Turtle));
            //m_functions.Add("SETPW", new FuncSETPW(m_canvasWindow.Turtle));

            //m_functions.Add("INPUT", new FuncINPUT(m_canvasWindow.Turtle,this));

            //m_functions.Add("HIDE", new FuncHIDE(m_canvasWindow.Turtle));
            //m_functions.Add("SHOW", new FuncSHOW(m_canvasWindow.Turtle));
            //m_functions.Add("DELAY", new FuncDELAY());

            //m_functions.Add("UPDATE", new FuncUPDATE(m_canvasWindow.Turtle));
        }
        
        private void registerFunctions(MipsMachine m)
        {
            //foreach (var pair in m_functions)
            //{
            //    m.SetGlobal(pair.Key, new Varible(pair.Value));
            //}
        }

        #endregion

        #region Timer
        private MachineStatus m_lastState = MachineStatus.MACHINE_STATUS_FINISHED;
        private bool m_stateChanged = true;
        private System.Windows.Threading.DispatcherTimer m_timer;
        //private int times = 0;
        //private bool start = ;

        private static readonly double EnableDefaultOpacity = 1;
        private static readonly double DisableDefaultOpacity = 0.3;
        private void EnableTool(Button b,MenuItem m)
        {
            if (b != null)
            {
                b.IsEnabled = true;
                b.Opacity = EnableDefaultOpacity;
            }
            if (m != null) m.IsEnabled = true;
        }
        private void DisableTool(Button b, MenuItem m)
        {
            if (b != null)
            {
                b.IsEnabled = false;
                b.Opacity = DisableDefaultOpacity;
            }
            if (m != null) m.IsEnabled = false;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            MachineStatus newState = m_machine.Status;
            
            /*if (m_machine.CompileComplete)
            {
                if (!m_watchesWindow.ableButtons) m_watchesWindow.enableButtons(); 
            }
            else if (m_watchesWindow.ableButtons) m_watchesWindow.disableButtons();*/
            
            if (newState != m_lastState || m_stateChanged )
            {
                m_lastState = newState;
                m_stateChanged = false;

                m_stackWindow.Update(m_machine);
                m_watchesWindow.Update(m_machine);

                //m_watchesWindow.m_Changed = 3;
                switch (newState)
                {
                    case MachineStatus.MACHINE_STATUS_RUNNING:
                        DisableTool(ui_toolNew, ui_mnuNew);
                        DisableTool(ui_toolOpen,ui_mnuOpen);
                        DisableTool(ui_toolSave,ui_mnuSave);
                        DisableTool(null, ui_mnuSaveAs);
                        DisableTool(ui_toolUndo, ui_mnuUndo);
                        DisableTool(ui_toolRedo, ui_mnuRedo);
                        DisableTool(ui_toolRun, ui_mnuRun);
                        DisableTool(ui_toolStepIn, ui_mnuStepin);
                        //DisableTool(ui_toolStepOut, ui_mnuStepout);
                        DisableTool(ui_toolStepOver,ui_mnuStepover);
                        EnableTool(ui_toolPause,ui_mnuPause);
                        EnableTool(ui_toolStop,ui_mnuStop);
                        EnableTool(ui_toolExit,ui_mnuExit);

                        m_editorWindow.PauseLine = -1;
                        m_editorWindow.ErrorLine = -1;
                        m_editorWindow.Locked = true;
                        break;
                    case MachineStatus.MACHINE_STATUS_PAUSED:
                        if (m_machine.CompileComplete)
                        {
                            DisableTool(ui_toolNew, ui_mnuNew);
                            DisableTool(ui_toolOpen, ui_mnuOpen);
                            DisableTool(ui_toolSave, ui_mnuSave);
                            DisableTool(null, ui_mnuSaveAs);
                            DisableTool(ui_toolUndo, ui_mnuUndo);
                            DisableTool(ui_toolRedo, ui_mnuRedo);
                            EnableTool(ui_toolRun, ui_mnuRun);
                            DisableTool(ui_toolPause,ui_mnuPause);
                            EnableTool(ui_toolStop,ui_mnuStop);
                            EnableTool(ui_toolStepIn, ui_mnuStepin);
                            //EnableTool(ui_toolStepOut, ui_mnuStepout);
                            EnableTool(ui_toolStepOver,ui_mnuStepover);
                            EnableTool(ui_toolExit,ui_mnuExit);

                            m_editorWindow.PauseLine = m_machine.CurrentLine;
                            m_editorWindow.ui_editor.ScrollToLine(m_editorWindow.PauseLine);
                            m_editorWindow.Locked = true;
                        }
                        else
                        {
                            EnableTool(ui_toolNew, ui_mnuNew);
                            EnableTool(ui_toolOpen, ui_mnuOpen);
                            EnableTool(ui_toolSave, ui_mnuSave);
                            EnableTool(null, ui_mnuSaveAs);
                            EnableTool(ui_toolUndo, ui_mnuUndo);
                            EnableTool(ui_toolRedo, ui_mnuRedo);
                            EnableTool(ui_toolRun, ui_mnuRun);
                            DisableTool(ui_toolPause,ui_mnuPause);
                            DisableTool(ui_toolStop,ui_mnuStop);
                            EnableTool(ui_toolStepIn, ui_mnuStepin);
                            //EnableTool(ui_toolStepOut, ui_mnuStepout);
                            EnableTool(ui_toolStepOver,ui_mnuStepover);
                            EnableTool(ui_toolExit,ui_mnuExit); 

                            //m_editorWindow.PauseLine = -1;
                            //m_editorWindow.ErrorLine = -1;
                            m_editorWindow.Locked = false;
                        }
                        break;
                    case MachineStatus.MACHINE_STATUS_FINISHED:
                        EnableTool(ui_toolNew, ui_mnuNew);
                        EnableTool(ui_toolOpen, ui_mnuOpen);
                        EnableTool(ui_toolSave, ui_mnuSave);
                        EnableTool(null, ui_mnuSaveAs);
                        EnableTool(ui_toolUndo, ui_mnuUndo);
                        EnableTool(ui_toolRedo, ui_mnuRedo);
                        EnableTool(ui_toolRun, ui_mnuRun);
                        DisableTool(ui_toolPause,ui_mnuPause);
                        DisableTool(ui_toolStop,ui_mnuStop);
                        EnableTool(ui_toolStepIn, ui_mnuStepin);
                        //EnableTool(ui_toolStepOut, ui_mnuStepout);
                        EnableTool(ui_toolStepOver,ui_mnuStepover);
                        EnableTool(ui_toolExit,ui_mnuExit); 

                        m_editorWindow.PauseLine = -1;
                        m_editorWindow.ErrorLine = -1;
                        m_editorWindow.Locked = false;
                        break;
                    case MachineStatus.MACHINE_STATUS_DEAD:
                        DisableTool(ui_toolNew, ui_mnuNew);
                        DisableTool(ui_toolOpen, ui_mnuOpen);
                        DisableTool(ui_toolSave, ui_mnuSave);
                        DisableTool(ui_toolRun, ui_mnuRun);
                        DisableTool(ui_toolPause,ui_mnuPause);
                        DisableTool(null, ui_mnuSaveAs);
                        DisableTool(ui_toolUndo, ui_mnuUndo);
                        DisableTool(ui_toolRedo, ui_mnuRedo);
                        EnableTool(ui_toolStop,ui_mnuStop);
                        DisableTool(ui_toolStepIn, ui_mnuStepin);
                        //DisableTool(ui_toolStepOut, ui_mnuStepout);
                        DisableTool(ui_toolStepOver,ui_mnuStepover);
                        EnableTool(ui_toolExit,ui_mnuExit); 

                        string str = "Runtime Error: " + m_machine.LastErrorMessage + "\n";
                        m_consoleWindow.Write(str);
                        m_consoleWindow.Show();
                        m_editorWindow.ErrorLine = m_machine.CurrentLine;
                        m_editorWindow.ui_editor.ScrollToLine(m_editorWindow.ErrorLine);
                        m_editorWindow.Locked = true;
                        break;
                    default:
                        throw new Exception("Unexpected machine status detected.");
                }
            }
            //if (m_watchesWindow.m_Changed > 0)
            //{
            //    m_watchesWindow.m_Changed = 0;
            //    m_watchesWindow.Update(m_machine);
            //}
              

            ui_mnuViewWatches.IsChecked = m_watchesWindow.IsVisible;
            ui_mnuViewStack.IsChecked = m_stackWindow.IsVisible;
            ui_mnuViewEditor.IsChecked = m_editorWindow.IsVisible;
            ui_mnuViewConsole.IsChecked = m_consoleWindow.IsVisible;
            //ui_mnuViewCanvas.IsChecked = m_canvasWindow.IsVisible;
            //ui_mnuViewMem.IsChecked = m_memWindow.IsVisible;
            if (m_editorWindow.ui_editor.CanUndo == false) DisableTool(ui_toolUndo, ui_mnuUndo); else if (!m_editorWindow.Locked) EnableTool(ui_toolUndo, ui_mnuUndo);
            if (m_editorWindow.ui_editor.CanRedo == false) DisableTool(ui_toolRedo, ui_mnuRedo); else if (!m_editorWindow.Locked) EnableTool(ui_toolRedo, ui_mnuRedo);
        }
        #endregion

        #region File Commands

        public static RoutedCommand CommandOpen = new RoutedCommand();
        public static RoutedCommand CommandSave = new RoutedCommand();
        public static RoutedCommand CommandNew = new RoutedCommand();
        public static RoutedCommand CommandUndo = new RoutedCommand();
        public static RoutedCommand CommandRedo = new RoutedCommand();

        private void mnuNew_Click(object sender, RoutedEventArgs e)
        {
            m_editorWindow.New();
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e)
        {
            m_editorWindow.Open();
        }

        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            m_editorWindow.Save();
        }

        private void mnuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            m_editorWindow.SaveAs();
        }

        #endregion

        #region Edit Commands

        private void mnuUndo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m_editorWindow.Undo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void mnuRedo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m_editorWindow.Redo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void mnuCut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m_editorWindow.Cut();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m_editorWindow.Copy();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void mnvPaste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m_editorWindow.Paste();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region Run & Debug Commands

        private string Compile(MipsMachine m)
        {
            try
            {
                m.Compile(new StringStream(m_editorWindow.Code));
            }
            catch (CompileException e)
            {
                string str = "Line " + e.Line.ToString() + ": " + e.Description;
                m_editorWindow.ErrorLine = e.Line;
                return str;
            }
            catch (Exception e)
            {
                return "奇妙的错误发生了！" + e.Message;
            }
            return "";
        }

        private void ui_mnuCheckSyntax_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = Compile(new MipsMachine());
            if (errorMessage != "")
                m_consoleWindow.Write("Syntax Check Failed: " + errorMessage + "\n");
            else
                m_consoleWindow.Write("Syntax Check Passed.\n");
            m_consoleWindow.Show();
        }

        private void SyncBreakPoints()
        {
            var machineBreakpointDict = m_machine.GetBreakpointList();
            List<int> machineBreakpointList = new List<int>();
            foreach (var breakpoint in machineBreakpointDict)
                machineBreakpointList.Add(breakpoint.Key);
            foreach (var breakpoint in machineBreakpointList)
                m_machine.RemoveBreakpoint(breakpoint);

            var breakpoints = m_editorWindow.BreakPoints;
            foreach (var breakpoint in breakpoints)
                m_machine.AddBreakpoint(breakpoint, true);

            HashSet<int> newBreakpoints = new HashSet<int>();
            var addedBreakpoints = m_machine.GetBreakpointList();
            foreach (var breakpoint in addedBreakpoints)
                newBreakpoints.Add(breakpoint.Key);
            m_editorWindow.BreakPoints = newBreakpoints;
        }

        private bool PrepareForExecution()
        {
            if (m_machine != null &&
                m_machine.Status == MachineStatus.MACHINE_STATUS_RUNNING)
                m_machine.Pause();
            m_machine = new MipsMachine();
            registerFunctions(m_machine);

            string errorMessage = Compile(m_machine);
            if (errorMessage != "")
            {
                m_consoleWindow.Write("Compile Failed: " + errorMessage + "\n");
                m_consoleWindow.Write("Execution Aborted.\n");
                m_consoleWindow.Show();
                return false;
            }
            else
            {
                m_consoleWindow.Write("Compile Successfully.\n");
            }

            //m_canvasWindow.Turtle.ResetAll();
            return true;
        }

        private void CompileAndExecute()
        {
            if (!PrepareForExecution()) return;
            SyncBreakPoints();
            m_machine.Execute();
            m_stateChanged = true;
        }

        private void mnuStop_Click(object sender, RoutedEventArgs e)
        {
            m_stateChanged = true;
            if (m_machine.Status == MachineStatus.MACHINE_STATUS_RUNNING)
                m_machine.Pause();
            m_machine = new MipsMachine();
            m_stateChanged = true;
            m_editorWindow.ErrorLine = -1;
            m_editorWindow.PauseLine = -1;
        }

        private void mnuRun_Click(object sender, RoutedEventArgs e)
        {
            if (!m_editorWindow.CheckModifiedCode(false) ||
                m_editorWindow.Modified)
                return;

            if (m_machine.Status == MachineStatus.MACHINE_STATUS_PAUSED &&
                m_machine.CompileComplete == true)
            {
                SyncBreakPoints();
                m_machine.Execute();
            }
            else
                CompileAndExecute();
            m_stateChanged = true;
        }

        private void mnuPause_Click(object sender, RoutedEventArgs e)
        {
            m_machine.Pause();
        }

        private bool PrepareForDebug()
        {
            if (!m_editorWindow.CheckModifiedCode(false) ||
                m_editorWindow.Modified)
                return false;

            if (m_machine.Status != MachineStatus.MACHINE_STATUS_PAUSED ||
                !m_machine.CompileComplete)
            {
                if (!PrepareForExecution())
                    return false;
            }
            SyncBreakPoints();
            return true;
        }

        private void mnuStepIn_Click(object sender, RoutedEventArgs e)
        {
            if (!PrepareForDebug()) return;
            m_machine.StepIn();
            m_stateChanged = true;
        }

        private void mnuStepOut_Click(object sender, RoutedEventArgs e)
        {
            if (!PrepareForDebug()) return;
            m_machine.StepOut();
            m_stateChanged = true;
        }

        private void mnuStepOver_Click(object sender, RoutedEventArgs e)
        {
            if (!PrepareForDebug()) return;
            m_machine.StepOver();
            m_stateChanged = true;
        }

        #endregion

        #region View Commands

        private void mnuViewEditor_Click(object sendre, RoutedEventArgs e)
        {
            if (ui_mnuViewEditor.IsChecked)
            {
                m_editorWindow.Close();
            }
            else
            {
                m_editorWindow.Show();
            }
        }

        private void mnuViewWatches_Click(object sender, RoutedEventArgs e)
        {
            if (ui_mnuViewWatches.IsChecked)
            {
                m_watchesWindow.Close();
            }
            else
            {
                m_watchesWindow.Show();
            }
        }

        private void mnuViewStack_Click(object sender, RoutedEventArgs e)
        {
            if (ui_mnuViewStack.IsChecked)
            {
                m_stackWindow.Close();
            }
            else
            {
                m_stackWindow.Show();
            }
        }

        private void mnuViewConsole_Click(object sender, RoutedEventArgs e)
        {
            if (ui_mnuViewConsole.IsChecked)
            {
                m_consoleWindow.Close();
            }
            else
            {
                m_consoleWindow.Show();
            }
        }

        #endregion

        #region Events

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveAutoConfig();
            if (m_editorWindow.Modified)
            {
                if (!m_editorWindow.CheckModifiedCode(true))
                {
                    e.Cancel = true;
                    return;
                }
            }
            m_timer.Stop();
            if (m_machine != null &&
                m_machine.Status == MachineStatus.MACHINE_STATUS_RUNNING)
            {
                m_machine.Pause();
            }
        }

        #endregion

        #region Auto Config

        private Config m_auto = new Config();
        static private readonly string autoConfigPath = "auto_config.cfg";

        private static readonly string cfg_MainWindow_Top = "MainWindow_Top";
        private static readonly string cfg_MainWindow_Left = "MainWindow_Left";
        private static readonly string cfg_MainWindow_Width = "MainWindow_Width";
        private static readonly string cfg_MainWindow_Height = "MainWindow_Height";

        private static readonly string cfg_Editor_Top = "Editor_Top";
        private static readonly string cfg_Editor_Left = "Editor_Left";
        private static readonly string cfg_Editor_Width = "Editor_Width";
        private static readonly string cfg_Editor_Height = "Editor_Height";
        private static readonly string cfg_Editor_Show = "Editor_Show";
        private static readonly string cfg_Editor_FilePath = "Editor_FilePath";
        private static readonly string cfg_Editor_FileName = "Editor_FileName";

        private static readonly string cfg_Canvas_Top = "Canvas_Top";
        private static readonly string cfg_Canvas_Left = "Canvas_Left";
        private static readonly string cfg_Canvas_Width = "Canvas_Width";
        private static readonly string cfg_Canvas_Height = "Canvas_Height";
        private static readonly string cfg_Canvas_Show = "Canvas_Show";

        private static readonly string cfg_Console_Top = "Console_Top";
        private static readonly string cfg_Console_Left = "Console_Left";
        private static readonly string cfg_Console_Width = "Console_Width";
        private static readonly string cfg_Console_Height = "Console_Height";
        private static readonly string cfg_Console_Show = "Console_Show";

        private static readonly string cfg_Watches_Top = "Watches_Top";
        private static readonly string cfg_Watches_Left = "Watches_Left";
        private static readonly string cfg_Watches_Width = "Watches_Width";
        private static readonly string cfg_Watches_Height = "Watches_Height";
        private static readonly string cfg_Watches_Show = "Watches_Show";

        private static readonly string cfg_Stack_Top = "Stack_Top";
        private static readonly string cfg_Stack_Left = "Stack_Left";
        private static readonly string cfg_Stack_Width = "Stack_Width";
        private static readonly string cfg_Stack_Height = "Stack_Height";
        private static readonly string cfg_Stack_Show = "Stack_Show";

        private static readonly string cfg_Mem_Top = "Mem_Top";
        private static readonly string cfg_Mem_Left = "Mem_Left";
        private static readonly string cfg_Mem_Width = "Mem_Width";
        private static readonly string cfg_Mem_Height = "Mem_Height";
        private static readonly string cfg_Mem_Show = "Mem_Show";

        private void SaveAutoConfig()
        {
            m_auto.SetField(cfg_MainWindow_Top, Top.ToString());
            m_auto.SetField(cfg_MainWindow_Left, Left.ToString());
            m_auto.SetField(cfg_MainWindow_Width, Width.ToString());
            m_auto.SetField(cfg_MainWindow_Height, Height.ToString());

            m_auto.SetField(cfg_Editor_Top, m_editorWindow.Top.ToString());
            m_auto.SetField(cfg_Editor_Left, m_editorWindow.Left.ToString());
            m_auto.SetField(cfg_Editor_Width, m_editorWindow.Width.ToString());
            m_auto.SetField(cfg_Editor_Height, m_editorWindow.Height.ToString());
            m_auto.SetField(cfg_Editor_Show, m_editorWindow.IsVisible.ToString().ToLower());
            m_auto.SetField(cfg_Editor_FilePath, m_editorWindow.FilePath);
            m_auto.SetField(cfg_Editor_FileName, m_editorWindow.FileName);

            //m_auto.SetField(cfg_Canvas_Top, m_canvasWindow.Top.ToString());
            //m_auto.SetField(cfg_Canvas_Left, m_canvasWindow.Left.ToString());
            //m_auto.SetField(cfg_Canvas_Width, m_canvasWindow.Width.ToString());
            //m_auto.SetField(cfg_Canvas_Height, m_canvasWindow.Height.ToString());
            //m_auto.SetField(cfg_Canvas_Show, m_canvasWindow.IsVisible.ToString().ToLower());

            m_auto.SetField(cfg_Console_Top, m_consoleWindow.Top.ToString());
            m_auto.SetField(cfg_Console_Left, m_consoleWindow.Left.ToString());
            m_auto.SetField(cfg_Console_Width, m_consoleWindow.Width.ToString());
            m_auto.SetField(cfg_Console_Height, m_consoleWindow.Height.ToString());
            m_auto.SetField(cfg_Console_Show, m_consoleWindow.IsVisible.ToString().ToLower());

            m_auto.SetField(cfg_Watches_Top, m_watchesWindow.Top.ToString());
            m_auto.SetField(cfg_Watches_Left, m_watchesWindow.Left.ToString());
            m_auto.SetField(cfg_Watches_Width, m_watchesWindow.Width.ToString());
            m_auto.SetField(cfg_Watches_Height, m_watchesWindow.Height.ToString());
            m_auto.SetField(cfg_Watches_Show, m_watchesWindow.IsVisible.ToString().ToLower());

            m_auto.SetField(cfg_Stack_Top, m_stackWindow.Top.ToString());
            m_auto.SetField(cfg_Stack_Left, m_stackWindow.Left.ToString());
            m_auto.SetField(cfg_Stack_Width, m_stackWindow.Width.ToString());
            m_auto.SetField(cfg_Stack_Height, m_stackWindow.Height.ToString());
            m_auto.SetField(cfg_Stack_Show, m_stackWindow.IsVisible.ToString().ToLower());

            m_auto.SetField(cfg_Mem_Top, m_memWindow.Top.ToString());
            m_auto.SetField(cfg_Mem_Left, m_memWindow.Left.ToString());
            m_auto.SetField(cfg_Mem_Width, m_memWindow.Width.ToString());
            m_auto.SetField(cfg_Mem_Height, m_memWindow.Height.ToString());
            m_auto.SetField(cfg_Mem_Show, m_memWindow.IsVisible.ToString().ToLower());

            try
            {
                using (StreamWriter sw = new StreamWriter(autoConfigPath))
                    m_auto.Save(sw);
            }
            catch { }
        }

        private void LoadAutoConfig()
        {
            Top = m_auto.TryGetDouble(cfg_MainWindow_Top, Top);
            Left = m_auto.TryGetDouble(cfg_MainWindow_Left, Left);
            Width = m_auto.TryGetDouble(cfg_MainWindow_Width, Width);
            Height = m_auto.TryGetDouble(cfg_MainWindow_Height, Height);

            m_editorWindow.Owner = this;
            m_editorWindow.Top = m_auto.TryGetDouble(cfg_Editor_Top, m_editorWindow.Top);
            m_editorWindow.Left = m_auto.TryGetDouble(cfg_Editor_Left, m_editorWindow.Left);
            m_editorWindow.Width = m_auto.TryGetDouble(cfg_Editor_Width, m_editorWindow.Width);
            m_editorWindow.Height = m_auto.TryGetDouble(cfg_Editor_Height, m_editorWindow.Height);
            m_editorWindow.InitialPath = m_auto.GetField(cfg_Editor_FilePath);
            m_editorWindow.InitialName = m_auto.GetField(cfg_Editor_FileName);

            //m_canvasWindow.Owner = this;
            //m_canvasWindow.Top = m_auto.TryGetDouble(cfg_Canvas_Top, m_canvasWindow.Top);
            //m_canvasWindow.Left = m_auto.TryGetDouble(cfg_Canvas_Left, m_canvasWindow.Left);
            //m_canvasWindow.Width = m_auto.TryGetDouble(cfg_Canvas_Width, m_canvasWindow.Width);
            //m_canvasWindow.Height = m_auto.TryGetDouble(cfg_Canvas_Height, m_canvasWindow.Height);
            //m_turtle = m_canvasWindow.Turtle;

            m_watchesWindow.Owner = this;
            m_watchesWindow.Top = m_auto.TryGetDouble(cfg_Watches_Top, m_watchesWindow.Top);
            m_watchesWindow.Left = m_auto.TryGetDouble(cfg_Watches_Left, m_watchesWindow.Left);
            m_watchesWindow.Width = m_auto.TryGetDouble(cfg_Watches_Width, m_watchesWindow.Width);
            m_watchesWindow.Height = m_auto.TryGetDouble(cfg_Watches_Height, m_watchesWindow.Height);

            m_stackWindow.Owner = this;
            m_stackWindow.Top = m_auto.TryGetDouble(cfg_Stack_Top, m_stackWindow.Top);
            m_stackWindow.Left = m_auto.TryGetDouble(cfg_Stack_Left, m_stackWindow.Left);
            m_stackWindow.Width = m_auto.TryGetDouble(cfg_Stack_Width, m_stackWindow.Width);
            m_stackWindow.Height = m_auto.TryGetDouble(cfg_Stack_Height, m_stackWindow.Height);

            m_consoleWindow.Owner = this;
            m_consoleWindow.Top = m_auto.TryGetDouble(cfg_Console_Top, m_consoleWindow.Top);
            m_consoleWindow.Left = m_auto.TryGetDouble(cfg_Console_Left, m_consoleWindow.Left);
            m_consoleWindow.Width = m_auto.TryGetDouble(cfg_Console_Width, m_consoleWindow.Width);
            m_consoleWindow.Height = m_auto.TryGetDouble(cfg_Console_Height, m_consoleWindow.Height);

            m_memWindow.Owner = this;
            m_memWindow.Top = m_auto.TryGetDouble(cfg_Mem_Top, m_memWindow.Top);
            m_memWindow.Left = m_auto.TryGetDouble(cfg_Mem_Left, m_memWindow.Left);
            m_memWindow.Width = m_auto.TryGetDouble(cfg_Mem_Width, m_memWindow.Width);
            m_memWindow.Height = m_auto.TryGetDouble(cfg_Mem_Height, m_memWindow.Height);

            if (m_auto.GetField(cfg_Editor_Show).ToLower() != "false")
                m_editorWindow.Show();
            //if (m_auto.GetField(cfg_Canvas_Show).ToLower() != "false")
                //m_canvasWindow.Show();
            if (m_auto.GetField(cfg_Console_Show).ToLower() != "false")
                m_consoleWindow.Show();
            if (m_auto.GetField(cfg_Stack_Show).ToLower() != "false")
                m_stackWindow.Show();
            if (m_auto.GetField(cfg_Watches_Show).ToLower() != "false")
                m_watchesWindow.Show();  
        }

        #endregion

        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("求不蠢");
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mnuViewMem_Click(object sender, RoutedEventArgs e)
        {
            //if (ui_mnuViewMem.IsChecked)
            //{
            //    m_memWindow.Close();
            //}
            //else
            //{
            //    m_memWindow.Show();
            //}
        }
    }
}
