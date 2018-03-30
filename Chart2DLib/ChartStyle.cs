using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Readearth.Chart2D.ChartElements;
using Readearth.Chart2D.BasicStyle;
using Readearth.Chart2D.Data;

namespace Readearth.Chart2D
{
    public class ChartStyle
    {
        private Chart2D chart2d;
        internal Rectangle chartArea;
        private Rectangle plotArea; 
        private GraphColor chartAreaColor;
        private GraphColor plotAreaColor;
        public float XOffset { get; set; }
        public float YOffset { get; set; }
        internal Rectangle polarArea;

        internal ChartStyle(Chart2D ct2d)
        {
            chart2d = ct2d;
            chartArea = new Rectangle(new Point(0, 0),chart2d.ChartSize);
            chartAreaColor = new GraphColor(Color.White, Color.White);
            plotAreaColor = new GraphColor();
            XOffset = ChartArea.Width / 30.0f;
            YOffset = ChartArea.Height / 30.0f;
        }
        #region 绘图区域
        [Description("Gets the size for the chart area."), Category("Appearance")]
        public Rectangle ChartArea
        {
            get { return chartArea; }
            //set { chartArea = value; }
        }
        [Description("Gets size for the plot area."), Category("Appearance")]
        public Rectangle PlotArea
        {
            get { return plotArea; }
            //set { plotArea = value; }
        }

        [Description("The background&border colors of the chart area."), Category("Appearance")]
        public GraphColor ChartAreaColor
        {
            get { return chartAreaColor; }
            set
            {
                chartAreaColor = value;
                //chart2d.AddChart();
            }
        }
        [Description("The background&border colors of the plot area."), Category("Appearance")]
        public GraphColor PlotAreaColor
        {
            get { return plotAreaColor; }
            set
            {
                plotAreaColor = value;
                //chart2d.AddChart();
            }
        }
        #endregion

        #region 通用样式
        /// <summary>
        /// 整体填色（通用）
        /// </summary>
        /// <param name="g"></param>
        private void FillColorInChartAreas(Graphics g)
        {
            //ChartArea填色
            Pen aPen = new Pen(ChartAreaColor.Border, 1f);
            SolidBrush aBrush = new SolidBrush(ChartAreaColor.Fill);
            g.FillRectangle(aBrush, ChartArea);
            g.DrawRectangle(aPen, ChartArea.X, ChartArea.Y, ChartArea.Width - aPen.Width, ChartArea.Height - aPen.Width);//减线宽处理
            //PlotArea填色
            aPen = new Pen(PlotAreaColor.Border, 1f);
            aBrush = new SolidBrush(PlotAreaColor.Fill);
            g.FillRectangle(aBrush, PlotArea);
            g.DrawRectangle(aPen, PlotArea.X, PlotArea.Y, PlotArea.Width, PlotArea.Height);
            aPen.Dispose();
            aBrush.Dispose();
            /* 现象：DrawRectangle右边、下边线条消失，出现两条空隙
             * 原因：线条骑墙，GDI+默认采用最快的取整方法，DrawRectangle比原始Rectangle多出一行一列，导致右下边框不见
             * 方法1：DrawRectangle的高、宽作减线宽处理，出现完整清晰边框。
             * 方法2：Rectangle不变，抗锯齿模式下偏移0.5个单位，画出半个像素，出现完整边框但整体图像模糊，不采用。
             * g.SmoothingMode = SmoothingMode.AntiAlias; g.PixelOffsetMode = PixelOffsetMode.Half; 
             * 注：FillRectangle不存在此问题
             */
        }
        /// <summary>
        /// 绘制标题（通用）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="tl"></param>
        private void AddTitle(Graphics g, Title tl)
        {
            StringFormat sFormat = new StringFormat();
            sFormat.Alignment = StringAlignment.Center;
            //SizeF stringSize = g.MeasureString(tl.Title, tl.TitleStyle.TextFont);
            //stringSize = g.MeasureString(lb.XLabel, lb.XYLabelStyle.TextFont);
            //字体居中的两种方法：
            //1.字符串水平对齐方式(sFormat.Alignment = StringAlignment.Center)；
            //2.起始点坐标加1/2字符串宽(stringSize.Width/2)。
            
            SizeF titleFontSize = g.MeasureString("A", tl.TitleStyle.TextFont);
            SolidBrush aBrush = new SolidBrush(tl.TitleStyle.TextColor);

            if (!string.IsNullOrWhiteSpace(tl.TitleName) || tl.TitleName.ToUpper() != "NO TITLE")
            {
                g.DrawString(tl.TitleName, tl.TitleStyle.TextFont, aBrush,
                    new Point(PlotArea.Left + PlotArea.Width / 2, ChartArea.Top + (int)YOffset),
                    sFormat);
            }
            aBrush.Dispose();
        }
        #endregion

