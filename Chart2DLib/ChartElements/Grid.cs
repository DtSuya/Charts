using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Readearth.Chart2D.BasicStyle;

namespace Readearth.Chart2D.ChartElements
{
    [TypeConverter(typeof(GridConverter))]
    public class Grid
    {
        private LineStyle gridStyle;
        private bool isXGrid = true;
        private bool isYGrid = true;
        //private Chart2D chart2d;

        public Grid()
        {
            //chart2d = ct2d;
            gridStyle = new LineStyle();
            gridStyle.LineColor = Color.LightGray;
        }

        [Description("Indicates whether the X grid is shown."),
        Category("Appearance")]
        public bool IsXGrid
        {
            get { return isXGrid; }
            set
            {
                isXGrid = value;
                //chart2d.AddChart();
            }
        }

        [Description("Indicates whether the Y grid is shown."),
        Category("Appearance")]
        public bool IsYGrid
        {
            get { return isYGrid; }
            set
            {
                isYGrid = value;
                //chart2d.AddChart();
            }
        }

        public LineStyle GridStyle
        {
            get { return gridStyle; }
            set { gridStyle = value; }
        }

        [Description("Sets the line pattern for the grid lines."),
        Category("Appearance")]
        virtual public DashStyle GridPattern
        {
            get { return gridStyle.LinePattern; }
            set
            {
                gridStyle.LinePattern = value;
                //chart2d.AddChart();
            }
        }

        [Description("Sets the thickness for the grid lines."),
        Category("Appearance")]
        public float GridThickness
        {
            get { return gridStyle.LineThickness; }
            set
            {
                gridStyle.LineThickness = value;
                //chart2d.AddChart();
            }
        }

        [Description("The color used to display the grid lines."),
        Category("Appearance")]
        virtual public Color GridColor
        {
            get { return gridStyle.LineColor; }
            set
            {
                gridStyle.LineColor = value;
                //chart2d.AddChart();
            }
        }

        public Grid Clone()
        {
            Grid newGD = new Grid();
            newGD.IsXGrid = this.IsXGrid;
            newGD.IsYGrid = this.IsYGrid;
            newGD.GridStyle = this.GridStyle.Clone();
            return newGD;
        }
    }

    public class GridConverter : TypeConverter
    {
        public GridConverter()
        {

        }

        // allows us to display the + symbol near the property name
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(Grid));
        }
    }
}
