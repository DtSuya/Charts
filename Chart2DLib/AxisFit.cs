using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Readearth.Chart2D.ChartElements;
using Readearth.Chart2D.Data;

namespace Readearth.Chart2D
{
    internal class AxisFit
    {
        /// <summary>
        /// x轴标记角度自适应
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cs"></param>
        /// <param name="lb"></param>
        /// <param name="xa"></param>
        internal static void SetTickAngle(Graphics g, ChartStyle cs, XYLabel lb, XAxis xa)
        {
            SizeF sizeXTick;
            float maxlength = 0;float maxheight = 0;
            for (int n = 0; n < xa.XTickMarkFull.Length; n++)
            {
                sizeXTick = g.MeasureString(xa.XTickMarkFull[n], xa.XTickStyle.TextFont);
                maxlength = (sizeXTick.Width > maxlength) ? sizeXTick.Width : maxlength;
                maxheight = (sizeXTick.Height > maxheight)? sizeXTick.Height:maxheight;
            }

            float xticklength = xa.XTick * cs.PlotArea.Width / (xa.XLimMax - xa.XLimMin);
            xa.XTickAngle = (xa.XMarkInterval * xticklength > maxlength) ? 0 : ((xa.XMarkInterval * xticklength > maxlength / 2) ? -45 : -90);
        }