        #region 初始化样式（默认）
        /// <summary>
        /// 初始化样式布局（默认为直角坐标，无图例）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="y2a"></param>
        /// <param name="gd"></param>
        /// <param name="lb"></param>
        /// <param name="tl"></param>
        internal void AddChartStyle(Graphics g, Title tl, XYLabel lb, Grid gd, XAxis xa, YAxis ya, Y2Axis y2a)
        {
            //设置绘图区位置、大小
            SetPlotArea(g, tl, lb, gd, xa, ya, y2a);
            //ChartArea和PlotArea底色绘制（通用）
            FillColorInChartAreas(g);
            //直角坐标通用网格线
            AddGirdSquare(g, gd, xa, ya);

            //绘制x，y,y2 轴刻度线和标记
            float fX, fY;
            SolidBrush aBrush = new SolidBrush(xa.XTickStyle.TextColor);
            for (fX = xa.XLimMin; fX <= xa.XLimMax; fX += xa.XTick)
            {
                if (Math.Abs(fX - Math.Round(fX, 7)) < 0.000002)
                    fX = (float)Math.Round(fX, 7);
                PointF yAxisPoint = Point2D(new PointF(fX, ya.YLimMin), xa, ya);
                g.DrawLine(Pens.Black, yAxisPoint, new PointF(yAxisPoint.X, yAxisPoint.Y - 5f));

                StringFormat sFormat = new StringFormat();
                sFormat.Alignment = StringAlignment.Far;
                SizeF sizeXTick = g.MeasureString(fX.ToString(), xa.XTickStyle.TextFont);
                g.DrawString(fX.ToString(), xa.XTickStyle.TextFont, aBrush, new PointF(yAxisPoint.X + sizeXTick.Width / 2, yAxisPoint.Y + 4f), sFormat);
            }

            aBrush = new SolidBrush(ya.YTickStyle.TextColor);
            SizeF tickFontSize = g.MeasureString("A", ya.YTickStyle.TextFont);
            for (fY = ya.YLimMin; fY <= ya.YLimMax; fY += ya.YTick)
            {
                if (Math.Abs(fY - Math.Round(fY, 7)) < 0.000002)
                    fY = (float)Math.Round(fY, 7);
                PointF xAxisPoint = Point2D(new PointF(xa.XLimMin, fY), xa, ya);
                g.DrawLine(Pens.Black, xAxisPoint, new PointF(xAxisPoint.X + 5f, xAxisPoint.Y));

                StringFormat sFormat = new StringFormat();
                sFormat.Alignment = StringAlignment.Far;
                g.DrawString(fY.ToString(), ya.YTickStyle.TextFont, aBrush, new PointF(xAxisPoint.X - 3f, xAxisPoint.Y - tickFontSize.Height / 2), sFormat);
            }

            if (y2a.IsY2Axis)
            {
                aBrush = new SolidBrush(y2a.Y2TickStyle.TextColor);
                tickFontSize = g.MeasureString("A", y2a.Y2TickStyle.TextFont);
                for (fY = y2a.Y2LimMin; fY <= y2a.Y2LimMax; fY += y2a.Y2Tick)
                {
                    if (Math.Abs(fY - Math.Round(fY, 7)) < 0.000002)
                        fY = (float)Math.Round(fY, 7);
                    PointF x2AxisPoint = Point2DY2(new PointF(xa.XLimMax, fY), xa, y2a);
                    g.DrawLine(Pens.Black, x2AxisPoint, new PointF(x2AxisPoint.X - 5f, x2AxisPoint.Y));

                    StringFormat sFormat = new StringFormat();
                    sFormat.Alignment = StringAlignment.Near;
                    g.DrawString(fY.ToString(), y2a.Y2TickStyle.TextFont, aBrush, new PointF(x2AxisPoint.X + 3f, x2AxisPoint.Y - tickFontSize.Height / 2), sFormat);
                }
            }
            aBrush.Dispose();

            AddTitle(g, tl);
            AddLabels(g, tl, lb, y2a);
        }
        /// <summary>
        /// 设置默认绘图区位置，大小（无图例影响的默认位置）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="y2a"></param>
        /// <param name="gd"></param>
        /// <param name="lb"></param>
        /// <param name="tl"></param>
        private void SetPlotArea(Graphics g, Title tl, XYLabel lb, Grid gd, XAxis xa, YAxis ya, Y2Axis y2a)
        {
            SizeF titleFontSize = g.MeasureString("A", tl.TitleStyle.TextFont);

            if (string.IsNullOrWhiteSpace(tl.TitleName) || tl.TitleName.ToUpper() == "NO TITLE")
            {
                titleFontSize.Width = 8f;
                titleFontSize.Height = 8f;
            }
            float xSpacing = XOffset / 2.0f;
            float ySpacing = YOffset / 2.0f;
            SizeF tickFontSize = g.MeasureString("A", xa.XTickStyle.TextFont);
            float tickSpacing = 2f;

            //获取y轴标记宽度
            SizeF yTickSize = g.MeasureString(ya.YLimMin.ToString(), ya.YTickStyle.TextFont);
            int ycount = (int)Math.Round((ya.YLimMax - ya.YLimMin) / ya.YTick) + 1;
            for (int i =0; i <ycount ; i++)
            {
                float yTick = ya.YLimMin + i * ya.YTick;
                if (Math.Abs(yTick - Math.Round(yTick, 7)) < 0.000002)
                    yTick = (float)Math.Round(yTick, 7);
                SizeF tempSize = g.MeasureString(yTick.ToString(), ya.YTickStyle.TextFont);
                if (yTickSize.Width < tempSize.Width)
                {
                    yTickSize = tempSize;
                }
            }
            SizeF labelFontSize = g.MeasureString("A", lb.XYLabelStyle.TextFont);
            float leftMargin = XOffset + labelFontSize.Height + xSpacing + yTickSize.Width + tickSpacing;
            float rightMargin = 2 * XOffset;
            float topMargin = YOffset + titleFontSize.Height + ySpacing;
            float bottomMargin = YOffset + labelFontSize.Height + ySpacing + tickFontSize.Height + tickSpacing;

            //单y轴绘图区大小
            int plotX = ChartArea.X + (int)leftMargin;
            int plotY = ChartArea.Y + (int)topMargin;
            int plotHeight = ChartArea.Height - (int)topMargin - (int)bottomMargin;
            int plotWidth = ChartArea.Width - (int)leftMargin - (int)rightMargin;

            //双y轴：重新定义绘图区宽度
            if (y2a.IsY2Axis)
            {
                //y2轴标记宽度
                SizeF y2TickSize = g.MeasureString(y2a.Y2LimMin.ToString(), y2a.Y2TickStyle.TextFont);
                int y2count = (int)Math.Round((y2a.Y2LimMax - y2a.Y2LimMin) / y2a.Y2Tick) + 1;
                for (int i =0; i <ycount ; i++)
                {
                    float y2Tick = y2a.Y2LimMin + i * y2a.Y2Tick;
                    if (Math.Abs(y2Tick - Math.Round(y2Tick, 7)) < 0.000002)
                        y2Tick = (float)Math.Round(y2Tick, 7);
                    SizeF tempSize2 = g.MeasureString(y2Tick.ToString(), y2a.Y2TickStyle.TextFont);
                    if (y2TickSize.Width < tempSize2.Width)
                    {
                        y2TickSize = tempSize2;
                    }
                }
                rightMargin = XOffset + labelFontSize.Height + xSpacing + y2TickSize.Width + tickSpacing;
                plotWidth = ChartArea.Width - (int)leftMargin - (int)rightMargin;
            }
            plotArea = new Rectangle(plotX, plotY, plotWidth, plotHeight);
           
        }
        /// <summary>
        /// 绘制标签（无图例影响的默认位置）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="tl"></param>
        /// <param name="lb"></param>
        /// <param name="y2a"></param>
        private void AddLabels(Graphics g, Title tl, XYLabel lb, Y2Axis y2a)
        {
            StringFormat sFormat = new StringFormat();
            sFormat.Alignment = StringAlignment.Center;

            SizeF titleFontSize = g.MeasureString("A", tl.TitleStyle.TextFont);
            SolidBrush aBrush = new SolidBrush(lb.XYLabelStyle.TextColor);
            //添加x轴标签
            SizeF labelFontSize = g.MeasureString("A", lb.XYLabelStyle.TextFont);
            if (lb.IsXLabelVisible)
            {
                if (string.IsNullOrWhiteSpace(lb.XLabel))
                    lb.XLabel = "X Axis";
                g.DrawString(lb.XLabel, lb.XYLabelStyle.TextFont, aBrush, 
                    new Point(PlotArea.Left + PlotArea.Width / 2,  ChartArea.Bottom - (int)YOffset - (int)labelFontSize.Height), sFormat);
            }

            //添加y轴标签
            if (lb.IsYLabelVisible)
            {
                if (string.IsNullOrWhiteSpace(lb.YLabel))
                    lb.YLabel = "Y Axis";
                // Save the state of the current Graphics object
                GraphicsState gState = g.Save();
                g.TranslateTransform(ChartArea.X + XOffset, ChartArea.Y + YOffset + titleFontSize.Height + YOffset / 2 + PlotArea.Height / 2);
                g.RotateTransform(-90);
                g.DrawString(lb.YLabel, lb.XYLabelStyle.TextFont, aBrush, new Point(0, 0), sFormat);
                // Restore it:
                g.Restore(gState);
            }

            //添加y2轴标签
            if (y2a.IsY2Axis)
            {
                if (lb.IsY2LabelVisible)
                {
                    if (string.IsNullOrWhiteSpace(lb.Y2Label))
                        lb.Y2Label = "Y2 Axis";
                    // Save the state of the current Graphics object
                    GraphicsState gState2 = g.Save();
                    g.TranslateTransform(ChartArea.X + ChartArea.Width - XOffset - labelFontSize.Width, ChartArea.Y + YOffset + titleFontSize.Height + YOffset / 2 + PlotArea.Height / 2);
                    g.RotateTransform(-90);
                    g.DrawString(lb.Y2Label, lb.XYLabelStyle.TextFont, aBrush, new Point(0, 0), sFormat);
                    // Restore it:
                    g.Restore(gState2);
                }
            }
            aBrush.Dispose();

        }
        /// <summary>
        /// 绘制直角坐标通用网格线
        /// </summary>
        /// <param name="g"></param>
        /// <param name="gd"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        private void AddGirdSquare(Graphics g, Grid gd, XAxis xa, YAxis ya)
        {
            float fX, fY;
            Pen aPen = new Pen(gd.GridColor, 1f);
            aPen.DashStyle = gd.GridPattern;
            //绘制垂直格网：
            if (gd.IsYGrid == true)
            {
                for (fX = xa.XLimMin + xa.XTick; fX < xa.XLimMax; fX += xa.XTick)
                {
                    if (Math.Abs(fX - Math.Round(fX, 7)) < 0.000002)
                        fX = (float)Math.Round(fX, 7);
                    //注：float类型变量的精度损失处理
                    g.DrawLine(aPen, Point2D(new PointF(fX, ya.YLimMin), xa, ya), Point2D(new PointF(fX, ya.YLimMax), xa, ya));
                }
            }

            //绘制水平格网：
            if (gd.IsXGrid == true)
            {
                for (fY = ya.YLimMin + ya.YTick; fY < ya.YLimMax; fY += ya.YTick)
                {
                    if (Math.Abs(fY - Math.Round(fY, 7)) < 0.000002)
                        fY = (float)Math.Round(fY, 7);
                    g.DrawLine(aPen, Point2D(new PointF(xa.XLimMin, fY), xa, ya), Point2D(new PointF(xa.XLimMax, fY), xa, ya));
                }
            }
            aPen.Dispose();
        }
        #endregion

