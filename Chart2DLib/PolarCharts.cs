using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Readearth.Chart2D.Data;
using Readearth.Chart2D.ChartElements;

namespace Readearth.Chart2D
{
    public class PolarCharts
    {
        public enum PolarChartTypeEnum
        {
            Radar = 0,
            Spline = 1,
            RadarPolygon = 2,
            Rose = 3,
        }

        public enum AngleDirectionEnum
        {
            [Description("逆时针")]
            CounterClockWise = 0,
            [Description("顺时针")]
            ClockWise = 1
        }

        internal static void AddPolar(DataCollectionPolar dc, Graphics g, ChartStyle cs, RAxis ra)
        {
            if (!dc.CheckNegative())
                return;
            float xc = cs.polarArea.X + cs.polarArea.Width / 2;
            float yc = cs.polarArea.Y + cs.polarArea.Height / 2;
            Pen aPen = null;
            SolidBrush aBrush = null;
            // Plot lines:
            //雷达图
            if (dc.PolarChartType == PolarChartTypeEnum.Radar || dc.PolarChartType == PolarChartTypeEnum.RadarPolygon)
            {
                foreach (DataSeries ds in dc.DataSeriesList)
                {
                    aBrush = new SolidBrush(ds.FillColor);
                    aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.LineThickness);
                    aPen.DashStyle = ds.LineStyle.LinePattern;
                    PointF[] pts = new PointF[ds.PointList.Count];
                    for (int i = 0; i < ds.PointList.Count; i++)
                    {
                        pts[i] = cs.Point2DR((PointF)ds.PointList[i], ra);
                    }

                    if (ds.LineStyle.IsVisible == true)
                        g.DrawPolygon(aPen, pts);
                    if(dc.PolarChartType == PolarChartTypeEnum.RadarPolygon)
                        g.FillPolygon(aBrush, pts);
                }
            }
            //玫瑰图
            else if (dc.PolarChartType == PolarChartTypeEnum.Rose)
            {
                Dictionary<float, float> rSum = new Dictionary<float, float>();
                for (int a = 0; a < 360 / ra.AngleStep; a++)
                {
                    rSum.Add(a * ra.AngleStep, 0);
                }
                foreach (DataSeries ds in dc.DataSeriesList)
                {
                    foreach (PointF p in ds.PointList)
                    {
                        if (p.Y < 0)
                            return;
                        if (rSum.ContainsKey(p.X))
                            rSum[p.X] += p.Y;
                        else
                            return ;
                    }
                }
                for (int i = dc.DataSeriesList.Count - 1; i >= 0; i--)
                {
                    DataSeries ds = (DataSeries)dc.DataSeriesList[i];
                    aBrush = new SolidBrush(ds.FillColor);
                    aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.LineThickness);
                    aPen.DashStyle = ds.LineStyle.LinePattern;
                    //ds.LineStyle.IsVisible = false;

                    float sweepAngle = ra.AngleStep * dc.RoseBarWidth;
                    float pStartAngle = 0;
                    int pBarLeng = 0;
                    foreach (PointF p in ds.PointList)
                    {
                        pStartAngle = p.X + ra.StartAngle - sweepAngle / 2;
                        pBarLeng = (int)Math.Ceiling(cs.RNorm(rSum[p.X],ra));
                        Rectangle rect1 = new Rectangle((int)xc - pBarLeng, (int)yc - pBarLeng, pBarLeng*2, pBarLeng*2);
                        g.FillPie(aBrush, rect1, pStartAngle, sweepAngle);
                        if(ds.LineStyle.IsVisible)
                            g.DrawPie(aPen, rect1, pStartAngle, sweepAngle);
                        rSum[p.X] -= p.Y;
                    }
                }
            }
            //曲线图
            else if (dc.PolarChartType == PolarChartTypeEnum.Spline)
            {
                foreach (DataSeries ds in dc.DataSeriesList)
                {
                    aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.LineThickness);
                    aPen.DashStyle = ds.LineStyle.LinePattern;
                    PointF[] pts = new PointF[ds.PointList.Count];

                    if (ds.LineStyle.IsVisible == true)
                    {
                        for (int i = 0; i < ds.PointList.Count; i++)
                        {
                            pts[i] = cs.Point2DR((PointF)ds.PointList[i], ra);
                        }
                        g.DrawClosedCurve(aPen, pts);
                    }
                }
            }

            // Plot Symbols:
            if (dc.PolarChartType == PolarChartTypeEnum.Radar || dc.PolarChartType == PolarChartTypeEnum.Spline)
            {
                foreach (DataSeries ds in dc.DataSeriesList)
                {
                    for (int i = 0; i < ds.PointList.Count; i++)
                    {
                        PointF pt = cs.Point2DR((PointF)ds.PointList[i], ra);
                        if(!pt.IsEmpty)
                            ds.SymbolStyle.DrawSymbol(g, pt);
                    }
                }
            }
            if(aPen != null)
                aPen.Dispose();
            if (aBrush != null)
                aBrush.Dispose();
        }

    }
}
