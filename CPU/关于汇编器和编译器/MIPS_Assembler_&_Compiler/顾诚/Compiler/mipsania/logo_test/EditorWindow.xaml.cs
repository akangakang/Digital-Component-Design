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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml;

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;

namespace LogoScriptIDE
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditorWindow : Window
    {
        #region Members and Attributes

        private readonly static string InitialCode
            = "function main() {" + Environment.NewLine
            + "\t`write your code here" + Environment.NewLine + "}" + Environment.NewLine;

        private CaretLineRenderer m_caretLineRenderer;
        private CodeLineRenderer m_codeLineRenderer;
        private BreakPointMargin m_breakPointMargin;

        public String Code
        {
            get
            {
                Show();
                return ui_editor.Text; 
            }
            set { ui_editor.Text = value; }
        }

        bool m_modified = false;
        public bool Modified
        {
            get { return m_modified; }
            private set
            {
                if (m_modified != value)
                {
                    m_modified = value;
                    RefreshTitle();
                }
            }
        }

        public bool Locked
        {
            get { return ui_editor.IsReadOnly; }
            set { ui_editor.IsReadOnly = value; }
        }

        private Color m_pauseColor = Color.FromArgb(0x40, 0xFF, 0xFF, 0);
        public int PauseLine
        {
            get
            {
                return m_breakPointMargin.PausingLineNumber;
            }
            set
            {
                if (!m_initialized) return;
                if (value != -1) m_errorLine = -1;
                m_breakPointMargin.PausingLineNumber = value;
                m_codeLineRenderer.RenderSettings.Clear();
                if (value > 0)
                {
                    var setting = new CodeLineRenderer.Setting(value, m_pauseColor);
                    m_codeLineRenderer.RenderSettings.Add(setting);
                }
                ui_editor.TextArea.TextView.InvalidateLayer(m_codeLineRenderer.Layer);
                if (value != -1) Show();
            }
        }

        private int m_errorLine = -1;
        private Color m_errorColor = Color.FromArgb(0x40, 0xFF, 0, 0);
        public int ErrorLine
        {
            get { return m_errorLine; }
            set
            {
                if (!m_initialized) return;
                if (value != -1) m_breakPointMargin.PausingLineNumber = -1;
                m_errorLine = value;
                m_codeLineRenderer.RenderSettings.Clear();
                if (value > 0)
                {
                    var setting = new CodeLineRenderer.Setting(value, m_errorColor);
                    m_codeLineRenderer.RenderSettings.Add(setting);
                }
                ui_editor.TextArea.TextView.InvalidateLayer(m_codeLineRenderer.Layer);
                if (value != -1) Show();
            }
        }

        public ICollection<int> BreakPoints
        {
            get
            {
                List<int> lineNumbers = new List<int>();
                m_breakPointMargin.GetBreakPoints(lineNumbers);
                return lineNumbers;
            }
            set
            {
                if (value == null)
                    m_breakPointMargin.ClearBreakPoints();
                else
                    m_breakPointMargin.SetBreakPoints(value);
            }
        }

        private string m_fileName = "Unnamed";
        public string FileName
        {
            get { return m_fileName; }
            private set
            {
                m_fileName = value; 
                RefreshTitle();
            }
        }
        private string m_filePath = "";
        public string FilePath
        {
            get { return m_filePath; }
            private set { m_filePath = value; }
        }

        #endregion

        #region Initialization

        public EditorWindow()
        {
            InitializeComponent();
        }

        private bool m_initialized = false;

        public string InitialPath { get; set; }
        public string InitialName { get; set; }

        static private readonly string ConfigPath = "editor_config.cfg";
        private HashSet<string> m_keyfunctions = new HashSet<string>();
        public HashSet<string> KeyFunctions
        {
            get { return m_keyfunctions; }
            set { m_keyfunctions = value; }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                m_breakPointMargin = new BreakPointMargin(ui_editor);
                ui_editor.TextArea.LeftMargins.Insert(0, m_breakPointMargin);

                m_codeLineRenderer = new CodeLineRenderer(ui_editor);
                ui_editor.TextArea.TextView.BackgroundRenderers.Add(m_codeLineRenderer);

                m_caretLineRenderer = new CaretLineRenderer(ui_editor);
                ui_editor.TextArea.TextView.BackgroundRenderers.Add(m_caretLineRenderer);
                ui_editor.Text = InitialCode;

                try
                {
                    using (StreamReader sr = new StreamReader(ConfigPath))
                        Configure.Load(sr);
                }
                catch { Configure = new Config(); }
                //Configure.KeyFunctions = KeyFunctions;
                
                UpdateConfig();

                Modified = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            m_initialized = true;

            try
            {
                FilePath = InitialPath;
                FileName = InitialName;
                ui_editor.Load(FilePath);
            }
            catch
            {
                FilePath = "";
                FileName = "Unnamed";
                Code = InitialCode;
            }
            Modified = false;
        }

        #endregion

        #region Edit Commands

        public void Undo()
        {
            ui_editor.Undo();
        }

        public void Redo()
        {
            ui_editor.Redo();
        }

        public void Cut()
        {
            if (ui_editor.SelectionLength > 0)
                ui_editor.Cut();
        }

        public void Copy()
        {
            if (ui_editor.SelectionLength > 0)
                ui_editor.Copy();
        }

        public void Paste()
        {
            ui_editor.Paste();
        }

        #endregion

        #region Events

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ui_editor_TextChanged(object sender, EventArgs e)
        {
            Modified = true;
            ErrorLine = -1;
        }

        #endregion

        #region File Commands

        public static RoutedCommand CommandOpen = new RoutedCommand();
        public static RoutedCommand CommandSave = new RoutedCommand();
        public static RoutedCommand CommandNew = new RoutedCommand();

        private void RefreshTitle()
        {
            string s = Modified ? "* " : "";
            Title = s + FileName;
        }

        public bool CheckModifiedCode(bool noAllowed)
        {
            if (Modified)
            {
                MessageBoxResult result =
                    MessageBox.Show("Save File " + FileName + "?", "Save",
                    noAllowed ? MessageBoxButton.YesNoCancel : MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                    return false;
                if (result == MessageBoxResult.Yes ||
                    result == MessageBoxResult.OK)
                    return Save();
            }
            return true;
        }

        public void New()
        {
            if (!CheckModifiedCode(true))
                return;
            Code = InitialCode;
            FileName = "Unnamed";
            FilePath = "";
            Modified = false;
        }

        public void Open()
        {
            if (!CheckModifiedCode(true))
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text documents (.txt)|*.txt";
            openFileDialog.DefaultExt = ".txt";
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                FileName = openFileDialog.SafeFileName;
                ui_editor.Load(m_filePath);
                Modified = false;
            }
        }

        public bool Save()
        {
            if (m_filePath != "")
            {
                try
                {
                    ui_editor.Save(m_filePath);
                    Modified = false;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("FAILED TO SAVE FILE :\n" + ex.Message);
                    return false;
                }
            }
            else
            {
                return SaveAs();
            }
        }

        public bool SaveAs()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text documents (.txt)|*.txt";
                saveFileDialog.FileName = FileName;
                saveFileDialog.DefaultExt = ".txt";

                if (saveFileDialog.ShowDialog() == true)
                {
                    FilePath = saveFileDialog.FileName;
                    FileName = saveFileDialog.SafeFileName;
                    return Save();
                }
                    
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("FAILED TO SAVE FILE :\n" + ex.Message);
                return false;
            }
        }

        private void CommandOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Open();
        }

        private void CommandSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void CommandNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            New();
        }

        #endregion

        #region Config

        private Config m_config = new Config();
        public Config Configure
        {
            get { return m_config; }
            set { m_config = value; }
        }
        public void UpdateConfig()
        {
            LoadConfig(m_config);
        }

        private void LoadConfig(Config config)
        {
            string xshdString = config.ToXshdString();
            TextReader tr = new StringReader(xshdString);
            XshdSyntaxDefinition syntaxDefinition;
            using (XmlTextReader reader = new XmlTextReader(tr))
            {
                syntaxDefinition = HighlightingLoader.LoadXshd(reader);
            }
            HighlightingManager manager = HighlightingManager.Instance;
            IHighlightingDefinition def = HighlightingLoader.Load(syntaxDefinition, manager);
            ui_editor.SyntaxHighlighting = def;

            ui_editor.FontFamily = new FontFamily(Configure.TextFont);
            ui_editor.FontSize = Configure.TextSize;
            ui_editor.FontStyle = Configure.TextStyle;
            ui_editor.FontWeight = Configure.TextWeight;
            ui_editor.Foreground = new SolidColorBrush(Configure.TextColor);

            ui_editor.Background = new SolidColorBrush(Configure.BackGroundColor);
            m_caretLineRenderer.BgColor = Configure.CurrentLineColor;
            m_errorColor = Configure.ErrorLineColor;
            m_pauseColor = Configure.PauseLineColor;
            m_breakPointMargin.BreakPointBrush
                = new SolidColorBrush(Configure.BreakpointColor);
            m_breakPointMargin.PausingArrowBrush
                = new SolidColorBrush(Configure.PauseArrowColor);

            ui_editor.LineNumbersForeground
                = new SolidColorBrush(Configure.LineNumberColor);
            ui_editor.ShowLineNumbers = Configure.ShowLineNumber;
            ui_editor.Options.IndentationSize = Configure.IndentionSize;
        }

        public class Config
        {
            public Config()
            {
                HighlightSettings.Add(cfg_Comment,
                    new Highlight(Colors.Gray, false, true));
                //HighlightSettings.Add(cfg_String,
                //    new Highlight(Colors.Black, false, false));
                HighlightSettings.Add(cfg_KeyWord,
                    new Highlight(Colors.Blue, true, false));
                HighlightSettings.Add(cfg_Function,
                    new Highlight(Colors.BlueViolet, false, false));
                HighlightSettings.Add(cfg_Digit,
                    new Highlight(Colors.Red, false, false));
            }

            #region Member Variable

            public static readonly string cfg_Comment = "Comment";
            public static readonly string cfg_String = "String";
            public static readonly string cfg_KeyWord = "LogoScriptKeyWords";
            public static readonly string cfg_Function = "LogoScriptGlobalFunctions";
            public static readonly string cfg_Digit = "Digits";

            public static readonly string cfg_TextFont = "TextFont";
            public static readonly string cfg_TextSize = "TextSize";
            public static readonly string cfg_TextStyle = "TextStyle";
            public static readonly string cfg_TextWeight = "TextWeight";
            public static readonly string cfg_TextColor = "TextColor";
            public string TextFont = "consolas";
            public double TextSize = 16;
            public FontStyle TextStyle = FontStyles.Normal;
            public FontWeight TextWeight = FontWeights.Normal;
            public Color TextColor = Colors.Black;

            public static readonly string cfg_BackGroundColor = "BackGroundColor";
            public Color BackGroundColor
                = Colors.White;
            public static readonly string cfg_CurrentLineColor = "CurrentLineColor";
            public Color CurrentLineColor
                = Color.FromArgb(0x30, 0, 0, 0xFF);
            public static readonly string cfg_PauseArrowColor = "PauseArrowColor";
            public Color PauseArrowColor
                = Colors.Orange;
            public static readonly string cfg_PauseLineColor = "PauseLineColor";
            public Color PauseLineColor
                = Colors.Yellow;
            public static readonly string cfg_ErrorLineColor = "ErrorLineColor";
            public Color ErrorLineColor
                = Color.FromArgb(0x70, 0xFF, 0, 0);
            public static readonly string cfg_LineNumberColor = "LineNumberColor";
            public Color LineNumberColor
                = Colors.DarkCyan;
            public static readonly string cfg_BreakpointColor = "BreakpointColor";
            public Color BreakpointColor
                = Colors.Red;

            public static readonly string cfg_IndentionSize = "IndentionSize";
            public int IndentionSize = 6;
            public static readonly string cfg_ShowLineNumber = "ShowLineNumber";
            public bool ShowLineNumber = true;

            #endregion

            #region Convert

            private string ByteToString(byte b)
            {
                string ans = "";
                ans += NumToChar[b / 16];
                ans += NumToChar[b % 16];
                return ans;
            }

            private string ColorToString(Color c)
            {
                string str = "#" + ByteToString(c.A) +
                    ByteToString(c.R) + ByteToString(c.G) + ByteToString(c.B);
                return str;
            }

            private string HighlightToString(Highlight hl)
            {
                string ans = ColorToString(hl.Color) + ";";
                ans += hl.Bold ? "bold;" : "normal;";
                ans += hl.Italic ? "italic" : "normal";
                return ans;
            }

            private int CharToInt(char c)
            {
                if ('0' <= c && c <= '9')
                    return c - '0';
                else if ('A' <= c && c <= 'F')
                    return c - 'A' + 10;
                else
                    return 0;
            }

            private byte StringToByte(string s, int index)
            {
                try
                {
                    return (byte)(CharToInt(s[index]) * 16 + CharToInt(s[index + 1]));
                }
                catch { return 0; }
            }

            private Color StringToColor(string s)
            {
                byte r = 0, g = 0, b = 0, a = 0;
                a = StringToByte(s, 1);
                r = StringToByte(s, 3);
                g = StringToByte(s, 5);
                b = StringToByte(s, 7);
                return Color.FromArgb(a, r, g, b);
            }

            private Highlight StringToHighlight(string s)
            {
                string[] split = s.Split(';');
                try
                {
                    Highlight hl = new Highlight();
                    hl.Color = StringToColor(split[0]);
                    hl.Bold = (split[1].ToLower() == "bold");
                    hl.Italic = (split[2].ToLower() == "italic");
                    return hl;
                }
                catch { return new Highlight(Colors.Black, false, false); }
            }

            #endregion

            #region Xshd Builder

            public struct Highlight
            {
                public Highlight(Color c, bool bold, bool italic)
                {
                    Color = c;
                    Bold = bold;
                    Italic = italic;
                }
                public bool Bold;
                public bool Italic;
                public Color Color;
            }

            public Dictionary<string, Highlight> HighlightSettings
                = new Dictionary<string, Highlight>();

            public HashSet<string> KeyWords
                = new HashSet<string>(new string[] { 
                    "add", "sub", "and", "or", "xor", "sll", "srl", "sra", "jr",
                    "addi", "andi", "ori", "xori", "lw", "sw", "beq", "bne", "lui",
                    "j", "jal"
                });

            public HashSet<string> KeyFunctions
                = new HashSet<string>(new string[] {
                    "$0", "$1", "$2", "$3", "$4", "$5", "$6", "$7", "$8", "$9",
                    "$10", "$11", "$12", "$13", "$14", "$15", "$16", "$17", "$18", "$19",
                    "$20", "$21", "$22", "$23", "$24", "$25", "$26", "$27", "$28", "$29",
                    "$30", "$31", "$ra"
                });

            private static readonly char[] NumToChar = new char[] {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'A', 'B', 'C', 'D', 'E', 'F'
            };

            public string ToXshdString()
            {
                StringBuilder sb = new StringBuilder(65536);

                sb.AppendLine("<?xml version=\"1.0\"?>");
                sb.AppendLine("<SyntaxDefinition name=\"LogoScript\" extensions=\".ls\" xmlns=\"http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008\">");

                foreach (var setting in HighlightSettings)
                {
                    sb.Append("<Color name=\"");
                    sb.Append(setting.Key);
                    sb.Append("\" foreground=\"");
                    sb.Append(ColorToString(setting.Value.Color));
                    sb.Append("\"");

                    if (setting.Value.Bold)
                        sb.Append(" fontWeight=\"bold\"");
                    if (setting.Value.Italic)
                        sb.Append(" fontStyle=\"italic\"");

                    sb.AppendLine(" />");
                }

                sb.AppendLine("<RuleSet ignoreCase=\"true\">");

                sb.AppendLine(string.Format("<Keywords color=\"{0}\">", cfg_KeyWord));
                foreach (var word in KeyWords)
                    sb.AppendLine(string.Format("<Word>{0}</Word>", word));
                sb.AppendLine("</Keywords>");

                sb.AppendLine(string.Format("<Keywords color=\"{0}\">", cfg_Function));
                foreach (var word in KeyFunctions)
                    sb.AppendLine(string.Format("<Word>{0}</Word>", word));
                sb.AppendLine("</Keywords>");

                sb.AppendLine(string.Format("<Span color=\"{0}\">", cfg_Comment));
                sb.AppendLine("<Begin>\\#</Begin>");
                sb.AppendLine("</Span>");

                sb.AppendLine(string.Format("<Rule color=\"{0}\">", cfg_Digit));
			    sb.AppendLine("\\b0[xX][0-9a-fA-F]+|(\\b\\d+(\\.[0-9]+)?|\\.[0-9]+)");
		        sb.AppendLine("</Rule>");

                sb.AppendLine("</RuleSet>");
                sb.AppendLine("</SyntaxDefinition>");

                return sb.ToString();
            }

            #endregion

            #region Save/Load

            public void Save(TextWriter tw)
            {
                LogoScriptIDE.Config cfg = new LogoScriptIDE.Config();
                
                cfg.SetField(cfg_Comment, HighlightToString(HighlightSettings[cfg_Comment]));
                cfg.SetField(cfg_String, HighlightToString(HighlightSettings[cfg_String]));
                cfg.SetField(cfg_KeyWord, HighlightToString(HighlightSettings[cfg_KeyWord]));
                cfg.SetField(cfg_Function, HighlightToString(HighlightSettings[cfg_Function]));
                cfg.SetField(cfg_Digit, HighlightToString(HighlightSettings[cfg_Digit]));

                cfg.SetField(cfg_TextFont, TextFont);
                cfg.SetField(cfg_TextSize, TextSize.ToString());
                cfg.SetField(cfg_TextStyle, TextStyle == FontStyles.Italic ? "italic" : "normal");
                cfg.SetField(cfg_TextWeight, TextWeight == FontWeights.Bold ? "bold" : "normal");
                cfg.SetField(cfg_TextColor, ColorToString(TextColor));

                cfg.SetField(cfg_BackGroundColor, ColorToString(BackGroundColor));
                cfg.SetField(cfg_CurrentLineColor,ColorToString(CurrentLineColor));
                cfg.SetField(cfg_PauseArrowColor,ColorToString(PauseArrowColor));
                cfg.SetField(cfg_PauseLineColor,ColorToString(PauseLineColor));
                cfg.SetField(cfg_ErrorLineColor,ColorToString(ErrorLineColor));
                cfg.SetField(cfg_LineNumberColor,ColorToString(LineNumberColor));
                cfg.SetField(cfg_BreakpointColor, ColorToString(BreakpointColor));

                cfg.SetField(cfg_IndentionSize, IndentionSize.ToString());
                cfg.SetField(cfg_ShowLineNumber, ShowLineNumber ? "true" : "false");

                cfg.Save(tw);
            }

            public void Load(TextReader tr)
            {
                LogoScriptIDE.Config cfg = new LogoScriptIDE.Config();
                cfg.Load(tr);

                HighlightSettings[cfg_Comment] = StringToHighlight(cfg.GetField(cfg_Comment));
                HighlightSettings[cfg_String] = StringToHighlight(cfg.GetField(cfg_String));
                HighlightSettings[cfg_KeyWord] = StringToHighlight(cfg.GetField(cfg_KeyWord));
                HighlightSettings[cfg_Function] = StringToHighlight(cfg.GetField(cfg_Function));
                HighlightSettings[cfg_Digit] = StringToHighlight(cfg.GetField(cfg_Digit));

                TextFont = cfg.GetField(cfg_TextFont);
                try { TextSize = Int32.Parse(cfg.GetField(cfg_TextSize)); }
                catch { TextSize = 16; }
                if (TextSize < 9 || TextSize > 100) TextSize = 16;
                TextStyle = (cfg.GetField(cfg_TextStyle).ToLower() == "italic" ?
                    FontStyles.Italic : FontStyles.Normal);
                TextWeight = (cfg.GetField(cfg_TextWeight).ToLower() == "bold" ?
                    FontWeights.Bold : FontWeights.Normal);
                TextColor = StringToColor(cfg.GetField(cfg_TextColor));

                BackGroundColor = StringToColor(cfg.GetField(cfg_BackGroundColor));
                CurrentLineColor = StringToColor(cfg.GetField(cfg_CurrentLineColor));
                PauseArrowColor = StringToColor(cfg.GetField(cfg_PauseArrowColor));
                PauseLineColor = StringToColor(cfg.GetField(cfg_PauseLineColor));
                ErrorLineColor = StringToColor(cfg.GetField(cfg_ErrorLineColor));
                LineNumberColor = StringToColor(cfg.GetField(cfg_LineNumberColor));
                BreakpointColor = StringToColor(cfg.GetField(cfg_BreakpointColor));

                try { IndentionSize = Int32.Parse(cfg.GetField(cfg_IndentionSize)); }
                catch { IndentionSize = 4; }
                if (IndentionSize < 1 || IndentionSize > 8) IndentionSize = 4;
                ShowLineNumber = (cfg.GetField(cfg_ShowLineNumber).ToLower() == "true" ? true : false);
            }

            #endregion
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
           
        }
    }
}