        /// <summary>
        /// 坐标轴检查，便于手动设定limit后不强制开启自适应
        /// </summary>
        /// <param name="dclist"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="y2a"></param>
        /// <returns></returns>
        internal static bool AxesFitCheck(List<DataCollection> dclist, XAxis xa, YAxis ya, Y2Axis y2a)
        {
            bool openFit, xopenfit, yopenfit, y2openfit;
            openFit = xopenfit = yopenfit = y2openfit = false;
            float xmax, xmin, ymax, ymin, y2max, y2min;
            xmax = xmin = ymax = ymin = y2max = y2min = -99;
            //含柱状图时,底轴两侧预留一个tick
            float barmin, barmax;
            barmin = barmax = -99;
            bool havebar = false;  bool baraxisX = true;
            try
            {
                foreach (DataCollection dc in dclist)
                {
                    #region 叠加型
                    #region AreaChart
                    if (dc.ChartType == Chart2DTypeEnum.AreaChart)
                    {
                        int nPoints = 0;
                        bool isConsistant = true;
                        bool isY2 = false;
                        foreach (DataSeries ds in dc.DataSeriesList)
                        {
                            if (nPoints == 0)
                                nPoints = ds.PointList.Count;
                            else if (nPoints != ds.PointList.Count)
                            {
                                isConsistant = false;
                                return openFit;
                            }
                            if (ds.IsY2Data)
                                isY2 = true;
                        }
                        try
                        {
                            DataSeries ds1 = (DataSeries)dc.DataSeriesList[0];
                            if(xmax == -99 && xmin ==-99)
                                xmax = xmin = ((PointF)ds1.PointList[0]).X;
                            if (!isY2 && ymax == -99 && ymin== -99)
                                ymax = ymin = ((PointF)ds1.PointList[0]).Y;
                            else if (isY2 && y2max == -99 && y2min == -99)
                                y2max = y2min = ((PointF)ds1.PointList[0]).Y; 
                        }
                        catch
                        {

                            if (xmax == -99 && xmin == -99)
                                xmax = xmin = 0;
                            if (ymax == ymin && ymax == -99)
                                ymax = ymin = 0;
                            if (y2max == y2min && y2max == -99)
                                y2max = y2min = 0;
                        }

                        if (((DataCollectionArea)dc).AreaChartType == AreaCharts.AreaChartTypeEnum.Area)
                        {
                            float[] ySum = new float[nPoints];
                            foreach (DataSeries ds in dc.DataSeriesList)
                            {
                                for(int i =0;i<nPoints;i++)
                                {
                                    PointF p = (PointF)ds.PointList[i];
                                    xmax = (p.X > xmax) ? p.X : xmax;
                                    xmin = (p.X < xmin) ? p.X : xmin;
                                    ySum[i] += p.Y;
                                }
                            }
                            if (!isY2)
                            {
                                foreach (float y in ySum)
                                {
                                    ymax = (y > ymax) ? y : ymax;
                                    ymin = (y < ymin) ? y : ymin;
                                }
                            }
                            else
                            {
                                foreach (float y in ySum)
                                {
                                    y2max = (y > y2max) ? y : y2max;
                                    y2min = (y < y2min) ? y : y2min;
                                } 
                            }
                        }
                        else
                        {
                            //纵轴表示百分比
                            float ytick = 0.2f;
                            if (!isY2)
                            {
                                ya.YLimMax = ymax = 1;
                                ya.YLimMin = ymin = 0;
                                ya.YTick = ytick;
                            }
                            else
                            {
                                y2a.Y2LimMax = ymax = 1;
                                y2a.Y2LimMin = ymin = 0;
                                y2a.Y2Tick = ytick;
                            }
                        }
                    }
                    #endregion

                    #region BarChart
                    else if (dc.ChartType == Chart2DTypeEnum.BarChart)
                    {
                        havebar = true;
                        int nPoints = 0;
                        bool isConsistant = true;
                        bool isY2 = false;
                        foreach (DataSeries ds in dc.DataSeriesList)
                        {
                            if (nPoints < ds.PointList.Count)
                                nPoints = ds.PointList.Count;
                            if (ds.IsY2Data)
                                isY2 = true;
                        }
                        try
                        {
                            DataSeries ds1 = (DataSeries)dc.DataSeriesList[0];
                            if (xmax == -99 && xmin == -99)
                                xmax = xmin = ((PointF)ds1.PointList[0]).X;
                            if (!isY2 && ymax == -99 && ymin == -99)
                                ymax = ymin = ((PointF)ds1.PointList[0]).Y;
                            else if(isY2 && y2max == -99 && y2min== -99)
                                y2max = y2min = ((PointF)ds1.PointList[0]).Y;

                            if (((DataCollectionBar)dc).BarChartType == BarCharts.BarChartTypeEnum.Vertical || ((DataCollectionBar)dc).BarChartType == BarCharts.BarChartTypeEnum.VerticalOverlay || ((DataCollectionBar)dc).BarChartType == BarCharts.BarChartTypeEnum.VerticalStack)
                                barmax = barmin = ((PointF)ds1.PointList[0]).X;
                            else
                            {
                                barmax = barmin = ((PointF)ds1.PointList[0]).Y;
                                baraxisX = false;
                            }
                        }
                        catch
                        {
                            if (xmax == -99 && xmin == -99)
                                xmax = xmin = 0;
                            if (ymax == ymin && ymax == -99)
                                ymax = ymin = 0;
                            if (y2max == y2min && y2max == -99)
                                y2max = y2min = 0;
                            barmax = barmin = 0;
                        }
                        //不同方向Bar影响不同轴的自适应情况
                        if (((DataCollectionBar)dc).BarChartType == BarCharts.BarChartTypeEnum.Vertical || ((DataCollectionBar)dc).BarChartType == BarCharts.BarChartTypeEnum.VerticalOverlay)
                        {
                            foreach (DataSeries ds in dc.DataSeriesList)
                            {
                                foreach (PointF p in ds.PointList)
                                {
                                    xmax = (p.X > xmax) ? p.X : xmax;
                                    xmin = (p.X < xmin) ? p.X : xmin;
                                    barmax = (p.X > barmax) ? p.X : barmax;
                                    barmin = (p.X < barmin) ? p.X : barmin;

                                    if (!ds.IsY2Data)
                                    {
                                        ymax = (p.Y > ymax) ? p.Y : ymax;
                                        ymin = (p.Y < ymin) ? p.Y : ymin;
                                    }
                                    else
                                    {
                                        y2max = (p.Y > y2max) ? p.Y : y2max;
                                        y2min = (p.Y < y2min) ? p.Y : y2min;
                                    }
                                }
                            }
                        }
                        else if (((DataCollectionBar)dc).BarChartType == BarCharts.BarChartTypeEnum.VerticalStack)
                        {
                            float[] ySum = new float[nPoints];
                            foreach (DataSeries ds in dc.DataSeriesList)
                            {
                                for (int i = 0; i < nPoints; i++)
                                {
                                    PointF p = (PointF)ds.PointList[i];
                                    xmax = (p.X > xmax) ? p.X : xmax;
                                    xmin = (p.X < xmin) ? p.X : xmin;

                                    barmax = (p.X > barmax) ? p.X : barmax;
                                    barmin = (p.X < barmin) ? p.X : barmin;

                                    ySum[i] += p.Y;
                                }
                            }
                            if (!isY2)
                            {
                                foreach (float y in ySum)
                                {
                                    ymax = (y > ymax) ? y : ymax;
                                    ymin = (y < ymin) ? y : ymin;
                                }
                            }
                            else
                            {
                                foreach (float y in ySum)
                                {
                                    y2max = (y > y2max) ? y : y2max;
                                    y2min = (y < y2min) ? y : y2min;
                                }
                            }
                        }
                        else if (((DataCollectionBar)dc).BarChartType == BarCharts.BarChartTypeEnum.Horizontal || ((DataCollectionBar)dc).BarChartType == BarCharts.BarChartTypeEnum.HorizontalOverlay)
                        {
                            foreach (DataSeries ds in dc.DataSeriesList)
                            {
                                foreach (PointF p in ds.PointList)
                                {
                                    xmax = (p.Y > xmax) ? p.Y : xmax;
                                    xmin = (p.Y < xmin) ? p.Y : xmin;
                                    ymax = (p.X > ymax) ? p.X : ymax;
                                    ymin = (p.X < ymin) ? p.X : ymin;

                                    barmax = (p.Y > barmax) ? p.Y : barmax;
                                    barmin = (p.Y < barmin) ? p.Y : barmin;
                                }
                            }
                        }
                        else if (((DataCollectionBar)dc).BarChartType == BarCharts.BarChartTypeEnum.HorizontalStack)
                        {
                            float[] xSum = new float[nPoints];
                            foreach (DataSeries ds in dc.DataSeriesList)
                            {
                                for (int i = 0; i < nPoints; i++)
                                {
                                    PointF p = (PointF)ds.PointList[i];
                                    xSum[i] += p.Y;
                                    ymax = (p.X > ymax) ? p.X : ymax;
                                    ymin = (p.X < ymin) ? p.X : ymin;

                                    barmax = (p.Y > barmax) ? p.Y : barmax;
                                    barmin = (p.Y < barmin) ? p.Y : barmin;
                                }
                            }
                            foreach (float x in xSum)
                            {
                                xmax = (x > xmax) ? x : xmax;
                                xmin = (x < xmin) ? x : xmin;
                            }
                        }
                    }
                    #endregion

                    #endregion
                    else
                    {
                        try
                        {
                            DataSeries ds1 = (DataSeries)dc.DataSeriesList[0];
                            if (xmax == -99 && xmin == -99)
                                xmax = xmin = ((PointF)ds1.PointList[0]).X;
                            foreach (DataSeries ds in dc.DataSeriesList)
                            {
                                if (ds.LineChartType == LineCharts.LineChartTypeEnum.Candle || ds.LineChartType == LineCharts.LineChartTypeEnum.HiLoOpenClose || ds.LineChartType == LineCharts.LineChartTypeEnum.HiLo)
                                {
                                    if (!ds.IsY2Data && ymax == ymin && ymax == -99)
                                        ymax = ymin = Convert.ToSingle(ds.DataString[2, 0]);
                                    else if (ds.IsY2Data && y2max == y2min && y2max == -99)
                                        y2max = y2min = Convert.ToSingle(ds.DataString[2, 0]);
                                }
                                else
                                {
                                    if (!ds.IsY2Data && ymax == ymin && ymax == -99)
                                        ymax = ymin = ((PointF)ds.PointList[0]).Y;
                                    else if (ds.IsY2Data && y2max == y2min && y2max == -99)
                                        y2max = y2min = ((PointF)ds.PointList[0]).Y;
                                }
                            }
                        }
                        catch
                        {
                            if (xmax == -99 && xmin == -99)
                                xmax = xmin = 0;
                            if( ymax == ymin && ymax == -99)
                                ymax = ymin = 0;
                            if (y2max == y2min && y2max == -99)
                                y2max = y2min = 0;
                        }
                        foreach (DataSeries ds in dc.DataSeriesList)
                        {
                            #region 附加型
                            if (ds.ErrorList.Count != 0 && ds.LineChartType == LineCharts.LineChartTypeEnum.ErrorBar)
                            {
                                for (int i = 0; i < ds.ErrorList.Count; i++)
                                {
                                    PointF p = (PointF)ds.PointList[i];
                                    PointF errp = (PointF)ds.ErrorList[i];
                                    xmax = (p.X > xmax) ? p.X : xmax;
                                    xmin = (p.X < xmin) ? p.X : xmin;
                                    if (!ds.IsY2Data)
                                    {
                                        ymax = (p.Y + errp.Y > ymax) ? p.Y + errp.Y : ymax;
                                        ymin = (p.Y - errp.Y < ymin) ? p.Y - errp.Y : ymin;
                                    }
                                    else
                                    {
                                        y2max = (p.Y + errp.Y > y2max) ? p.Y + errp.Y : y2max;
                                        y2min = (p.Y - errp.Y < y2min) ? p.Y - errp.Y : y2min;
                                    }
                                }
                            }
                            else if (ds.DataString != null && (ds.LineChartType == LineCharts.LineChartTypeEnum.Candle || ds.LineChartType == LineCharts.LineChartTypeEnum.HiLoOpenClose || ds.LineChartType == LineCharts.LineChartTypeEnum.HiLo))
                            {
                                xmax = (ds.DataString.GetLength(1) > xmax) ? ds.DataString.GetLength(1) : xmax;
                                xmin = (0 < xmin) ? 0 : xmin;
                                for (int i = 0; i < ds.DataString.GetLength(1); i++)
                                {
                                    float ptHigh = Convert.ToSingle(ds.DataString[2, i]);
                                    float ptLow = Convert.ToSingle(ds.DataString[3, i]);
                                    if (!ds.IsY2Data)
                                    {
                                        ymax = (ptHigh > ymax) ? ptHigh : ymax;
                                        ymin = (ptLow < ymin) ? ptLow : ymin;
                                    }
                                    else
                                    {
                                        y2max = (ptHigh > y2max) ? ptHigh : y2max;
                                        y2min = (ptLow < y2min) ? ptLow : y2min;
                                    }
                                }
                            }
                            #endregion
                            else
                            {
                                foreach (PointF p in ds.PointList)
                                {
                                    xmax = (p.X > xmax) ? p.X : xmax;
                                    xmin = (p.X < xmin) ? p.X : xmin;
                                    if (!ds.IsY2Data)
                                    {
                                        ymax = (p.Y > ymax) ? p.Y : ymax;
                                        ymin = (p.Y < ymin) ? p.Y : ymin;
                                    }
                                    else
                                    {
                                        y2max = (p.Y > y2max) ? p.Y : y2max;
                                        y2min = (p.Y < y2min) ? p.Y : y2min;
                                    }
                                }
                            }
                        }
                    }
                }
                //自适应触发条件：极值不满足或范围过大
                if (xa.XLimMax < xmax || xa.XLimMin > xmin || (xa.XLimMax - xa.XLimMin) / 2 > (xmax - xmin))
                {
                    xopenfit = true;
                    float xtick = 0;
                    FitModify(ref xmax, ref xmin, out xtick);
                    xa.XLimMax = xmax;
                    xa.XLimMin = xmin;
                    xa.XTick = xtick;
                }
                if (ya.YLimMax < ymax || ya.YLimMin > ymin || (ya.YLimMax - ya.YLimMin) / 2 > (ymax - ymin))
                {
                    yopenfit = true;
                    float ytick = 0;
                    FitModify(ref ymax, ref ymin, out ytick);
                    ya.YLimMax = ymax;
                    ya.YLimMin = ymin;
                    ya.YTick = ytick;
                }
                if (y2a.Y2LimMax < y2max || y2a.Y2LimMin > y2min || (y2a.Y2LimMax - y2a.Y2LimMin) / 2 > (y2max - y2min))
                {
                    y2openfit = true;
                    float y2tick = 0;
                    FitModify(ref y2max, ref y2min, out y2tick);
                    y2a.Y2LimMax = y2max;
                    y2a.Y2LimMin = y2min;
                    y2a.Y2Tick = y2tick;
                }
                //Bar的一侧宽度问题，预留一个tick
                if (havebar)
                {
                    if (baraxisX)
                    {
                        if (xa.XLimMin <= barmin && xa.XLimMin + xa.XTick > barmin)
                        {
                            xopenfit = true;
                            xa.XLimMin -= xa.XTick;
                            if (xa.XMarkStartindex == 0)
                            {
                                xa.XTickMarkFull[0] = string.Empty;
                            }
                        }
                        if (xmin <= barmin && xmin + xa.XTick > barmin)
                        {
                            xa.XTickMarkFull[0] = string.Empty;
                        }
                        if (xa.XLimMax >= barmax && xa.XLimMax - xa.XTick < barmax)
                        {
                            xopenfit = true;
                            xa.XLimMax += xa.XTick;
                        }
                        
                    }
                    else
                    {
                        if (ya.YLimMin <= barmin && ya.YLimMin + ya.YTick > barmin)
                        {
                            yopenfit = true;
                            ya.YLimMin -= ya.YTick;
                        }
                        if (ya.YLimMax >= barmax && ya.YLimMax - ya.YTick < barmax)
                        {
                            yopenfit = true;
                            ya.YLimMax += ya.YTick;
                        }
                    }
                }
            }
            catch { }
            if (xopenfit || yopenfit || y2openfit)
                openFit = true;
            return openFit;
        }

