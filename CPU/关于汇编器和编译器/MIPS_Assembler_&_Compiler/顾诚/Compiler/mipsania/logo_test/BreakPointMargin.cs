using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

namespace LogoScriptIDE
{
    public class BreakPointMargin : AbstractMargin
    {
        private TextArea m_textArea;
        private TextEditor m_editor;

        public BreakPointMargin(TextEditor editor)
        {
            BreakPointBrush = Brushes.OrangeRed;
            PausingArrowBrush = Brushes.Yellow;
            m_editor = editor;
        }
        

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (TextView != null &&
                TextView.VisualLinesValid &&
                TextView.VisualLines.Count > 0)
            {
                return new Size(TextView.VisualLines[0].Height, 0);
            }
            else
            {
                return new Size(m_editor.FontSize + 3, 0);
            }
        }

        public Brush BreakPointBrush { get; set; }
        public Brush PausingArrowBrush { get; set; }

        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext)
        {
            TextView textView = this.TextView;
            Size renderSize = this.RenderSize;
            if (textView != null && textView.VisualLinesValid)
            {
                double radius = textView.VisualLines[0].Height / 2;
                foreach (VisualLine line in textView.VisualLines)
                {
                    int lineNumber = line.FirstDocumentLine.LineNumber;
                    double yOffset = line.VisualTop - textView.VerticalOffset;
                    if (GetAnchorAtLine(lineNumber) != null)
                    {
                        drawingContext.DrawEllipse(BreakPointBrush, null,
                            new Point(radius, radius + yOffset),
                                radius * 0.9, radius * 0.9);
                    }
                    if (lineNumber == PausingLineNumber)
                    {
                        Path myPath = new Path();
                        var geometry = new StreamGeometry();
                        double size = radius * 2;
                        
                        using (StreamGeometryContext ctx = geometry.Open())
                        {
                            ctx.BeginFigure(new Point(size * 0.25, size * 0.15 + yOffset), true, true);
                            ctx.LineTo(new Point(size * 0.85, size * 0.5 + yOffset), true, false);
                            ctx.LineTo(new Point(size * 0.25, size * 0.85 + yOffset), true, false);
                        }
                        geometry.Freeze();
                        
                        drawingContext.DrawGeometry(
                            PausingArrowBrush, null, geometry);
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
        {
            if (oldTextView != null)
            {
                oldTextView.VisualLinesChanged -= TextViewVisualLinesChanged;
            }
            base.OnTextViewChanged(oldTextView, newTextView);
            if (newTextView != null)
            {
                newTextView.VisualLinesChanged += TextViewVisualLinesChanged;
                // find the text area belonging to the new text view
                m_textArea = newTextView.Services.GetService(typeof(TextArea)) as TextArea;
            }
            else
            {
                m_textArea = null;
            }
            InvalidateVisual();
        }

        void TextViewVisualLinesChanged(object sender, EventArgs e)
        {
            RefreshAnchors();
            InvalidateVisual();
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!e.Handled && TextView != null && m_textArea != null)
            {
                e.Handled = true;
                m_textArea.Focus();

                Point pos = e.GetPosition(TextView);
                pos.X = 0;
                pos.Y += TextView.VerticalOffset;
                VisualLine vl = TextView.GetVisualLineFromVisualTop(pos.Y);
                if (vl == null) return;

                int lineNumber = vl.FirstDocumentLine.LineNumber;
                RefreshAnchors();

                TextAnchor anchor = GetAnchorAtLine(lineNumber);
                if (anchor == null)
                {
                    int refer = lineNumber;
                    var newAnchor = CreateAnchorAtLine(ref refer, false);
                    if (newAnchor != null)
                        m_anchors.Add(newAnchor);
                }
                else
                    m_anchors.Remove(anchor);

                InvalidateVisual();
            }
        }

        /// <inheritdoc/>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            // accept clicks even when clicking on the background
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        private ICollection<TextAnchor> m_anchors = new HashSet<TextAnchor>();

        private void RefreshAnchors()
        {
            HashSet<TextAnchor> newAnchors = new HashSet<TextAnchor>();
            HashSet<int> lines = new HashSet<int>();
            foreach (var anchor in m_anchors)
            {
                if (anchor.IsDeleted)
                    continue;
                if (lines.Contains(anchor.Line))
                    continue;

                int lineNumber = anchor.Line;
                var newAnchor = CreateAnchorAtLine(ref lineNumber, false);
                if (newAnchor != null)
                {
                    lines.Add(lineNumber);
                    newAnchors.Add(newAnchor);
                }
            }
            m_anchors = newAnchors;
        }

        private TextAnchor GetAnchorAtLine(int lineNumber)
        {
            foreach (var anchor in m_anchors)
            {
                if (anchor.Line == lineNumber) return anchor;
            }
            return null;
        }

        private TextAnchor CreateAnchorAtLine(ref int lineNumber, bool strict)
        {
            var line = Document.GetLineByNumber(lineNumber);
            int offset = line.Offset;
            while (offset < line.EndOffset)
            {
                if (Document.Text[offset] == '#') break;
                if (Char.IsWhiteSpace(Document.Text[offset]))
                    ++offset;
                else
                    return Document.CreateAnchor(offset);
            }
            if (strict || lineNumber == Document.LineCount)
                return null;
            else
            {
                lineNumber++;
                return CreateAnchorAtLine(ref lineNumber, strict);
            }
        }

        public void SetBreakPoints(ICollection<int> lineNumbers)
        {
            ClearBreakPoints();
            foreach (int number in lineNumbers)
            {
                int refer = number;
                var anchor = CreateAnchorAtLine(ref refer, true);
                if (anchor == null)
                    throw new Exception("Invalid line number: " + number.ToString());
                m_anchors.Add(anchor);
            }
        }

        public void GetBreakPoints(ICollection<int> lineNumbers)
        {
            lineNumbers.Clear();
            foreach (var anchor in m_anchors)
                lineNumbers.Add(anchor.Line);
        }

        public void ClearBreakPoints()
        {
            m_anchors.Clear();
        }

        private int m_pausingLineNumber;
        public int PausingLineNumber
        {
            get { return m_pausingLineNumber; }
            set { m_pausingLineNumber = value; }
        }
    }
}