        #region 直角坐标ChartStyle
        /// <summary>
        /// 直角坐标样式布局
        /// </summary>
        /// <param name="g"></param>
        /// <param name="dcs"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="y2a"></param>
        /// <param name="gd"></param>
        /// <param name="lb"></param>
        /// <param name="tl"></param>
        /// <param name="lg"></param>
        internal void AddChartStyleSquare(Graphics g, Title tl, XYLabel lb, Grid gd, XAxis xa, YAxis ya, Y2Axis y2a, Legend lg, List<DataCollection> dcs)
        {
            try
            {
                //设置直角坐标绘图区位置、大小（受元素之间的相互影响）
                SetPlotArea(g, tl, lb, xa, ya, y2a, gd, lg, dcs);
                //再次确定大小的原因：根据plot宽度->确定文本角度->重新确定plot高度
                AxisFit.SetTickAngle(g, this, lb, xa);
                SetPlotArea(g, tl, lb, xa, ya, y2a, gd, lg, dcs);

                //ChartArea和PlotArea底色绘制（通用）
                FillColorInChartAreas(g);
                //绘制标题（通用）
                AddTitle(g, tl);
                //直角坐标通用网格线
                AddGirdSquare(g, gd, xa, ya);

                //绘制轴标签（受图例影响）
                AddLabels(g, tl, lb, y2a, lg, dcs);
                //绘制坐标轴刻度线和标记（受数据影响）
                AddTicks(g, lb, xa, ya, y2a);
            }
            catch { }
        }
        /// <summary>
        /// 设置直角坐标绘图区位置、大小（受元素之间的相互影响）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="tl"></param>
        /// <param name="lb"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="y2a"></param>
        /// <param name="gd"></param>
        /// <param name="lg"></param>
        /// <param name="dcs"></param>
        private void SetPlotArea(Graphics g, Title tl, XYLabel lb, XAxis xa, YAxis ya, Y2Axis y2a, Grid gd, Legend lg, List<DataCollection> dcs)
        {
            SizeF titleFontSize = g.MeasureString("A", tl.TitleStyle.TextFont);
            if (string.IsNullOrWhiteSpace(tl.TitleName) || tl.TitleName.ToUpper() == "NO TITLE")
            {
                titleFontSize.Width = 8f;
                titleFontSize.Height = 8f;
            }
            float xSpacing = XOffset / 2.0f;
            float ySpacing = YOffset / 2.0f;
            SizeF labelFontSize = g.MeasureString("A", lb.XYLabelStyle.TextFont);
            SizeF tickFontSize = g.MeasureString("A", xa.XTickStyle.TextFont);
            float tickSpacing = 2f;

            //图例所在位置影响plotarea的大小和位置
            #region 不受图例影响时
            if (!lg.IsLegendVisible || lg.IsInside)
            {
                //X轴标记的自适应和全显示，影响底部间距
                //float bottomMargin = YOffset + xlabelFontSize.Height + ySpacing + tickFontSize.Height + tickSpacing;
                float bottomMargin = YOffset + tickSpacing + ySpacing;
                if (lb.IsXLabelVisible)
                    bottomMargin += labelFontSize.Height;

                SizeF xTickSize = g.MeasureString(xa.XLimMin.ToString(), xa.XTickStyle.TextFont);
                int xcount = (int)Math.Round((xa.XLimMax - xa.XLimMin) / xa.XTick) + 1;
                for (int i = 0; i < xcount; i++)
                {
                    SizeF tempSize = g.MeasureString(xa.XTickMarkFull[i], xa.XTickStyle.TextFont);
                    if (xTickSize.Width < tempSize.Width)
                    {
                        xTickSize = tempSize;
                    }
                }
                if (xa.XTickAngle == 0)
                    bottomMargin += xTickSize.Height;
                else if (xa.XTickAngle == -90)
                    bottomMargin += xTickSize.Width;
                else
                    bottomMargin += xTickSize.Width * 0.72f;
                

                //获取y轴标记宽度
                SizeF yTickSize = g.MeasureString(ya.YLimMin.ToString(), ya.YTickStyle.TextFont);
                int ycount = (int)Math.Round((ya.YLimMax - ya.YLimMin) / ya.YTick) + 1;
                for (int i = 0; i  < ycount; i++)
                {
                    float yTick = ya.YLimMin + i * ya.YTick;
                    SizeF tempSize = g.MeasureString(yTick.ToString(), ya.YTickStyle.TextFont);
                    if (yTickSize.Width < tempSize.Width)
                    {
                        yTickSize = tempSize;
                    }
                }
                float leftMargin = XOffset + labelFontSize.Height + xSpacing + yTickSize.Width + tickSpacing;
                float rightMargin = 2 * XOffset;
                float topMargin = YOffset + titleFontSize.Height + ySpacing;

                //单y轴绘图区
                int plotX = ChartArea.X + (int)leftMargin;
                int plotY = ChartArea.Y + (int)topMargin;
                int plotHeight = ChartArea.Height - (int)topMargin - (int)bottomMargin;
                int plotWidth = ChartArea.Width - (int)leftMargin - (int)rightMargin;

                //双y轴：重新定义绘图区宽度
                if (y2a.IsY2Axis)
                {
                    //y2轴标记宽度
                    SizeF y2TickSize = g.MeasureString(y2a.Y2LimMin.ToString(), y2a.Y2TickStyle.TextFont);
                    int y2count = (int)Math.Round((y2a.Y2LimMax - y2a.Y2LimMin) / y2a.Y2Tick) + 1;
                    for (int i = 0; i < y2count ; i++)
                    {
                        float y2Tick = y2a.Y2LimMin + i * y2a.Y2Tick;
                        SizeF tempSize2 = g.MeasureString(y2Tick.ToString(), y2a.Y2TickStyle.TextFont);
                        if (y2TickSize.Width < tempSize2.Width)
                        {
                            y2TickSize = tempSize2;
                        }
                    }
                    rightMargin = XOffset + labelFontSize.Height + xSpacing + y2TickSize.Width + tickSpacing;
                    plotWidth = ChartArea.Width - (int)leftMargin - (int)rightMargin;
                }
                plotArea = new Rectangle(plotX, plotY, plotWidth, plotHeight);
            }
            #endregion
            #region 外部图例
            else
            {
                //X轴标记的自适应和全显示，影响底部间距bottom
                float bottomMargin = YOffset + tickSpacing + ySpacing;
                if (lb.IsXLabelVisible)
                    bottomMargin += labelFontSize.Height;
                
                SizeF xTickSize = g.MeasureString(xa.XLimMin.ToString(), xa.XTickStyle.TextFont);
                int count = (int)Math.Round((xa.XLimMax - xa.XLimMin) / xa.XTick) + 1;
                for (int i = 0; i  < count; i++)
                {
                    SizeF tempSize = g.MeasureString(xa.XTickMarkFull[i], xa.XTickStyle.TextFont);
                    if (xTickSize.Width < tempSize.Width)
                    {
                        xTickSize = tempSize;
                    }
                }
                if (xa.XTickAngle == 0)
                    bottomMargin += xTickSize.Height;
                else if (xa.XTickAngle == -90)
                    bottomMargin += xTickSize.Width;
                else
                    bottomMargin += xTickSize.Width * 0.72f;

                //获取y轴标记宽度
                SizeF yTickSize = g.MeasureString(ya.YLimMin.ToString(), ya.YTickStyle.TextFont);
                int ycount = (int)Math.Round((ya.YLimMax - ya.YLimMin) / ya.YTick) + 1;
                for (int i = 0;  i < ycount; i++)
                {
                    float yTick = ya.YLimMin + i * ya.YTick;
                    SizeF tempSize = g.MeasureString(yTick.ToString(), ya.YTickStyle.TextFont);
                    if (yTickSize.Width < tempSize.Width)
                    {
                        yTickSize = tempSize;
                    }
                }
                float leftMargin = XOffset + labelFontSize.Height + xSpacing + yTickSize.Width + tickSpacing;
                float rightMargin = XOffset + xSpacing;
                float topMargin = YOffset + titleFontSize.Height + ySpacing;

                //根据图例位置确定边距
                float legendWidth = 0; float legendHeight = 0;
                lg.GetLegendSize(g, dcs, this, out legendWidth, out legendHeight);
                if (lg.LegendPosition == Legend.LegendPositionEnum.East || lg.LegendPosition == Legend.LegendPositionEnum.SouthEast || lg.LegendPosition == Legend.LegendPositionEnum.NorthEast)
                    rightMargin += legendWidth;
                if (lg.LegendPosition == Legend.LegendPositionEnum.West || lg.LegendPosition == Legend.LegendPositionEnum.SouthWest || lg.LegendPosition == Legend.LegendPositionEnum.NorthWest)
                    leftMargin += legendWidth;
                if (lg.LegendPosition == Legend.LegendPositionEnum.North)
                    topMargin += legendHeight;
                if (lg.LegendPosition == Legend.LegendPositionEnum.South)
                    bottomMargin += legendHeight;

                //单y轴绘图区大小
                int plotX = ChartArea.X + (int)leftMargin;
                int plotY = ChartArea.Y + (int)topMargin;
                int plotHeight = ChartArea.Height - (int)topMargin - (int)bottomMargin;
                int plotWidth = ChartArea.Width - (int)leftMargin - (int)rightMargin;

                //双y轴：重新定义绘图区宽度
                if (y2a.IsY2Axis)
                {
                    //y2轴标记宽度
                    SizeF y2TickSize = g.MeasureString(y2a.Y2LimMin.ToString(), y2a.Y2TickStyle.TextFont);
                    int y2count = (int)Math.Round((y2a.Y2LimMax - y2a.Y2LimMin) / y2a.Y2Tick) + 1;
                    for (int i = 0;  i <y2count ; i++)
                     {
                         float y2Tick = y2a.Y2LimMin + i * y2a.Y2Tick;
                        SizeF tempSize2 = g.MeasureString(y2Tick.ToString(), y2a.Y2TickStyle.TextFont);
                        if (y2TickSize.Width < tempSize2.Width)
                        {
                            y2TickSize = tempSize2;
                        }
                    }
                    rightMargin = XOffset + labelFontSize.Height + xSpacing + y2TickSize.Width + tickSpacing;

                    if (lg.LegendPosition == Legend.LegendPositionEnum.East || lg.LegendPosition == Legend.LegendPositionEnum.SouthEast || lg.LegendPosition == Legend.LegendPositionEnum.NorthEast)
                    {
                        rightMargin += legendWidth;
                    }
                    plotWidth = ChartArea.Width - (int)leftMargin - (int)rightMargin;
                }
                plotArea = new Rectangle(plotX, plotY, plotWidth, plotHeight);
            }
            #endregion
        }
        /// <summary>
        /// 绘制轴标签（受图例影响）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="tl"></param>
        /// <param name="lb"></param>
        /// <param name="y2a"></param>
        /// <param name="lg"></param>
        /// <param name="dcs"></param>
        private void AddLabels(Graphics g, Title tl, XYLabel lb, Y2Axis y2a, Legend lg, List<DataCollection> dcs)
        {
            StringFormat sFormat = new StringFormat();
            sFormat.Alignment = StringAlignment.Center;
            SolidBrush aBrush = new SolidBrush(lb.XYLabelStyle.TextColor);
            SizeF labelFontSize = g.MeasureString("A", lb.XYLabelStyle.TextFont);
            float stringX; float stringY;

            SizeF titleFontSize = g.MeasureString("A", tl.TitleStyle.TextFont);
            //图例对标题的影响
            float legendWidth = 0; float legendHeight = 0;
            if (lg.IsLegendVisible && !lg.IsInside)
                lg.GetLegendSize(g, dcs, this, out legendWidth, out legendHeight);

            //添加x轴标签
            if (lb.IsXLabelVisible)
            {
                if (string.IsNullOrWhiteSpace(lb.XLabel))
                    lb.XLabel = "X Axis";
                stringX = PlotArea.Left + PlotArea.Width / 2;
                stringY = ChartArea.Bottom - (int)YOffset - (int)labelFontSize.Height;
                if (lg.LegendPosition == Legend.LegendPositionEnum.South)
                    stringY -= legendHeight;
                g.DrawString(lb.XLabel , lb.XYLabelStyle.TextFont, aBrush, stringX, stringY, sFormat);
            }

            //添加y轴标签
            if (string.IsNullOrWhiteSpace(lb.YLabel))
                lb.YLabel  = "Y Axis";
            stringX = ChartArea.X + XOffset;
            stringY = ChartArea.Y + YOffset + titleFontSize.Height + YOffset / 3 + PlotArea.Height / 2;
            if (lg.LegendPosition == Legend.LegendPositionEnum.West || lg.LegendPosition == Legend.LegendPositionEnum.SouthWest || lg.LegendPosition == Legend.LegendPositionEnum.NorthWest)
                stringX += legendWidth;
            if (lg.LegendPosition == Legend.LegendPositionEnum.North)
                stringY += legendHeight;
            // Save the state of the current Graphics object
            GraphicsState gState = g.Save();
            g.TranslateTransform(stringX, stringY);
            g.RotateTransform(-90);
            g.DrawString(lb.YLabel , lb.XYLabelStyle.TextFont, aBrush, 0, 0, sFormat);
            // Restore it:
            g.Restore(gState);

            //添加y2轴标签
            if (y2a.IsY2Axis)
            {
                if (string.IsNullOrWhiteSpace(lb.Y2Label ))
                    lb.Y2Label  = "Y2 Axis";
                stringX = ChartArea.X + ChartArea.Width - XOffset - labelFontSize.Height;
                stringY = ChartArea.Y + YOffset + titleFontSize.Height + YOffset / 3 + PlotArea.Height / 2;
                if (lg.LegendPosition == Legend.LegendPositionEnum.East || lg.LegendPosition == Legend.LegendPositionEnum.SouthEast || lg.LegendPosition == Legend.LegendPositionEnum.NorthEast)
                    stringX -= legendWidth;
                if (lg.LegendPosition == Legend.LegendPositionEnum.North)
                    stringY += legendHeight;
                // Save the state of the current Graphics object
                GraphicsState gState2 = g.Save();
                g.TranslateTransform(stringX, stringY);
                g.RotateTransform(-90);
                g.DrawString(lb.Y2Label , lb.XYLabelStyle.TextFont, aBrush, 0, 0, sFormat);

                // Restore it:
                g.Restore(gState2);
            }
            aBrush.Dispose();
        }
        /// <summary>
        /// 绘制刻度线和标记（受数据影响）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lb"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="y2a"></param>
        private void AddTicks(Graphics g, XYLabel lb, XAxis xa, YAxis ya, Y2Axis y2a)
        {
            float fX, fY;
            //绘制x轴刻度线和标记：
            SolidBrush aBrush = new SolidBrush(xa.XTickStyle.TextColor);
            SizeF sizeXTick;

            //出现在SetPlotArea和AddTicks中三轴循环
            //for(float xTick = xa.XLimMin; xTick <= xa.XLimMax; xTick += xa.XTick)
            //弃用原因1：防止Tick为0时出现死循环
            //弃用原因2：float的精度损失程度累加

            int count = (int)Math.Round((xa.XLimMax - xa.XLimMin) / xa.XTick) + 1;
            for (int i = 0; i < count; i++)
            {
                fX = xa.XLimMin + i * xa.XTick;
                if (Math.Abs(fX - Math.Round(fX, 7)) < 0.000002)
                    fX = (float)Math.Round(fX, 7);
                PointF yAxisPoint = Point2D(new PointF(fX, ya.YLimMin), xa, ya);
                g.DrawLine(Pens.Black, yAxisPoint, new PointF(yAxisPoint.X, yAxisPoint.Y - 5f));

                StringFormat sFormat = new StringFormat();
                sFormat.Alignment = StringAlignment.Far;

                sizeXTick = g.MeasureString(xa.XTickMarkFull[i], xa.XTickStyle.TextFont);
                if (xa.XTickAngle != 0)
                {
                    g.TranslateTransform(yAxisPoint.X, yAxisPoint.Y); //设置旋转中心为文字中心
                    g.RotateTransform(xa.XTickAngle); //旋转
                    //g.TranslateTransform(-yAxisPoint.X, -yAxisPoint.Y);
                    g.DrawString(xa.XTickMarkFull[i], xa.XTickStyle.TextFont, aBrush, new PointF(0, 4f - sizeXTick.Height / 2), sFormat);
                    g.ResetTransform();
                }
                else
                    g.DrawString(xa.XTickMarkFull[i], xa.XTickStyle.TextFont, aBrush, new PointF(yAxisPoint.X + sizeXTick.Width / 2, yAxisPoint.Y + 4f), sFormat);
            }

            SizeF tickFontSize = g.MeasureString("A", ya.YTickStyle.TextFont);
            //绘制y轴刻度线和标记：
            int ycount = (int)Math.Round((ya.YLimMax - ya.YLimMin) / ya.YTick) + 1;
            for (int i=0; i < ycount ;i++)
            {
                fY = ya.YLimMin + i * ya.YTick;
                if (Math.Abs(fY - Math.Round(fY, 7)) < 0.000002)
                    fY = (float)Math.Round(fY, 7);
                PointF xAxisPoint = Point2D(new PointF(xa.XLimMin, fY), xa, ya);
                g.DrawLine(Pens.Black, xAxisPoint, new PointF(xAxisPoint.X + 5f, xAxisPoint.Y));

                StringFormat sFormat = new StringFormat();
                sFormat.Alignment = StringAlignment.Far;

                g.DrawString(fY.ToString(), ya.YTickStyle.TextFont, aBrush, new PointF(xAxisPoint.X - 3f, xAxisPoint.Y - tickFontSize.Height / 2), sFormat);
            }

            //绘制y2轴刻度线和标记：
            if (y2a.IsY2Axis)
            {
                int y2count = (int)Math.Round((y2a.Y2LimMax - y2a.Y2LimMin) / y2a.Y2Tick) + 1;
                for (int i = 0; i < y2count ; i++)
                {
                    fY = y2a.Y2LimMin + i * y2a.Y2Tick;
                    if (Math.Abs(fY - Math.Round(fY, 7)) < 0.000002)
                        fY = (float)Math.Round(fY, 7);
                    PointF x2AxisPoint = Point2DY2(new PointF(xa.XLimMax, fY), xa, y2a);
                    g.DrawLine(Pens.Black, x2AxisPoint, new PointF(x2AxisPoint.X - 5f, x2AxisPoint.Y));

                    StringFormat sFormat = new StringFormat();
                    sFormat.Alignment = StringAlignment.Near;
                    g.DrawString(fY.ToString(), y2a.Y2TickStyle.TextFont, aBrush, new PointF(x2AxisPoint.X + 3f, x2AxisPoint.Y - tickFontSize.Height / 2), sFormat);
                }
            }
            aBrush.Dispose();
        }
        #endregion