        /// <summary>
        /// 自适应调整limit和tick
        /// </summary>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="tick"></param>
        private static void FitModify(ref float maxvalue, ref float minvalue, out float tick)
        {
            if(maxvalue == minvalue)
            {
                double log = Math.Floor(Math.Log10(maxvalue));
                double ex = maxvalue / Math.Pow(10, log);
                tick = (float)Math.Pow(10, log);
                maxvalue = ((int)Math.Ceiling((maxvalue - 0) / tick) + 2) * tick;
                minvalue = ((int)Math.Floor((minvalue - 0) / tick) - 2) * tick;
            }
            else
            {
                double log = Math.Floor(Math.Log10(maxvalue - minvalue));//数量级
                double ex = (maxvalue - minvalue) / Math.Pow(10, log);
                if (ex >= 6)
                    tick = (float)Math.Pow(10, log) * 2;
                else if (ex >= 3)
                    tick = (float)Math.Pow(10, log);
                else if (ex >= 1)
                    tick = (float)Math.Pow(10, log) / 2;
                else if (ex >= 0.5)
                    tick = (float)Math.Pow(10, log) / 5;
                else
                    tick = (float)Math.Pow(10, log) / 10;
                maxvalue = (int)Math.Ceiling((maxvalue - 0) / tick) * tick;
                minvalue = (int)Math.Floor((minvalue - 0) / tick) * tick;
            }
        }

