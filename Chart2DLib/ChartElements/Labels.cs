using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Readearth.Chart2D.BasicStyle;

namespace Readearth.Chart2D.ChartElements
{
    //坐标轴标签
    [TypeConverter(typeof(XYLabelConverter))]
    public class XYLabel
    {
        private string xLabel = "X Axis";
        private string yLabel = "Y Axis";
        private string y2Label = "Y2 Axis";
        private bool isXLabelVisible = true;
        private bool isYLabelVisible = true;
        private bool isY2LabelVisible = true;
        private TextStyle labelStyle = new TextStyle();

        //private Chart2D chart2d;

        public XYLabel()
        {
            //chart2d = ct2d;
        }
        public TextStyle XYLabelStyle
        {
            get { return labelStyle; }
            set { labelStyle = value; }
        }

        public string XLabel
        {
            get { return xLabel; }
            set { xLabel = value; }
        }
        public string YLabel
        {
            get { return yLabel; }
            set { yLabel = value; }
        }
        public string Y2Label
        {
            get { return y2Label; }
            set { y2Label = value; }
        }
        public bool IsXLabelVisible
        {
            get { return isXLabelVisible; }
            set { isXLabelVisible = value; }
        }
        public bool IsYLabelVisible
        {
            get { return isYLabelVisible; }
            set { isYLabelVisible = value; }
        }
        public bool IsY2LabelVisible
        {
            get { return isY2LabelVisible; }
            set { isY2LabelVisible = value; }
        }
        public XYLabel Clone()
        {
            XYLabel newLBs = new XYLabel();
            newLBs.XYLabelStyle = this.XYLabelStyle.Clone();
            newLBs.xLabel = this.XLabel;
            newLBs.yLabel = this.YLabel;
            newLBs.y2Label = this.Y2Label;
            newLBs.isXLabelVisible = this.isXLabelVisible;
            newLBs.isYLabelVisible = this.isYLabelVisible;
            newLBs.isY2LabelVisible = this.isY2LabelVisible;
            
            return newLBs;
        }
    }

    public class XYLabelConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(XYLabel));
        }
    }
    


}