        #region 极坐标ChartSytle
        /// <summary>
        /// 极坐标下的样式布局
        /// </summary>
        /// <param name="g"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="y2a"></param>
        /// <param name="gd"></param>
        /// <param name="lb"></param>
        /// <param name="tl"></param>
        /// <param name="lg"></param>
        /// <param name="dcs"></param>
        internal void AddChartStylePolar(Graphics g, Title tl, XYLabel lb, Grid gd, XAxis xa, YAxis ya, Y2Axis y2a, Legend lg, List<DataCollection> dcs)
        {
            //图例外置
            lg.IsInside = false;
            SetPolarArea(g, tl, lg, dcs);
            //整体填色（通用）
            FillColorInChartAreas(g);
            //绘制标题（通用）
            AddTitle(g, tl);
        }
        /// <summary>
        /// 设置极坐标图的绘图区位置，大小
        /// </summary>
        /// <param name="g"></param>
        /// <param name="tl"></param>
        /// <param name="lg"></param>
        /// <param name="dcs"></param>
        private void SetPolarArea(Graphics g, Title tl, Legend lg, List<DataCollection> dcs)
        {
            SizeF titleFontSize = g.MeasureString("A", tl.TitleStyle.TextFont);
            float lgW, lgH, xsurplus, ysurplus;
            lg.GetLegendSize(g, dcs, this, out lgW, out lgH);
            xsurplus = ysurplus = 0;
            int xSpacing = (int)XOffset / 2;
            int ySpacing = (int)YOffset / 2;
            float topMargin, bottomMargin, leftMargin, rightMargin;

            if ((lg.LegendPosition != Legend.LegendPositionEnum.North && lg.LegendPosition != Legend.LegendPositionEnum.South))
            {
                topMargin = YOffset + titleFontSize.Height + ySpacing;
                bottomMargin = YOffset;
                switch (lg.LegendPosition)
                {
                    case Legend.LegendPositionEnum.East:
                    case Legend.LegendPositionEnum.NorthEast:
                    case Legend.LegendPositionEnum.SouthEast:
                        leftMargin = XOffset;
                        rightMargin = XOffset + xSpacing + lgW;
                        break;
                    case Legend.LegendPositionEnum.West:
                    case Legend.LegendPositionEnum.NorthWest:
                    case Legend.LegendPositionEnum.SouthWest:
                        leftMargin = XOffset + xSpacing + lgW;
                        rightMargin = XOffset;
                        break;
                    default :
                        leftMargin = rightMargin = XOffset;
                        break;
                }
            }
            else
            {
                leftMargin = rightMargin = XOffset;
                switch (lg.LegendPosition)
                {
                    case Legend.LegendPositionEnum.North:
                        topMargin = YOffset + titleFontSize.Height + 2 * ySpacing + lgH;
                        bottomMargin = YOffset;
                        break;
                    case Legend.LegendPositionEnum.South:
                        topMargin = YOffset + titleFontSize.Height + ySpacing;
                        bottomMargin = YOffset + ySpacing + lgH;
                        break;
                    default:
                        topMargin = YOffset + titleFontSize.Height + ySpacing;
                        bottomMargin = YOffset + ySpacing + lgH;
                        break;
                }
            }
            int plotHeight = ChartArea.Height - (int)topMargin - (int)bottomMargin; 
            int plotWidth = chartArea.Width - (int)leftMargin - (int)rightMargin;
            int diameter = (plotHeight <= plotWidth) ? plotHeight : plotWidth;
            xsurplus = plotWidth - diameter;
            ysurplus = plotHeight - diameter;
            int plotX = (int)(ChartArea.X + leftMargin + xsurplus / 2);
            int plotY = (int)(ChartArea.Y + topMargin + ysurplus / 2);
            plotArea = new Rectangle(plotX, plotY, diameter, diameter);
            //考虑到Pie的突出情况，预留1/10的空间，PieCharts实际绘制区为polarArea
            polarArea = new Rectangle(plotX + diameter / 12, plotY + diameter / 12, diameter * 10 / 12, diameter * 10 / 12);
        }

