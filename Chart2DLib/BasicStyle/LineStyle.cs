using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Readearth.Chart2D.BasicStyle
{
    public class LineStyle
    {
        private DashStyle linePattern = DashStyle.Solid;
        private Color lineColor = Color.Black;
        private float lineThickness = 1.0f;
        private PlotLinesMethodEnum pltLineMethod = PlotLinesMethodEnum.Lines;
        private bool isVisible = true;

        public LineStyle()
        {
        }
        public LineStyle(Color linecolor, float thick, DashStyle pattern)
        {
            lineColor = linecolor;
            lineThickness = thick;
            linePattern = pattern;
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        public DashStyle LinePattern
        {
            get { return linePattern; }
            set { linePattern = value; }
        }

        public float LineThickness
        {
            get { return lineThickness; }
            set { lineThickness = value; }
        }

        public Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }

        internal PlotLinesMethodEnum PlotMethod
        {
            get { return pltLineMethod; }
            set { pltLineMethod = value; }
        }
        internal enum PlotLinesMethodEnum
        {
            Lines = 0,
            Splines = 1
        }

        public LineStyle Clone()
        {
            LineStyle newLS = new LineStyle(this.LineColor, this.LineThickness, this.LinePattern);
            newLS.IsVisible = this.IsVisible;
            newLS.PlotMethod = this.PlotMethod;
            return newLS;
        }
    }
}


