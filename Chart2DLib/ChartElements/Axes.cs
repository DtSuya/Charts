using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Readearth.Chart2D.BasicStyle;
using Readearth.Chart2D.Addtional;

namespace Readearth.Chart2D.ChartElements
{
    public enum TickMarkFrom
    {
        None = 0,
        TimeStamp = 1,
        Strings = 2,
        Number = 3,
    }
    //坐标轴范围和刻度标记
    [TypeConverter(typeof(XAxisConverter))]
    public class XAxis
    {
        private TextStyle xtickStyle = new TextStyle();
        private float xLimMin = 0f;
        private float xLimMax = 10f;
        private float xTick = 2f;
        private string[] xTickMark;
        private int xmarkStartindex = 0;
        private int xmarkInterval = 1;
        private string xmarkFormat = string.Empty;
        private TickMarkFrom xmarkFrom = TickMarkFrom.None;

        private string[] xTickMarkFull;
        private float xtickangle = 0;

        public XAxis()
        {
            xtickStyle.TextSize = 9;
        }

        [Description("The style used to display the ticks."), Category("Appearance")]
        public TextStyle XTickStyle
        {
            get { return xtickStyle; }
            set { xtickStyle = value; }
        }

        [Description("Sets the maximum limit for the X axis."), Category("Appearance")]
        public float XLimMax
        {
            get { return xLimMax; }
            set
            {
                xLimMax = value;
                SetXTickMark(xTickMark, XMarkStartindex, XMarkInterval);
            }
        }

        [Description("Sets the minimum limit for the X axis."), Category("Appearance")]
        public float XLimMin
        {
            get { return xLimMin; }
            set
            {
                xLimMin = value;
                SetXTickMark(xTickMark, XMarkStartindex, XMarkInterval);
            }
        }

        [Description("Sets the ticks for the X axis."), Category("Appearance")]
        public float XTick
        {
            get { return xTick; }
            set
            {
                if (value != 0)
                {
                    xTick = value;
                    SetXTickMark(xTickMark, XMarkStartindex, XMarkInterval);
                }
            }
        }

        public int XMarkStartindex 
        { 
            get { return xmarkStartindex;}
            set 
            {
                xmarkStartindex = (value >= 0) ? value : 0;
                setXTickMarkFull();
            }
        }

        public int XMarkInterval 
        {
            get { return xmarkInterval ; }
            set
            {
                xmarkInterval = (value >= 1) ? value : 1;
                setXTickMarkFull();
            }
        }

        public string XMarkFormat
        {
            get { return xmarkFormat; }
            //set
            //{
            //    xmarkFormat = value;
            //}
        }

        /// <summary>
        /// X轴tick标记内容
        /// </summary>
        public string[] XTickMark
        {
            get
            {
                if (xTickMark == null)
                    defaultXTickMark();
                return xTickMark;
            }
            set
            {
                xTickMark = value;
                if (xTickMark == null)
                {
                    xmarkFrom = TickMarkFrom.None;
                    defaultXTickMark();
                }
                else
                {
                    xmarkFrom = TickMarkFrom.Number;
                    refreshXTickMark();
                }
            }
        }
        public void SetXTickMark(string[] xtickmarks)
        {
            if (xtickmarks != null)
            {
                xTickMark = xtickmarks;
                xmarkFrom = TickMarkFrom.Strings;
                refreshXTickMark();
            }
 
        }
        /// <summary>
        /// 设置X轴tick标记
        /// </summary>
        /// <param name="xtickmarks">X轴tick标记内容</param>
        /// <param name="startIndex">标记起始位置索引，默认从0开始</param>
        /// <param name="interval">标记间隔，默认为1</param>
        public void SetXTickMark(string[] xtickmarks, int startIndex, int interval)
        {
            if (xtickmarks != null)
            {
                xTickMark = xtickmarks;
                xmarkStartindex = (startIndex >= 0) ? startIndex : 0;
                xmarkInterval = (interval >= 1) ? interval : 1;

                refreshXTickMark();
            }
        }
        /// <summary>
        /// 设置X轴tick标记(时间格式字符串)
        /// </summary>
        /// <param name="xtickmarks">时间文本</param>
        /// <param name="Format">tick时间格式</param>
        public void SetXTickMark(string[] xtickmarks, string Format)
        {
            if (xtickmarks != null)
            {
                try
                {
                    string[] newmarks = new string[xtickmarks.Length];
                    xmarkFormat = Format;
                    xmarkFrom = TickMarkFrom.Strings;
                    for (int i = 0; i < xtickmarks.Length; i++)
                    {
                        newmarks[i] = Convert.ToDateTime(xtickmarks[i]).ToString(XMarkFormat);
                    }
                    xTickMark = newmarks;

                    refreshXTickMark();
                }
                catch
                {
                    xmarkFormat = string.Empty;
                    xmarkFrom = TickMarkFrom.None;
                    refreshXTickMark();
                }
            }
        }
        /// <summary>
        /// 设置X轴tick标记(时间戳)
        /// </summary>
        /// <param name="starttime">起始时间</param>
        /// <param name="xtimeStamps">时间戳(单位为秒)</param>
        /// <param name="Format">tick时间格式</param>
        public void SetXTickMark(DateTime starttime, long[] xtimeStamps, string Format)
        {
            if (xtimeStamps != null)
            {
                try
                {
                    string[] xtickmarks = new string[xtimeStamps.Length];
                    xmarkFormat = Format;
                    xmarkFrom = TickMarkFrom.TimeStamp;
                    for (int i = 0; i < xtimeStamps.Length; i++)
                    {
                        xtickmarks[i] = TimeConverter.GetTime(xtimeStamps[i], starttime).ToString(XMarkFormat);
                    }
                    xTickMark = xtickmarks;

                    refreshXTickMark();
                }
                catch 
                {
                    xmarkFormat = string.Empty;
                    xmarkFrom = TickMarkFrom.None;
                    refreshXTickMark();
                }
            }
        }