        #endregion

        /// <summary>
        /// 绘制过程中以主纵坐标轴为准的坐标转换
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <returns></returns>
        internal PointF Point2D(PointF pt, XAxis xa, YAxis ya)
        {
            PointF aPoint = new PointF();
            if (pt.X < xa.XLimMin || pt.X > xa.XLimMax || pt.Y < ya.YLimMin || pt.Y > ya.YLimMax)
            {
                pt.X = Single.NaN;
                pt.Y = Single.NaN;
            }
            aPoint.X = PlotArea.Left + (pt.X - xa.XLimMin) * PlotArea.Width / (xa.XLimMax - xa.XLimMin);
            aPoint.Y = PlotArea.Bottom - (pt.Y - ya.YLimMin) * PlotArea.Height / (ya.YLimMax - ya.YLimMin);
            return aPoint;
        }
        /// <summary>
        /// 绘制过程中以次纵坐标轴为准的坐标转换
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="xa"></param>
        /// <param name="y2a"></param>
        /// <returns></returns>
        internal PointF Point2DY2(PointF pt, XAxis xa, Y2Axis y2a)
        {
            PointF aPoint = new PointF();
            if (pt.X < xa.XLimMin || pt.X > xa.XLimMax || pt.Y < y2a.Y2LimMin || pt.Y > y2a.Y2LimMax)
            {
                pt.X = Single.NaN;
                pt.Y = Single.NaN;
            }
            aPoint.X = PlotArea.Left + (pt.X - xa.XLimMin) * PlotArea.Width / (xa.XLimMax - xa.XLimMin);
            aPoint.Y = PlotArea.Bottom - (pt.Y - y2a.Y2LimMin) * PlotArea.Height / (y2a.Y2LimMax - y2a.Y2LimMin);
            return aPoint;
        }

        /// <summary>
        /// 样式复制，包括绘图区大小、颜色和边距
        /// </summary>
        /// <returns></returns>
        public ChartStyle Clone()
        {
            ChartStyle newCS = new ChartStyle(this.chart2d);
            newCS.chartArea = new Rectangle(this.ChartArea.X, this.ChartArea.Y, this.ChartArea.Width, this.ChartArea.Height);
            newCS.plotArea = new Rectangle(this.PlotArea.X, this.PlotArea.Y, this.PlotArea.Width, this.PlotArea.Height);
            newCS.ChartAreaColor = this.ChartAreaColor.Clone();
            newCS.PlotAreaColor = this.PlotAreaColor.Clone();
            newCS.XOffset = this.XOffset;
            newCS.YOffset = this.YOffset;
            return newCS;
        }
    }

}

