using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using Readearth.Chart2D.ChartElements;
using Readearth.Chart2D.Data;

namespace Readearth.Chart2D
{
    public class LineCharts
    {
        public enum LineChartTypeEnum
        {
            [Description("折线图")]
            Line = 0,
            [Description("曲线图")]
            Spline = 1,
            [Description("阶梯图")]
            StairStep = 2,
            [Description("杆状图")]
            Stem = 3,
            [Description("误差线图")]
            ErrorBar = 4,

            //股票类
            [Description("HiLo")]
            HiLo = 5,
            [Description("HiLo-Open-Close")]
            HiLoOpenClose = 6,
            [Description("K线图")]
            Candle = 7

            //Background = 20,
            //Straight = 21,
        }

        //绘制线性图
        internal static void AddLines(DataCollection dc, Graphics g, ChartStyle cs, XAxis xa, YAxis ya, Y2Axis y2a)
        {
            if (dc.ChartType != Chart2DTypeEnum.LineChart || dc.DataSeriesList.Count == 0)
                return;
            //SeriesColor.SetSeriesColor(dc);

            // Plot lines:
            foreach (DataSeries ds in dc.DataSeriesList)
            {
                if (ds.LineStyle.IsVisible == true)
                {
                    PointF pt = new PointF();
                    PointF pt0 = new PointF();

                    Pen aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.LineThickness);
                    aPen.DashStyle = ds.LineStyle.LinePattern;

                    #region line
                    // Plot Line:
                    if (ds.LineChartType == LineChartTypeEnum.Line)
                    {
                        for (int i = 1; i < ds.PointList.Count; i++)
                        {
                            if (!ds.IsY2Data)
                            {
                                g.DrawLine(aPen, cs.Point2D((PointF)ds.PointList[i - 1], xa, ya),
                                                    cs.Point2D((PointF)ds.PointList[i], xa, ya));
                            }
                            else
                            {
                                g.DrawLine(aPen, cs.Point2DY2((PointF)ds.PointList[i - 1], xa, y2a),
                                                    cs.Point2DY2((PointF)ds.PointList[i], xa, y2a));
                            }
                        }
                    }
                    // Plot Spline:
                    else if (ds.LineChartType == LineChartTypeEnum.Spline)
                    {
                        PointF[] pts = new PointF[ds.PointList.Count];
                        for (int i = 0; i < pts.Length; i++)
                        {
                            if (!ds.IsY2Data)
                            {
                                pts[i] = cs.Point2D((PointF)(ds.PointList[i]), xa, ya);
                            }
                            else
                            {
                                pts[i] = cs.Point2DY2((PointF)(ds.PointList[i]), xa, y2a);
                            }
                        }
                        g.DrawCurve(aPen, pts);
                    }
                    #endregion
                    #region Stairstep
                    // Plot Stairstep:
                    else if (ds.LineChartType == LineChartTypeEnum.StairStep)
                    {
                        ArrayList aList = new ArrayList();
                        if (!ds.IsY2Data)
                        {
                            // Create Stairstep data:
                            for (int i = 0; i < ds.PointList.Count - 1; i++)
                            {
                                pt = (PointF)ds.PointList[i];
                                pt0 = (PointF)ds.PointList[i + 1];
                                aList.Add(pt);
                                aList.Add(new PointF(pt0.X, pt.Y));
                            }
                            aList.Add(ds.PointList[ds.PointList.Count - 1]);
                            // Draw stairstep chart:
                            for (int i = 1; i < aList.Count; i++)
                            {

                                g.DrawLine(aPen, cs.Point2D((PointF)aList[i - 1], xa, ya),
                                                 cs.Point2D((PointF)aList[i], xa, ya));
                            }
                        }
                        else 
                        {
                            for (int i = 0; i < ds.PointList.Count - 1; i++)
                            {
                                pt = (PointF)ds.PointList[i];
                                pt0 = (PointF)ds.PointList[i + 1];
                                aList.Add(pt);
                                aList.Add(new PointF(pt0.X, pt.Y));
                            }
                            aList.Add(ds.PointList[ds.PointList.Count - 1]);
                            for (int i = 1; i < aList.Count; i++)
                            {

                                g.DrawLine(aPen, cs.Point2DY2((PointF)aList[i - 1], xa, y2a),
                                                 cs.Point2DY2((PointF)aList[i], xa, y2a));
                            }
                        }
                    }
                    #endregion
                    #region Stem
                    // Plot Stems:
                    else if (ds.LineChartType == LineChartTypeEnum.Stem)
                    {
                        if (!ds.IsY2Data)
                        {
                            for (int i = 0; i < ds.PointList.Count; i++)
                            {
                                pt = (PointF)ds.PointList[i];
                                pt0 = new PointF(pt.X, 0);
                                g.DrawLine(aPen, cs.Point2D(pt0, xa, ya), cs.Point2D(pt, xa, ya));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ds.PointList.Count; i++)
                            {
                                pt = (PointF)ds.PointList[i];
                                pt0 = new PointF(pt.X, 0);
                                g.DrawLine(aPen, cs.Point2DY2(pt0, xa, y2a), cs.Point2DY2(pt, xa, y2a));
                            }
                        }

                    }
                    #endregion
                    #region ErrorBar
                    //Plot ErrorBar
                    else if (ds.LineChartType == LineChartTypeEnum.ErrorBar)
                    {
                        Pen errorPen = new Pen(ds.ErrorLineStyle.LineColor, ds.ErrorLineStyle.LineThickness);
                        errorPen.DashStyle = ds.ErrorLineStyle.LinePattern;
                        float barLength = 0;

                        if (!ds.IsY2Data)
                        {
                            // Draw lines:
                            for (int i = 1; i < ds.PointList.Count; i++)
                            {
                                pt0 = (PointF)ds.PointList[i - 1];
                                pt = (PointF)ds.PointList[i];
                                g.DrawLine(aPen, cs.Point2D(pt0, xa, ya), cs.Point2D(pt, xa, ya));
                                barLength = (pt.X - pt0.X) / 4;
                            }
                            // Draw error lines:
                            for (int i = 0; i < ds.ErrorList.Count; i++)
                            {
                                PointF errorPoint = (PointF)ds.ErrorList[i];
                                PointF linePoint = (PointF)ds.PointList[i];
                                pt0 = new PointF(linePoint.X, linePoint.Y - errorPoint.Y / 2);
                                pt = new PointF(linePoint.X, linePoint.Y + errorPoint.Y / 2);
                                g.DrawLine(errorPen, cs.Point2D(pt0, xa, ya), cs.Point2D(pt, xa, ya));
                                PointF pt1 = new PointF(pt0.X - barLength / 2, pt0.Y);
                                PointF pt2 = new PointF(pt0.X + barLength / 2, pt0.Y);
                                g.DrawLine(errorPen, cs.Point2D(pt1, xa, ya), cs.Point2D(pt2, xa, ya));
                                pt1 = new PointF(pt.X - barLength / 2, pt.Y);
                                pt2 = new PointF(pt.X + barLength / 2, pt.Y);
                                g.DrawLine(errorPen, cs.Point2D(pt1, xa, ya), cs.Point2D(pt2, xa, ya));
                            }
                        }
                        else
                        {
                            for (int i = 1; i < ds.PointList.Count; i++)
                            {
                                pt0 = (PointF)ds.PointList[i - 1];
                                pt = (PointF)ds.PointList[i];
                                g.DrawLine(aPen, cs.Point2DY2(pt0, xa, y2a), cs.Point2DY2(pt, xa, y2a));
                                barLength = (pt.X - pt0.X) / 4;
                            }
                            
                            for (int i = 0; i < ds.ErrorList.Count; i++)
                            {
                                PointF errorPoint = (PointF)ds.ErrorList[i];
                                PointF linePoint = (PointF)ds.PointList[i];
                                pt0 = new PointF(linePoint.X, linePoint.Y - errorPoint.Y / 2);
                                pt = new PointF(linePoint.X, linePoint.Y + errorPoint.Y / 2);
                                g.DrawLine(errorPen, cs.Point2DY2(pt0, xa, y2a), cs.Point2DY2(pt, xa, y2a));
                                PointF pt1 = new PointF(pt0.X - barLength / 2, pt0.Y);
                                PointF pt2 = new PointF(pt0.X + barLength / 2, pt0.Y);
                                g.DrawLine(errorPen, cs.Point2DY2(pt1, xa, y2a), cs.Point2DY2(pt2, xa, y2a));
                                pt1 = new PointF(pt.X - barLength / 2, pt.Y);
                                pt2 = new PointF(pt.X + barLength / 2, pt.Y);
                                g.DrawLine(errorPen, cs.Point2DY2(pt1, xa, y2a), cs.Point2DY2(pt2, xa, y2a));
                            }
                        }
                        errorPen.Dispose();
                    }
                    #endregion
                    #region Stock
                    else if((ds.LineChartType == LineChartTypeEnum.HiLo || ds.LineChartType == LineChartTypeEnum.HiLoOpenClose || ds.LineChartType == LineChartTypeEnum.Candle)&& ds.DataString != null)
                    {
                        SolidBrush aBrush = new SolidBrush(ds.LineStyle.LineColor);
                        SolidBrush whiteBrush = new SolidBrush(Color.White);
                        float barLength = cs.PlotArea.Width / (5 * ds.DataString.GetLength(1));
                        for (int i = 0; i < ds.DataString.GetLength(1); i++)
                        {
                            float[] stockdata = new float[4];
                            for (int j = 0; j < stockdata.Length; j++)
                            {
                                stockdata[j] = Convert.ToSingle(ds.DataString[j + 1, i]);
                            }
                            PointF ptHigh, ptLow, ptOpen, ptCLose;
                            if (!ds.IsY2Data)
                            {
                                ptHigh = cs.Point2D(new PointF(i, stockdata[1]), xa, ya);
                                ptLow = cs.Point2D(new PointF(i, stockdata[2]), xa, ya);
                                ptOpen = cs.Point2D(new PointF(i, stockdata[0]), xa, ya);
                                ptCLose = cs.Point2D(new PointF(i, stockdata[3]), xa, ya);
                            }
                            else
                            {
                                ptHigh = cs.Point2DY2(new PointF(i, stockdata[1]), xa, y2a);
                                ptLow = cs.Point2DY2(new PointF(i, stockdata[2]), xa, y2a);
                                ptOpen = cs.Point2DY2(new PointF(i, stockdata[0]), xa, y2a);
                                ptCLose = cs.Point2DY2(new PointF(i, stockdata[3]), xa, y2a);
                            }
                            PointF ptOpen1 = new PointF(ptOpen.X - barLength, ptOpen.Y);
                            PointF ptClose1 = new PointF(ptCLose.X + barLength, ptCLose.Y);
                            PointF ptOpen2 = new PointF(ptOpen.X + barLength, ptOpen.Y);
                            PointF ptClose2 = new PointF(ptCLose.X - barLength, ptCLose.Y);


                            // Draw Hi-Lo stock chart:
                            if (ds.LineChartType == LineChartTypeEnum.HiLo)
                            {
                                g.DrawLine(aPen, ptHigh, ptLow);
                            }

                            // Draw Hi-Li-Open-Close chart:
                            else if (ds.LineChartType == LineChartTypeEnum.HiLoOpenClose)
                            {
                                g.DrawLine(aPen, ptHigh, ptLow);
                                g.DrawLine(aPen, ptOpen, ptOpen1);
                                g.DrawLine(aPen, ptCLose, ptClose1);
                            }

                            // Draw candle chart:
                            else if (ds.LineChartType == LineChartTypeEnum.Candle)
                            {
                                PointF[] pts = new PointF[4];
                                pts[0] = ptOpen1;
                                pts[1] = ptOpen2;
                                pts[2] = ptClose1;
                                pts[3] = ptClose2;
                                g.DrawLine(aPen, ptHigh, ptLow);
                                if (stockdata[0] > stockdata[3])
                                {
                                    g.FillPolygon(aBrush, pts);
                                }
                                else if (stockdata[0] < stockdata[3])
                                {
                                    g.FillPolygon(whiteBrush, pts);
                                }
                                g.DrawPolygon(aPen, pts);
                            }
                        }
                        aBrush.Dispose();
                        whiteBrush.Dispose();
 
                    }
                    #endregion
                    aPen.Dispose();
                }
            }
                   
            
            // Plot Symbols:
            foreach (DataSeries ds in dc.DataSeriesList)
            {
                if (ds.LineChartType != LineChartTypeEnum.HiLo && ds.LineChartType != LineChartTypeEnum.HiLoOpenClose && ds.LineChartType != LineChartTypeEnum.Candle)
                    for (int i = 0; i < ds.PointList.Count; i++)
                    {
                        PointF pt = (PointF)ds.PointList[i];
                        if (!ds.IsY2Data)
                        {
                            if (pt.X >= xa.XLimMin && pt.X <= xa.XLimMax &&
                                pt.Y >= ya.YLimMin && pt.Y <= ya.YLimMax)
                            {
                                ds.SymbolStyle.DrawSymbol(g, cs.Point2D((PointF)ds.PointList[i], xa, ya));
                            }
                        }
                        else
                        {
                            if (pt.X >= xa.XLimMin && pt.X <= xa.XLimMax &&
                                pt.Y >= y2a.Y2LimMin && pt.Y <= y2a.Y2LimMax)
                            {
                                ds.SymbolStyle.DrawSymbol(g, cs.Point2DY2((PointF)ds.PointList[i], xa, y2a));
                            }
                        }
                    }
            }
        }

    }
}
