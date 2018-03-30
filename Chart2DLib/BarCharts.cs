using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using Readearth.Chart2D.ChartElements;
using Readearth.Chart2D.Data;

namespace Readearth.Chart2D
{
    public class BarCharts
    {
        public enum BarChartTypeEnum
        {
            [Description("柱状图")]
            Vertical = 0,
            [Description("条形图")]
            Horizontal = 1,
            [Description("堆积柱状图")]
            VerticalStack = 2, 
            [Description("堆积条形图")]
            HorizontalStack = 3,
            [Description("嵌套柱状图")]
            VerticalOverlay = 4, 
            [Description("嵌套条形图")]
            HorizontalOverlay = 5
        }

        /// <summary>
        /// 绘制矩形图，根据BarChartType和ds数量确定Bar绘制方式。
        /// 当BarChartType为Vertical或Horizontal，且ds数量大于1时，为多组柱图；其余情况为单柱图
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="g"></param>
        /// <param name="cs"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        internal static void AddBars(DataCollectionBar dc, Graphics g, ChartStyle cs, XAxis xa, YAxis ya, Y2Axis y2a)
        {
            int numberOfDataSeries = dc.DataSeriesList.Count;
            //比较各列个数，柱状图需统一个数，以最多个数为准
            int nPoints = 0;
            bool isConsistant = true;
            bool isY2 = false;
            foreach (DataSeries ds in dc.DataSeriesList)
            {
                if(nPoints < ds.PointList.Count)
                    nPoints = ds.PointList.Count;
                if (ds.IsY2Data)
                    isY2 = true;
            }

            // Draw bars:
            ArrayList temp = new ArrayList();
            float[] tempy = new float[nPoints];
            PointF temppt = new PointF();
            int n = 0;
            foreach (DataSeries ds in dc.DataSeriesList)
            {
                Pen aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.LineThickness);
                aPen.DashStyle = ds.LineStyle.LinePattern;
                SolidBrush aBrush; 
                if (ds.FillColor != Color.Empty)
                    aBrush = new SolidBrush(ds.FillColor);
                else
                    aBrush = new SolidBrush(Color.FromArgb(180, ds.LineStyle.LineColor));
                PointF[] pts = new PointF[4];
                PointF pt;
                float width;
                if (dc.BarChartType == BarChartTypeEnum.Vertical)
                {
                    if (numberOfDataSeries == 1)
                    {
                        width = (float)xa.XTick * dc.BarWidth;
                        if (!isY2)
                            for (int i = 0; i < ds.PointList.Count; i++)
                            {
                                pt = (PointF)ds.PointList[i];
                                float x = pt.X - xa.XTick / 2;
                                pts[0] = cs.Point2D(new PointF(x - width / 2, 0), xa, ya);
                                pts[1] = cs.Point2D(new PointF(x + width / 2, 0), xa, ya);
                                pts[2] = cs.Point2D(new PointF(x + width / 2, pt.Y), xa, ya);
                                pts[3] = cs.Point2D(new PointF(x - width / 2, pt.Y), xa, ya);
                                g.FillPolygon(aBrush, pts);
                                if (ds.LineStyle.IsVisible)
                                    g.DrawPolygon(aPen, pts);
                            }
                        else
                            for (int i = 0; i < ds.PointList.Count; i++)
                            {
                                pt = (PointF)ds.PointList[i];
                                float x = pt.X - xa.XTick / 2;
                                pts[0] = cs.Point2DY2(new PointF(x - width / 2, 0), xa, y2a);
                                pts[1] = cs.Point2DY2(new PointF(x + width / 2, 0), xa, y2a);
                                pts[2] = cs.Point2DY2(new PointF(x + width / 2, pt.Y), xa, y2a);
                                pts[3] = cs.Point2DY2(new PointF(x - width / 2, pt.Y), xa, y2a);
                                g.FillPolygon(aBrush, pts); 
                                if (ds.LineStyle.IsVisible)
                                    g.DrawPolygon(aPen, pts);
                            }
                    }
                    else if (numberOfDataSeries > 1)
                    {
                        //width = 0.7f * (float)xa.XTick;
                        dc.BarWidth = (dc.BarWidth > 0 && dc.BarWidth <= 1) ? dc.BarWidth : 0.95f;
                        width = (float)xa.XTick * dc.BarWidth;

                        if (!isY2)
                            for (int i = 0; i < ds.PointList.Count; i++)
                            {
                                pt = (PointF)ds.PointList[i];
                                float x = pt.X;// - (float)xa.XTick / 2
                                float w1 = width / numberOfDataSeries;
                                float w = dc.BarWidth * w1;
                                float space = (w1 - w) / 2;
                                pts[0] = cs.Point2D(new PointF(x - width / 2 + space + n * w1, 0), xa, ya);
                                pts[1] = cs.Point2D(new PointF(x - width / 2 + space + n * w1 + w, 0), xa, ya);
                                pts[2] = cs.Point2D(new PointF(x - width / 2 + space + n * w1 + w, pt.Y), xa, ya);
                                pts[3] = cs.Point2D(new PointF(x - width / 2 + space + n * w1, pt.Y), xa, ya);
                                g.FillPolygon(aBrush, pts);
                                if (ds.LineStyle.IsVisible)
                                    g.DrawPolygon(aPen, pts);
                            }
                        else
                            for (int i = 0; i < ds.PointList.Count; i++)
                            {
                                pt = (PointF)ds.PointList[i];
                                float x = pt.X - (float)xa.XTick / 2;
                                float w1 = width / numberOfDataSeries;
                                float w = dc.BarWidth * w1;
                                float space = (w1 - w) / 2;
                                pts[0] = cs.Point2DY2(new PointF(x - width / 2 + space + n * w1, 0), xa, y2a);
                                pts[1] = cs.Point2DY2(new PointF(x - width / 2 + space + n * w1 + w, 0), xa, y2a);
                                pts[2] = cs.Point2DY2(new PointF(x - width / 2 + space + n * w1 + w, pt.Y), xa, y2a);
                                pts[3] = cs.Point2DY2(new PointF(x - width / 2 + space + n * w1, pt.Y), xa, y2a);
                                g.FillPolygon(aBrush, pts);
                                if (ds.LineStyle.IsVisible)
                                    g.DrawPolygon(aPen, pts);
                            }
                    }
                }
                else if (dc.BarChartType == BarChartTypeEnum.VerticalOverlay  && numberOfDataSeries > 1)
                {
                    width = (float)xa.XTick * dc.BarWidth;
                    width = width / (float)Math.Pow(2, n);

                    if (!isY2)
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            pt = (PointF)ds.PointList[i];
                            float x = pt.X - xa.XTick / 2;
                            pts[0] = cs.Point2D(new PointF(x - width / 2, 0), xa, ya);
                            pts[1] = cs.Point2D(new PointF(x + width / 2, 0), xa, ya);
                            pts[2] = cs.Point2D(new PointF(x + width / 2, pt.Y), xa, ya);
                            pts[3] = cs.Point2D(new PointF(x - width / 2, pt.Y), xa, ya);
                            g.FillPolygon(aBrush, pts);
                            if (ds.LineStyle.IsVisible)
                                g.DrawPolygon(aPen, pts);
                        }
                    else
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            pt = (PointF)ds.PointList[i];
                            float x = pt.X - xa.XTick / 2;
                            pts[0] = cs.Point2DY2(new PointF(x - width / 2, 0), xa, y2a);
                            pts[1] = cs.Point2DY2(new PointF(x + width / 2, 0), xa, y2a);
                            pts[2] = cs.Point2DY2(new PointF(x + width / 2, pt.Y), xa, y2a);
                            pts[3] = cs.Point2DY2(new PointF(x - width / 2, pt.Y), xa, y2a);
                            g.FillPolygon(aBrush, pts);
                            if (ds.LineStyle.IsVisible)
                                g.DrawPolygon(aPen, pts);
                        }
                }
                else if (dc.BarChartType == BarChartTypeEnum.VerticalStack && numberOfDataSeries > 1)
                {
                    width = xa.XTick * dc.BarWidth;

                    if (!isY2)
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            pt = (PointF)ds.PointList[i];
                            if (temp.Count > 0)
                            {
                                tempy[i] = tempy[i] + ((PointF)temp[i]).Y;
                            }
                            float x = pt.X - xa.XTick / 2;
                            pts[0] = cs.Point2D(new PointF(x - width / 2, 0 + tempy[i]), xa, ya);
                            pts[1] = cs.Point2D(new PointF(x + width / 2, 0 + tempy[i]), xa, ya);
                            pts[2] = cs.Point2D(new PointF(x + width / 2, pt.Y + tempy[i]), xa, ya);
                            pts[3] = cs.Point2D(new PointF(x - width / 2, pt.Y + tempy[i]), xa, ya);

                            g.FillPolygon(aBrush, pts);
                            if (ds.LineStyle.IsVisible)
                                g.DrawPolygon(aPen, pts);
                        }
                    else
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            pt = (PointF)ds.PointList[i];
                            if (temp.Count > 0)
                            {
                                tempy[i] = tempy[i] + ((PointF)temp[i]).Y;
                            }
                            float x = pt.X - xa.XTick / 2;
                            pts[0] = cs.Point2DY2(new PointF(x - width / 2, 0 + tempy[i]), xa, y2a);
                            pts[1] = cs.Point2DY2(new PointF(x + width / 2, 0 + tempy[i]), xa, y2a);
                            pts[2] = cs.Point2DY2(new PointF(x + width / 2, pt.Y + tempy[i]), xa, y2a);
                            pts[3] = cs.Point2DY2(new PointF(x - width / 2, pt.Y + tempy[i]), xa, y2a);

