using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Readearth.Chart2D.Data;

namespace Readearth.Chart2D
{
    public class PieCharts
    {
        public enum PieChartTypeEnum
        {
            [Description("标准饼图")]
            Standard = 0,
            [Description("部分饼图")]
            Partial = 1,
            [Description("环形图")]
            Doughnut = 2
        }

        internal static void AddPie(DataCollectionPie dc, Graphics g, ChartStyle cs)
        {
            SolidBrush aBrush = new SolidBrush(Color.Black);
            Pen aPen = new Pen(dc.BorderColor);
            int nData = dc.DataSeriesList.Count;
            float fSum = 0;
            for (int i = 0; i < nData; i++)
            {
                fSum = fSum + (float)((DataSeries)dc.DataSeriesList[i]).PointList[0];
            }
            float startAngle = 0;
            float sweepAngle = 0;

            for (int i = 0; i < nData; i++)
            {
                DataSeries ds = (DataSeries)(dc.DataSeriesList[i]);
                aBrush = new SolidBrush(ds.FillColor);
                int explode = (ds.IsExplode) ? cs.PlotArea.Width/10 : 0;

                if (fSum < 1 && dc.PieChartType == PieChartTypeEnum.Partial)
                {
                    startAngle = startAngle + sweepAngle;
                    sweepAngle = 360 * (float)ds.PointList[0];
                }
                else
                {
                    startAngle = startAngle + sweepAngle;
                    sweepAngle = 360 * (float)ds.PointList[0] / fSum;

                }

                int xshift = (int)(explode * Math.Cos((startAngle + sweepAngle / 2) * Math.PI / 180));
                int yshift = (int)(explode * Math.Sin((startAngle + sweepAngle / 2) * Math.PI / 180));

                Rectangle rect1 = new Rectangle(cs.polarArea.X + xshift, cs.polarArea.Y + yshift, cs.polarArea.Width, cs.polarArea.Height);
                g.FillPie(aBrush, rect1, startAngle, sweepAngle);
                if(dc.IsBorderVis)
                    g.DrawPie(aPen, rect1, startAngle, sweepAngle);
            }
        }

        //internal static void GetShiftSize(DataCollectionPie dc, ChartStyle cs, ref int[] MaxShifts)
        //{
        //    int xLeftShift, xRightShift, yTopShift, yBottomShift;
        //    xLeftShift = xRightShift = yTopShift = yBottomShift = 0;

        //    int n = dc.DataSeriesList.Count;
        //    float fSum = 0;
        //    for (int i = 0; i < n; i++)
        //    {
        //        fSum = fSum + (float)((DataSeries)dc.DataSeriesList[i]).PointList[0];
        //    }
        //    float startAngle = 0;
        //    float sweepAngle = 0;

        //    for (int i = 0; i < n; i++)
        //    {
        //        DataSeries ds = (DataSeries)(dc.DataSeriesList[i]);
        //        int explode = (ds.IsExplode) ? cs.PlotArea.Width / 10 : 0;

        //        if (fSum < 1 && dc.PieChartType == PieChartTypeEnum.Partial)
        //        {
        //            startAngle = startAngle + sweepAngle;
        //            sweepAngle = 360 * (float)ds.PointList[0];
        //        }
        //        else
        //        {
        //            startAngle = startAngle + sweepAngle;
        //            sweepAngle = 360 * (float)ds.PointList[0] / fSum;

        //        }
        //        int xshift = (int)(explode * Math.Cos((startAngle + sweepAngle / 2) * Math.PI / 180));
        //        int yshift = (int)(explode * Math.Sin((startAngle + sweepAngle / 2) * Math.PI / 180));

        //        xLeftShift = (xshift < xLeftShift) ? xshift : xLeftShift;
        //        xRightShift = (xshift > xRightShift) ? xshift : xRightShift;
        //        yTopShift = (yshift < yTopShift) ? yshift : yTopShift;
        //        yBottomShift = (yshift > yBottomShift) ? yshift : yBottomShift;
        //    }
        //    MaxShifts[0] = xLeftShift; 
        //    MaxShifts[1] = xRightShift;
        //    MaxShifts[2] = yTopShift;
        //    MaxShifts[3] = yBottomShift;
        //}
    }
}