        #region 弃用，双y轴格网不便统一，只根据主y轴绘制格网

        ////*确定数据点位置，附加x时间文本
        //public static void ConvertValueToPoint_Tick(DataCollection dc, string timeformat, XAxis xa, YAxis ya, Y2Axis y2a)
        //{
        //    dc.Timeformat = timeformat;
        //    //获取最大最小值
        //    float fxmin = 0, fxmax = 0, maxcount = 0, fymin = 0, fymax = 0, fy2min = 0, fy2max = 0;
        //    foreach (DataSeries ds in dc.DataSeriesList)
        //    {
        //        maxcount = (ds.ValueList.Count > maxcount) ? ds.ValueList.Count : maxcount;
        //        if (!ds.IsY2Data)
        //        {
        //            for (int xno = 0; xno < ds.ValueList.Count; xno++)
        //            {
        //                DataValue dv = (DataValue)ds.ValueList[xno];
        //                fymax = (dv.YValue > fymax) ? dv.YValue : fymax;
        //                fymin = (dv.YValue < fymin) ? dv.YValue : fymin;
        //                if (dv.XValueType == typeof(double) || dv.XValueType == typeof(DateTime))
        //                {
        //                    fxmax = ((float)dv.XValue > fxmax) ? (float)dv.XValue : fxmax;
        //                    fxmin = ((float)dv.XValue < fxmin) ? (float)dv.XValue : fxmin;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for (int xno = 0; xno < ds.ValueList.Count; xno++)
        //            {
        //                DataValue dv = (DataValue)ds.ValueList[xno];
        //                fy2max = (dv.YValue > fy2max) ? dv.YValue : fy2max;
        //                fy2min = (dv.YValue < fy2min) ? dv.YValue : fy2min;
        //                if (dv.XValueType == typeof(double) || dv.XValueType == typeof(DateTime))
        //                {
        //                    fxmax = ((float)dv.XValue > fxmax) ? (float)dv.XValue : fxmax;
        //                    fxmin = ((float)dv.XValue < fxmin) ? (float)dv.XValue : fxmin;
        //                }
        //            }
        //        }
        //    }
        //    //根据值确定坐标格网最值,并统一双Y轴的tick数量便于绘制格网
        //    int grade1 = 5;
        //    ya.YLimMin = fymin;
        //    ya.YLimMax = AxisFit.Mod(fymax, fymin, ref grade1);
        //    if (fy2max - fy2min != 0)
        //    {
        //        ////方法1：改变最值
        //        //y2a.Y2LimMin = fy2min;
        //        //y2a.Y2LimMax = Mod(fy2max, fy2min, ref tick);
        //        //y2a.Y2Tick = tick;
        //        //int grade = (int)(((y2a.Y2LimMax - y2a.Y2LimMin) / y2a.Y2Tick > (ya.YLimMax - ya.YLimMin) / ya.YTick) ? (y2a.Y2LimMax - y2a.Y2LimMin) / y2a.Y2Tick : (ya.YLimMax - ya.YLimMin) / ya.YTick);
        //        //ya.YLimMax = grade * ya.YTick;
        //        //y2a.Y2LimMax = grade * y2a.Y2Tick;