                            g.FillPolygon(aBrush, pts);
                            if (ds.LineStyle.IsVisible)
                                g.DrawPolygon(aPen, pts);
                        }
                    temp = ds.PointList;
                }

                else if (dc.BarChartType == BarChartTypeEnum.Horizontal)
                {
                    if (numberOfDataSeries == 1)
                    {
                        if (!isY2)
                        {
                            width = ya.YTick * dc.BarWidth;
                            for (int i = 0; i < ds.PointList.Count; i++)
                            {
                                temppt = (PointF)ds.PointList[i];
                                pt = new PointF(temppt.Y, temppt.X);
                                float y = pt.Y - ya.YTick / 2;
                                pts[0] = cs.Point2D(new PointF(0, y - width / 2), xa, ya);
                                pts[1] = cs.Point2D(new PointF(0, y + width / 2), xa, ya);
                                pts[2] = cs.Point2D(new PointF(pt.X, y + width / 2), xa, ya);
                                pts[3] = cs.Point2D(new PointF(pt.X, y - width / 2), xa, ya);
                                g.FillPolygon(aBrush, pts);
                                if (ds.LineStyle.IsVisible)
                                    g.DrawPolygon(aPen, pts);
                            }
                        }
                        else
                        {
                            width = y2a.Y2Tick * dc.BarWidth;
                            for (int i = 0; i < ds.PointList.Count; i++)
                            {
                                temppt = (PointF)ds.PointList[i];
                                pt = new PointF(temppt.Y, temppt.X);
                                float y = pt.Y - y2a.Y2Tick / 2;
                                pts[0] = cs.Point2DY2(new PointF(0, y - width / 2), xa, y2a);
                                pts[1] = cs.Point2DY2(new PointF(0, y + width / 2), xa, y2a);
                                pts[2] = cs.Point2DY2(new PointF(pt.X, y + width / 2), xa, y2a);
                                pts[3] = cs.Point2DY2(new PointF(pt.X, y - width / 2), xa, y2a);
                                g.FillPolygon(aBrush, pts);
                                if (ds.LineStyle.IsVisible)
                                    g.DrawPolygon(aPen, pts);
                            }
                        }
                    }
                    else if (numberOfDataSeries > 1)
                    {
                        //width = 0.7f * ya.YTick;
                        dc.BarWidth = (dc.BarWidth > 0 && dc.BarWidth <= 1) ? dc.BarWidth : 0.95f;

                        if (!isY2)
                        {
                            width = (float)ya.YTick * dc.BarWidth;
                            for (int i = 0; i < ds.PointList.Count; i++)
                            {
                                temppt = (PointF)ds.PointList[i];
                                pt = new PointF(temppt.Y, temppt.X);
                                float w1 = width / numberOfDataSeries;
                                float w = dc.BarWidth * w1;
                                float space = (w1 - w) / 2;
                                float y = pt.Y - ya.YTick / 2;
                                pts[0] = cs.Point2D(new PointF(0,
                                    y - width / 2 + space + n * w1), xa, ya);
                                pts[1] = cs.Point2D(new PointF(0,
                                    y - width / 2 + space + n * w1 + w), xa, ya);
                                pts[2] = cs.Point2D(new PointF(pt.X,
                                    y - width / 2 + space + n * w1 + w), xa, ya);
                                pts[3] = cs.Point2D(new PointF(pt.X,
                                    y - width / 2 + space + n * w1), xa, ya);
                                g.FillPolygon(aBrush, pts);
                                if (ds.LineStyle.IsVisible)
                                    g.DrawPolygon(aPen, pts);
                            }
                        }
                        else
                        {
                            width = (float)y2a.Y2Tick * dc.BarWidth;
                            for (int i = 0; i < ds.PointList.Count; i++)
                            {
                                temppt = (PointF)ds.PointList[i];
                                pt = new PointF(temppt.Y, temppt.X);
                                float w1 = width / numberOfDataSeries;
                                float w = dc.BarWidth * w1;
                                float space = (w1 - w) / 2;
                                float y = pt.Y - y2a.Y2Tick / 2;
                                pts[0] = cs.Point2DY2(new PointF(0,
                                    y - width / 2 + space + n * w1), xa, y2a);
                                pts[1] = cs.Point2DY2(new PointF(0,
                                    y - width / 2 + space + n * w1 + w), xa, y2a);
                                pts[2] = cs.Point2DY2(new PointF(pt.X,
                                    y - width / 2 + space + n * w1 + w), xa, y2a);
                                pts[3] = cs.Point2DY2(new PointF(pt.X,
                                    y - width / 2 + space + n * w1), xa, y2a);
                                g.FillPolygon(aBrush, pts);
                                if (ds.LineStyle.IsVisible)
                                    g.DrawPolygon(aPen, pts);
                            }
 
                        }
                    }
                }
                else if (dc.BarChartType == BarChartTypeEnum.HorizontalOverlay &&  numberOfDataSeries > 1)
                {
                    if (!isY2)
                    {
                        width = ya.YTick * dc.BarWidth;
                        width = width / (float)Math.Pow(2, n);
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            temppt = (PointF)ds.PointList[i];
                            pt = new PointF(temppt.Y, temppt.X);
                            float y = pt.Y - ya.YTick / 2;
                            pts[0] = cs.Point2D(new PointF(0, y - width / 2), xa, ya);
                            pts[1] = cs.Point2D(new PointF(0, y + width / 2), xa, ya);
                            pts[2] = cs.Point2D(new PointF(pt.X, y + width / 2), xa, ya);
                            pts[3] = cs.Point2D(new PointF(pt.X, y - width / 2), xa, ya);
                            g.FillPolygon(aBrush, pts);
                            if (ds.LineStyle.IsVisible)
                                g.DrawPolygon(aPen, pts);
                        }
                    }
                    else
                    {
                        width = y2a.Y2Tick * dc.BarWidth;
                        width = width / (float)Math.Pow(2, n);
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            temppt = (PointF)ds.PointList[i];
                            pt = new PointF(temppt.Y, temppt.X);
                            float y = pt.Y - y2a.Y2Tick / 2;
                            pts[0] = cs.Point2DY2(new PointF(0, y - width / 2), xa, y2a);
                            pts[1] = cs.Point2DY2(new PointF(0, y + width / 2), xa, y2a);
                            pts[2] = cs.Point2DY2(new PointF(pt.X, y + width / 2), xa, y2a);
                            pts[3] = cs.Point2DY2(new PointF(pt.X, y - width / 2), xa, y2a);
                            g.FillPolygon(aBrush, pts);
                            if (ds.LineStyle.IsVisible)
                                g.DrawPolygon(aPen, pts);
                        }
                    }
                }
                else if (dc.BarChartType == BarChartTypeEnum.HorizontalStack && numberOfDataSeries > 1)
                {
                    if (!isY2)
                    {
                        width = ya.YTick * dc.BarWidth;
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            temppt = (PointF)ds.PointList[i];
                            pt = new PointF(temppt.Y, temppt.X);
                            if (temp.Count > 0)
                            {
                                temppt = (PointF)temp[i];
                                tempy[i] = tempy[i] + temppt.Y;
                            }
                            float y = pt.Y - ya.YTick / 2;
                            pts[0] = cs.Point2D(new PointF(0 + tempy[i], y - width / 2), xa, ya);
                            pts[1] = cs.Point2D(new PointF(0 + tempy[i], y + width / 2), xa, ya);
                            pts[2] = cs.Point2D(new PointF(pt.X + tempy[i], y + width / 2), xa, ya);
                            pts[3] = cs.Point2D(new PointF(pt.X + tempy[i], y - width / 2), xa, ya);

                            g.FillPolygon(aBrush, pts);
                            if (ds.LineStyle.IsVisible)
                                g.DrawPolygon(aPen, pts);
                        }
                    }
                    else
                    {
                        width = y2a.Y2Tick * dc.BarWidth;
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            temppt = (PointF)ds.PointList[i];
                            pt = new PointF(temppt.Y, temppt.X);
                            if (temp.Count > 0)
                            {
                                temppt = (PointF)temp[i];
                                tempy[i] = tempy[i] + temppt.Y;
                            }
                            float y = pt.Y - y2a.Y2Tick / 2;
                            pts[0] = cs.Point2DY2(new PointF(0 + tempy[i], y - width / 2), xa, y2a);
                            pts[1] = cs.Point2DY2(new PointF(0 + tempy[i], y + width / 2), xa, y2a);
                            pts[2] = cs.Point2DY2(new PointF(pt.X + tempy[i], y + width / 2), xa, y2a);
                            pts[3] = cs.Point2DY2(new PointF(pt.X + tempy[i], y - width / 2), xa, y2a);

                            g.FillPolygon(aBrush, pts);
                            if (ds.LineStyle.IsVisible)
                                g.DrawPolygon(aPen, pts);
                        }
                    }
                    temp = ds.PointList;
                }
                n++;
                aPen.Dispose();
                aBrush.Dispose();
            }
        }
        
    }
}
