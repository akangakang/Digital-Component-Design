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
    class CaretLineRenderer : IBackgroundRenderer
    {
        private TextEditor m_editor;

        public CaretLineRenderer(TextEditor editor)
        {
            m_editor = editor;
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Caret; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (m_editor.Document == null)
                return;

            textView.EnsureVisualLines();
            var currentLine = m_editor.Document.GetLineByOffset(m_editor.CaretOffset);
            
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                double drawingWidth = textView.ScrollOffset.X + textView.ActualWidth;
                drawingContext.DrawRectangle(
                    new SolidColorBrush(BgColor), null,
                    new Rect(rect.Location, new Size(drawingWidth, rect.Height)));
            }
        }

        private Color m_color = Color.FromArgb(0x20, 0, 0, 0xFF);
        public Color BgColor {
            get { return m_color; }
            set { m_color = value; }
        }
    }
}