        //        //方法2：改变最值和tick
        //        int grade2 = 5;
        //        y2a.Y2LimMin = fy2min;
        //        y2a.Y2LimMax = AxisFit.Mod(fy2max, fy2min, ref grade2);
        //        //int grade = (int)AxisFit.GreatDiv(ya.YLimMax - ya.YLimMin, y2a.Y2LimMax - y2a.Y2LimMin);
        //        int grade = (int)AxisFit.Div(grade1, grade2);
        //        ya.YTick = (ya.YLimMax - ya.YLimMin) / grade;
        //        y2a.Y2Tick = (y2a.Y2LimMax - y2a.Y2LimMin) / grade;
        //    }

        //    xa.XTick = (xa.XLimMax - xa.XLimMin) / (maxcount - 1);
        //    foreach (DataSeries ds in dc.DataSeriesList)
        //    {
        //        if (ds.IsY2Data)
        //            y2a.IsY2Axis = true;

        //        for (int xno = 0; xno < ds.ValueList.Count; xno++)
        //        {
        //            DataValue dv = (DataValue)ds.ValueList[xno];
        //            PointF p = new PointF(xa.XLimMin + xno * xa.XTick, dv.YValue);
        //            ds.PointList.Add(p);

        //            //添加X轴标记
        //            if (dv.XValueType == typeof(DateTime))
        //            {
        //                if (!xa.XTickMark.ContainsKey((float)Math.Round((double)(xa.XLimMin + xno * xa.XTick), 3)))
        //                    xa.AddTickMark(xa.XLimMin + xno * xa.XTick, TimeConverter.GetTime((int)Math.Round(dv.XValue)).ToString(timeformat));
        //            }
        //            else if (dv.XValueType == typeof(double))
        //            {
        //                if (!xa.XTickMark.ContainsKey((float)Math.Round((double)(xa.XLimMin + xno * xa.XTick), 3)))
        //                    xa.AddTickMark(xa.XLimMin + xno * xa.XTick, dv.XValue.ToString());
        //            }
        //            else
        //            {
        //                if (!xa.XTickMark.ContainsKey((float)Math.Round((double)(xa.XLimMin + xno * xa.XTick), 3)))
        //                    xa.AddTickMark(xa.XLimMin + xno * xa.XTick, dv.StrXValue.ToString());
        //            }
        //        }
        //    }

        //} 

        //公约数
        internal static double Div(double a, double b)
        {
            double P, Q, div;
            if (a >= b)
            {
                P = a; Q = b;
            }
            else
            {
                P = b; Q = a;
            }
            while (true)
            {
                if (P % Q == 0)
                {
                    div = Q;
                    break;
                }
                else
                {
                    double r = P % Q;
                    P = Q;
                    Q = r;
                }
            }

            return div;
        }
        //公约数
        internal static float GreatDiv(float a, float b)
        {
            int div = 1;
            int A, B;
            if (a > 1 && b > 1)
            {
                if (a >= b)
                {
                    A = (int)a; B = (int)b;
                }
                else
                {
                    A = (int)b; B = (int)a;
                }
                int P = A, Q = (B % 4 == 0) ? B / 4 : ((B % 5 == 0) ? B / 5 : ((B % 6 == 0) ? B / 6 : B));

                while (true)
                {
                    if (P % Q == 0)
                    {
                        div = Q;
                        break;
                    }
                    else
                    {
                        int r = P % Q;
                        P = Q;
                        Q = r;
                    }
                }
            }
            else
            {
                return 0.2f;
            }

            return B / div;
        }

        #endregion
    }

    
}
