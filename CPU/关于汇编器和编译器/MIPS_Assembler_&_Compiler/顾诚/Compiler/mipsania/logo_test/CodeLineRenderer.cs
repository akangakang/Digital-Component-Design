using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit;

namespace LogoScriptIDE
{
    class CodeLineRenderer : IBackgroundRenderer
    {
        private TextEditor m_editor;

        public CodeLineRenderer(TextEditor editor)
        {
            m_editor = editor;
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (m_editor.Document == null)
                return;

            textView.EnsureVisualLines();

            foreach (var setting in RenderSettings)
            {
                var currentLine = m_editor.Document.GetLineByNumber(setting.LineNumber);

                foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
                {
                    double drawingWidth = textView.ScrollOffset.X + textView.ActualWidth;
                    drawingContext.DrawRectangle(
                        new SolidColorBrush(setting.BgColor), null,
                        new Rect(rect.Location, new Size(drawingWidth, rect.Height)));
                }
            }
        }

        public struct Setting
        {
            public Setting (int lineNumber, Color bgColor)
            {
                LineNumber = lineNumber;
                BgColor = bgColor;
            }
            public int LineNumber;
            public Color BgColor;
        }

        private ICollection<Setting> m_renderSettings = new HashSet<Setting>();
        public ICollection<Setting> RenderSettings
        {
            get { return m_renderSettings; }
        }
    }
}