        /// <summary>
        /// 默认tick标记内容
        /// </summary>
        private void defaultXTickMark()
        {
            int count = (int)Math.Floor((xLimMax - xLimMin) / xTick) + 1;
            xTickMark = new string[count];
            for (int n = 0; n < count; n++)
            {
                xTickMark[n] = (xLimMin + n * xTick).ToString();
            }
            setXTickMarkFull();
        }
        /// <summary>
        /// 由于limit和tick变化引起的tickmark刷新
        /// 待完善：limit范围变小或tick变大时marks完整，否则出现不完整状况
        /// 建议先设置limit和tick,再赋值tickmark
        /// </summary>
        private void refreshXTickMark()
        {
            int count = (int)Math.Round((xLimMax - xLimMin) / xTick) + 1;
            if (count < 1 || count>200)
                return ;
            string[] newmarks = new string[count];
            if (xmarkFrom == TickMarkFrom.None && string.IsNullOrWhiteSpace(xmarkFormat))
            {
                for (int n = 0; n < count; n++)
                {
                    newmarks[n] = (xLimMin + n * xTick).ToString();
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < xTickMark.Length)
                        newmarks[i] = xTickMark[i];
                    else
                        newmarks[i] = string.Empty;
                }
            }
            xTickMark = newmarks;
            setXTickMarkFull();
        }

        /// <summary>
        /// 所有tick标记
        /// </summary>
        internal string[] XTickMarkFull
        {
            get
            {
                if (xTickMarkFull == null)
                    setXTickMarkFull();
                return xTickMarkFull;
            }
        }
        /// <summary>
        /// 补全间隔情况下所有tick标记内容
        /// </summary>
        private void setXTickMarkFull()
        {
            int count = (int)Math.Round((xLimMax - xLimMin) / xTick) + 1;
            xTickMarkFull = new string[count];
            for (int i = 0; i < count; i++)
            {
                int x = XMarkStartindex + i * XMarkInterval;
                if (x < count && i < XTickMark.Length)
                    xTickMarkFull[x] = XTickMark[i];
            }
        }

        internal float XTickAngle
        {
            get { return xtickangle; }
            set { xtickangle = value; }
        }

