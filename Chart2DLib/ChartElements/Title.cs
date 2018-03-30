using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Readearth.Chart2D.BasicStyle;

namespace Readearth.Chart2D.ChartElements
{
    [TypeConverter(typeof(TitleConverter))]
    public class Title
    {
        private string title = "Title";
        private TextStyle titleStyle;
        //private Font titleFont = new Font("Arial", 12, FontStyle.Regular);
        //private Color titleFontColor = Color.Black;
        //private Chart2D chart2d;

        public Title()
        {
            //chart2d = ct2d;
            titleStyle = new TextStyle();
            titleStyle.TextSize = 12f;
        }

        [Description("Creates a title for the chart."),
        Category("Appearance")]
        public string TitleName
        {
            get { return title; }
            set
            {
                title = value;
                //chart2d.AddChart();
            }
        }

        [Description("The style used to display the title."),
        Category("Appearance")]
        public TextStyle TitleStyle
        {
            get { return titleStyle; }
            set
            {
                titleStyle = value;
                //chart2d.AddChart();
            }
        }

        public Title Clone()
        {
            Title newTL = new Title();
            newTL.TitleName = this.TitleName;
            newTL.TitleStyle = this.TitleStyle.Clone();
            return newTL;
        }
    }

    public class TitleConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(Title));
        }
    }
}
