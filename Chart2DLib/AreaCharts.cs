using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Readearth.Chart2D.ChartElements;
using Readearth.Chart2D.Data;

namespace Readearth.Chart2D
{
    public class AreaCharts
    {
        public enum AreaChartTypeEnum
        {
            [Description("堆积面积图")]
            Area = 0,
            [Description("百分比面积图")]
            PercentArea = 1,//待解决
        }

        internal static void AddAreas(DataCollectionArea dc, Graphics g, ChartStyle cs, XAxis xa, YAxis ya, Y2Axis y2a)
        {
            if (dc.ChartType != Chart2DTypeEnum.AreaChart || dc.DataSeriesList.Count == 0)
                return;

            //比较各列点数，面积图需统一点数(暂不处理，待优化：以最少点数为准)
            int nPoints = 0;
            bool isConsistant = true;
            bool isY2 = false;
            foreach (DataSeries ds in dc.DataSeriesList)
            {
                if (nPoints == 0)
                    nPoints = ds.PointList.Count;
                else if (nPoints != ds.PointList.Count)
                    isConsistant = false;
                if (ds.IsY2Data)
                    isY2 = true;
            }
            if (!isConsistant)
                return;

            float[] ySum = new float[nPoints];
            PointF[] pts = new PointF[2 * nPoints];
            PointF[] pt0 = new PointF[nPoints];
            PointF[] pt1 = new PointF[nPoints];
            for (int i = 0; i < nPoints; i++)
            {
                ySum[i] = dc.AreaAxis;
            }

            int n = 0;
            Pen aPen = new Pen(Color.Black);
            SolidBrush aBrush = new SolidBrush(Color.Black);
            foreach (DataSeries ds in dc.DataSeriesList)
            {
                //Color fillColor = Color.FromArgb(dc.CMap[n, 0], dc.CMap[n, 1], dc.CMap[n, 2], dc.CMap[n, 3]);
                if(ds.FillColor != Color.Empty)
                    aBrush = new SolidBrush(ds.FillColor);
                else
                    aBrush = new SolidBrush(Color.FromArgb(180, ds.LineStyle.LineColor));
                aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.LineThickness);
                aPen.DashStyle = ds.LineStyle.LinePattern;

                // Draw lines and areas:
                if (dc.AreaChartType == AreaChartTypeEnum.Area)
                {
                    if (!isY2)
                        for (int i = 0; i < nPoints; i++)
                        {
                            pt0[i] = new PointF(((PointF)ds.PointList[i]).X, ySum[i]);
                            ySum[i] = ySum[i] + ((PointF)ds.PointList[i]).Y;
                            pt1[i] = new PointF(((PointF)ds.PointList[i]).X, ySum[i]);
                            pts[i] = cs.Point2D(pt0[i], xa, ya);
                            pts[2 * nPoints - 1 - i] = cs.Point2D(pt1[i], xa, ya);
                        }
                    else
                        for (int i = 0; i < nPoints; i++)
                        {
                            pt0[i] = new PointF(((PointF)ds.PointList[i]).X, ySum[i]);
                            ySum[i] = ySum[i] + ((PointF)ds.PointList[i]).Y;
                            pt1[i] = new PointF(((PointF)ds.PointList[i]).X, ySum[i]);
                            pts[i] = cs.Point2DY2(pt0[i], xa, y2a);
                            pts[2 * nPoints - 1 - i] = cs.Point2DY2(pt1[i], xa, y2a);
                        }
                    g.FillPolygon(aBrush, pts);
                    if (ds.LineStyle.IsVisible)
                        g.DrawPolygon(aPen, pts);
                    n++;
                }
            }
            aPen.Dispose();
            aBrush.Dispose();
        }
    }
}