        /// <summary>
        /// X坐标轴样式复制
        /// </summary>
        /// <returns></returns>
        public XAxis Clone()
        {
            XAxis newXA = new XAxis();
            newXA.XLimMin = this.XLimMin;
            newXA.XLimMax = this.XLimMax;
            newXA.XTick = this.XTick;
            newXA.XMarkInterval = this.XMarkInterval;
            newXA.XMarkStartindex = this.XMarkStartindex;
            newXA.XTickMark = this.XTickMark;
            newXA.XTickStyle = this.XTickStyle.Clone();
            newXA.XTickAngle = this.XTickAngle;
            return newXA;
        }
    }

    public class XAxisConverter : TypeConverter
    {
        public XAxisConverter()
        {

        }

        // allows us to display the + symbol near the property name
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(XAxis));
        }
    }

    [TypeConverter(typeof(YAxisConverter))]
    public class YAxis
    {
        private float yLimMin = 0f;
        private float yLimMax = 10f;
        private float yTick = 2f;
        private TextStyle ytickStyle = new TextStyle();
        //private Chart2D chart2d;

        public YAxis()
        {
            ytickStyle.TextSize = 9;
            //chart2d = ct2d;
        }

        [Description("Sets the maximum limit for the Y axis."), Category("Appearance")]
        public float YLimMax
        {
            get { return yLimMax; }
            set
            {
                yLimMax = value;
                //chart2d.AddChart();
            }
        }

        [Description("Sets the minimum limit for the Y axis."), Category("Appearance")]
        public float YLimMin
        {
            get { return yLimMin; }
            set
            {
                yLimMin = value;
                //chart2d.AddChart();
            }
        }

        [Description("Sets the ticks for the Y axis."), Category("Appearance")]
        public float YTick
        {
            get { return yTick; }
            set
            {
                yTick = value;
                //chart2d.AddChart();
            }
        }

        [Description("The style used to display the ticks."),  Category("Appearance")]
        public TextStyle YTickStyle
        {
            get { return ytickStyle; }
            set
            {
                ytickStyle = value;
                //chart2d.AddChart();
            }
        }

        public YAxis Clone()
        {
            YAxis newYA = new YAxis();
            newYA.YLimMin = this.YLimMin;
            newYA.YLimMax = this.YLimMax;
            newYA.YTick = this.YTick;
            newYA.YTickStyle = this.YTickStyle.Clone();
            return newYA;
        }
    }

    public class YAxisConverter : TypeConverter
    {
        public YAxisConverter()
        {

        }

        // allows us to display the + symbol near the property name
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(YAxis));
        }
    }

    [TypeConverter(typeof(Y2AxisConverter))]
    public class Y2Axis
    {
        private float y2LimMin = 0f;
        private float y2LimMax = 10f;
        private float y2Tick = 2f;
        private TextStyle y2tickStyle = new TextStyle();
        private bool isY2Axis = false;
        //private Chart2D chart2d;

        public Y2Axis()
        {
            y2tickStyle.TextSize = 9;
            //chart2d = ct2d;
        }

        [Description("Indicates whether the chart has the Y2 axis."), Category("Appearance")]
        public bool IsY2Axis
        {
            get { return isY2Axis; }
            set
            {
                isY2Axis = value;
                //chart2d.AddChart();
            }
        }

        [Description("Sets the maximum limit for the Y2 axis."), Category("Appearance")]
        public float Y2LimMax
        {
            get { return y2LimMax; }
            set
            {
                y2LimMax = value;
                //chart2d.AddChart();
            }
        }

        [Description("Sets the minimum limit for the Y2 axis."), Category("Appearance")]
        public float Y2LimMin
        {
            get { return y2LimMin; }
            set
            {
                y2LimMin = value;
                //chart2d.AddChart();
            }
        }

        [Description("Sets the ticks for the Y2 axis."), Category("Appearance")]
        public float Y2Tick
        {
            get { return y2Tick; }
            set
            {
                y2Tick = value;
                //chart2d.AddChart();
            }
        }

        [Description("The style used to display the ticks."),  Category("Appearance")]
        public TextStyle Y2TickStyle
        {
            get { return y2tickStyle; }
            set
            {
                y2tickStyle = value;
                //chart2d.AddChart();
            }
        }

        public Y2Axis Clone()
        {
            Y2Axis newY2A = new Y2Axis();
            newY2A.Y2LimMin = this.Y2LimMin;
            newY2A.Y2LimMax = this.Y2LimMax;
            newY2A.Y2Tick = this.Y2Tick;
            newY2A.Y2TickStyle = this.Y2TickStyle.Clone();
            newY2A.IsY2Axis = this.IsY2Axis;
            return newY2A;
        }
    }

    public class Y2AxisConverter : TypeConverter
    {
        public Y2AxisConverter()
        {

        }

        // allows us to display the + symbol near the property name
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(Y2Axis));
        }
    }

}
