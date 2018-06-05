using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Readearth.Chart2D.BasicStyle;
using Readearth.Chart2D.Additional;

namespace Readearth.Chart2D.Data
{
    public class DataSeries
    {
        //private int dataPointIndex;
        //Í¨ÓÃ
        private ArrayList pointList;
        private SymbolStyle symbolStyle;
        private string seriesName = "";
        private bool isY2Data = false;

        private LineStyle lineStyle;
        private LineCharts.LineChartTypeEnum lineChartType = LineCharts.LineChartTypeEnum.Line;
        //1.4Îó²îÏßÍ¼
        private ArrayList errorList;
        private LineStyle errorlineStyle;
        //2Ãæ»ýÍ¼£¬3Öù×´Í¼, 5±ý×´Í¼
        private Color fillColor;
        //4¹ÉÆ±Í¼
        private string[,] dataString;
        //5±ý×´Í¼
        private bool isExplode;

        //private RenderType renderType = RenderType.ColorBySeries;


        public DataSeries()
        {
            pointList = new ArrayList();
            errorList = new ArrayList();
            lineStyle = new LineStyle();
            errorlineStyle = new LineStyle();
            SymbolStyle = new SymbolStyle();

            //valueList = new ArrayList();
        }

        public LineCharts.LineChartTypeEnum LineChartType
        {
            get { return lineChartType; }
            set 
            {
                lineChartType = value;
                if (lineChartType == LineCharts.LineChartTypeEnum.Spline)
                    LineStyle.PlotMethod = LineStyle.PlotLinesMethodEnum.Splines;
                else
                    LineStyle.PlotMethod = LineStyle.PlotLinesMethodEnum.Lines;
            }
        }

        public LineStyle LineStyle
        {
            get { return lineStyle; }
            set 
            {
                lineStyle = value;
            }
        }

        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        public string[,] DataString
        {
            get { return dataString; }
            set { dataString = value; }
        }

        public bool IsExplode
        {
            get { return isExplode; }
            set { isExplode = value; }
        }

        public LineStyle ErrorLineStyle
        {
            get { return errorlineStyle; }
            set
            {
                errorlineStyle = value;
            }
        }


        public SymbolStyle SymbolStyle
        {
            get { return symbolStyle; }
            set { symbolStyle = value; }
        }

        //public RenderType RenderType
        //{
        //    get { return renderType; }
        //    set { renderType = value; }
        //}

        public string SeriesName
        {
            get { return seriesName; }
            set { seriesName = value; }
        }

        public bool IsY2Data
        {
            get { return isY2Data; }
            set { isY2Data = value; }
        }
        public ArrayList PointList
        {
            get { return pointList; }
            set { pointList = value; }
        }
        public void AddPoint(PointF pointf)
        {
            pointList.Add(pointf);
        }
        public void AddData(float data)
        {
            pointList.Add(data);
        }

        public ArrayList ErrorList
        {
            get { return errorList; }
            set { errorList = value; }
        }

        public void AddErrorData(PointF pt)
        {
            errorList.Add(pt);
        }
    }

    //public class DataValue
    //{
    //    private double xValue;
    //    private string s_xValue;
    //    private Type xValueType;
    //    private float yValue;
    //    public DataValue()
    //    {
    //        xValue = 0;
    //        yValue = 0;
    //        xValueType = typeof(float);
    //    }

    //    public DataValue(object xvalue, float yvalue, Type type)
    //    {
    //        xValueType = type;
    //        if (type == typeof(DateTime))
    //            xValue = TimeConverter.ConvertDateTimeInt(Convert.ToDateTime(xvalue));
    //        else if (type == typeof(double))
    //            xValue = Convert.ToDouble(xvalue);
    //        else
    //            s_xValue = xvalue.ToString();
    //        yValue = yvalue;
    //    }

    //    public Type XValueType
    //    {
    //        get { return xValueType; }
    //        set { xValueType = value; }
    //    }
    //    public double XValue
    //    {
    //        get { return xValue; }
    //        set { xValue = value; }
    //    }
    //    public string StrXValue
    //    {
    //        get { return s_xValue; }
    //        set { s_xValue = value; }
    //    }
    //    public float YValue
    //    {
    //        get { return yValue; }
    //        set { yValue = value; }
    //    }

    //}

    //public enum RenderType
    //{
    //    ColorBySeries=0,
    //    ColorByPoint=1
    //}

}


