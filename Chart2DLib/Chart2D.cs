using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Readearth.Chart2D.ChartElements;
using Readearth.Chart2D.Data;

namespace Readearth.Chart2D
{
    public enum Chart2DTypeEnum
    {
        None = 0,
        LineChart = 1,
        BarChart = 2,
        AreaChart = 3,
        PieChart = 4,
        PolorChart = 5
    }

    public class Chart2D
    {
        private Size chartSize;
        private ChartStyle cs;
        private List<DataCollection> dclist = new List<DataCollection>();
        private XAxis xa = new XAxis();
        private YAxis ya = new YAxis();
        private Y2Axis y2a = new Y2Axis();
        private Grid gd = new Grid();
        private XYLabel lb = new XYLabel();
        private Title tl = new Title();
        private Legend lg = new Legend();
        private bool isAxesfit = false;
        //private DataCollection dc_sub = new DataCollection();


        public Chart2D()
        {
            chartSize = new Size(600, 400);
            cs = new ChartStyle(this);
            //DataCollection dc = new DataCollection();
            //dclist.Add(dc);
            //dc_sub.ColorTable = "WarnColors.txt";
        }
        public Chart2D(Size chartsize)
        {
            chartSize = chartsize;
            cs = new ChartStyle(this);
            //DataCollection dc = new DataCollection();
            //dclist.Add(dc);
            //dc_sub.ColorTable = "WarnColors.txt";
        }
        public Bitmap AddChart()
        {
            Bitmap bmp = new Bitmap(chartSize.Width, chartSize.Height);
            //绘图质量控制
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.HighQuality;//抗锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.CompositingMode = CompositingMode.SourceCopy;
            //g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit; //文本抗锯齿ClearTypeGridFit/AntiAliasGridFit 
            
            bool isSquare = true;
            //图表组合判断
            if (CombineJudge(ref isSquare))
            {
                //是否含次坐标轴数据
                bool isY2 = false;
                foreach (DataCollection dc in dclist)
                {
                    foreach (DataSeries ds in dc.DataSeriesList)
                    {
                        if (ds.IsY2Data)
                        {
                            isY2 = true;
                            break;
                        }
                    }
                    if (isY2)
                        break;
                }
                y2a.IsY2Axis = isY2;

                //坐标轴检查，返回自适应是否开启,待完善
                isAxesfit = AxisFit.AxesFitCheck(dclist, xa, ya, y2a);

                //基础要素布局
                if (isSquare)
                {
                    //cs.AddChartStyle(g, xa, ya, y2a, gd, lb, tl);
                    cs.AddChartStyleSquare(g, tl, lb, gd, xa, ya, y2a, lg, dclist);
                }
                else
                {
                    //极坐标图
                    //额外处理：绘图区PlotArea颜色调整，与总体Area内部颜色一致
                    cs.PlotAreaColor.Border = cs.PlotAreaColor.Fill = cs.ChartAreaColor.Fill;
                    cs.AddChartStylePolar(g, tl, lb, gd, xa, ya, y2a, lg, dclist);
                }

                //数据排序绘制
                CombineOrder();
                foreach (DataCollection dc in dclist)
                {
                    if (dc.ChartType == Chart2DTypeEnum.LineChart)
                        LineCharts.AddLines(dc, g, cs, xa, ya, y2a);
                    else if (dc.ChartType == Chart2DTypeEnum.AreaChart)
                        AreaCharts.AddAreas((DataCollectionArea)dc, g, cs, xa, ya, y2a);
                    else if (dc.ChartType == Chart2DTypeEnum.BarChart)
                        BarCharts.AddBars((DataCollectionBar)dc, g, cs, xa, ya, y2a);
                    else if (dc.ChartType == Chart2DTypeEnum.PieChart)
                        PieCharts.AddPie((DataCollectionPie)dc, g, cs);
                }
                if (lg.IsLegendVisible)
                    lg.AddLegend(g, dclist, cs, tl);
            }
            else
                //默认空白直角坐标底图
                cs.AddChartStyle(g, tl, lb, gd, xa, ya, y2a);


            ////AxisFit.ConvertValueToPoint_Tick(dc, "yyyyMMdd HH:mm", xa, ya, y2a);

            ////cs.AddChartStyle(g, dc, ca, xa, ya, y2a, gd, lb, tl, lg);
            ////if (dc_sub.DataSeriesList.Count > 0)
            ////    AddSubs(g, cs, dc_sub, xa, ya, y2a);


            g.Dispose();
            GC.Collect();
            return bmp;
        }
    
        //图表兼容性判断
        internal bool CombineJudge(ref bool isSquare)
        {
            bool isSuccess = true;
            if (dclist.Count < 1)
                return isSuccess = false;

            string[] datatypes = new string[dclist.Count];
            int lines,areas,bars,pies,polars;
            lines = areas = bars = pies = polars =0;
            for(int n=0;n<dclist.Count;n++)
            {
                datatypes[n] = dclist[n].ChartType.ToString();
                if (datatypes[n] == Chart2DTypeEnum.LineChart.ToString())
                    lines++;
                else if (datatypes[n] == Chart2DTypeEnum.AreaChart.ToString())
                    areas++;
                else if (datatypes[n] == Chart2DTypeEnum.BarChart.ToString())
                    bars++;
                else if (datatypes[n] == Chart2DTypeEnum.PieChart.ToString())
                    pies++;
                else if(datatypes[n] == Chart2DTypeEnum.PolorChart.ToString())
                    polars++;
            }
            if (lines > 0 && areas <= 1 && bars <= 1 && pies + polars < 1)
                isSuccess = true;
            else if (polars > 0 && lines + areas + bars + pies < 1)
                isSuccess = true;
            else if (pies == 1 && lines + areas + bars + polars < 1)
                isSuccess = true;
            else
                isSuccess = false;

            if (pies + polars > 0 && lines + areas + bars < 1)
                isSquare = false;

            return isSuccess;
        }
        //图标组合叠放顺序
        internal void CombineOrder()
        {
            //鸡尾酒排序法
            int left = 0;                            // 初始化边界
            int right = dclist.Count - 1;
            while (left < right)
            {
                for (int i = left; i < right; i++)   // 前半轮,将最小元素放到后面
                {
                    if ((int)dclist[i].ChartType < (int)dclist[i + 1].ChartType)
                    {
                        DataCollection temp = dclist[i];
                        dclist[i] = dclist[i + 1];
                        dclist[i + 1] = temp;
                    }
                }
                right--;
                for (int i = right; i > left; i--)   // 后半轮,将最大元素放到前面
                {
                    if ((int)dclist[i - 1].ChartType < (int)dclist[i].ChartType)
                    {
                        DataCollection temp = dclist[i - 1];
                        dclist[i - 1] = dclist[i];
                        dclist[i] = temp;
                    }
                }
                left++;
            }
        }


        #region 设置方法
        //设置大小
        public void SetChartSize(int height, int width)
        {
            chartSize.Height = height;
            chartSize.Width = width;
            cs.chartArea = new Rectangle(new Point(0, 0), chartSize);
            cs.XOffset = cs.ChartArea.Width / 30.0f;
            cs.YOffset = cs.ChartArea.Height / 30.0f;
            //this.AddChart();
        }
        //设置标题
        public void SetTitle(string titlename)
        {
            tl.TitleName = titlename;
            //this.AddChart();
        }
        public void SetTitle(string titlename, Font titlefont, Color titlecolor)
        {
            tl.TitleName = titlename;
            tl.TitleStyle.TextFont = titlefont;
            tl.TitleStyle.TextColor = titlecolor;
            //this.AddChart();
        }
        //设置标签
        public void SetLabelsName(string xlabel, string ylabel, string y2label)
        {
            if (string.IsNullOrWhiteSpace(xlabel))
                lb.IsXLabelVisible = false;
            else
            {
                lb.IsXLabelVisible = true;
                lb.XLabel = xlabel;
            }

            if (string.IsNullOrWhiteSpace(ylabel))
                lb.IsYLabelVisible = false;
            else
            {
                lb.IsYLabelVisible = true;
                lb.YLabel = ylabel;
            }

            if (string.IsNullOrWhiteSpace(y2label))
                lb.IsY2LabelVisible = false;
            else
            {
                lb.IsY2LabelVisible = true;
                lb.Y2Label = y2label;
            }
        }

        public void SetXAxis(float xLimMin, float xLimMax, float xTick)
        {
            xa.XLimMin = xLimMin;
            xa.XLimMax = xLimMax;
            xa.XTick = xTick;
        }
        public void SetYAxis(float yLimMin, float yLimMax, float yTick)
        {
            ya.YLimMin = yLimMin;
            ya.YLimMax = yLimMax;
            ya.YTick = yTick;
        }
        public void SetY2Axis(float y2LimMin, float y2LimMax, float y2Tick)
        {
            y2a.Y2LimMin = y2LimMin;
            y2a.Y2LimMax = y2LimMax;
            y2a.Y2Tick = y2Tick;
        }
        //数据清空
        public void DataClear()
        {
            foreach (DataCollection dc in dclist)
            {
                dc.RemoveAll();
            }
            dclist.Clear();
            //dclist.Add(new DataCollection());
            if (cs.PlotAreaColor.Border == cs.PlotAreaColor.Fill)
                cs.PlotAreaColor.Border = Color.Black;
        }
        //恢复默认值
        public void Default()
        {
            foreach (DataCollection dc in dclist)
            {
                dc.RemoveAll();
            }
            dclist.Clear();
            //dclist.Add(new DataCollection());
            this.ChartStyle = new ChartStyle(this);
            this.Title = new Title();
            this.Label = new XYLabel();
            this.Grid = new Grid();
            this.Legend = new Legend();
            this.XAxis = new XAxis();
            this.YAxis = new YAxis();
            this.Y2Axis = new Y2Axis();
        }

        public Chart2D Clone()
        {
            Chart2D newChart = new Chart2D(this.ChartSize);
            newChart.ChartStyle = this.ChartStyle.Clone();
            newChart.XAxis = this.XAxis.Clone();
            newChart.YAxis = this.YAxis.Clone();
            newChart.Y2Axis = this.Y2Axis.Clone();
            newChart.Grid = this.Grid.Clone();
            newChart.Legend = this.Legend.Clone();
            newChart.Title = this.Title.Clone();
            newChart.Label = this.Label.Clone();

            return newChart;
        }

        #endregion

       
        #region 属性
        public Size ChartSize
        {
            get { return chartSize; }
            set 
            { 
                chartSize = value;
                cs.chartArea = new Rectangle(new Point(0, 0), chartSize);
                cs.XOffset = cs.ChartArea.Width / 30.0f;
                cs.YOffset = cs.ChartArea.Height / 30.0f;
            }
        }

        public bool isAxesFit 
        {
            get { return isAxesfit; }
        }
        //public DataCollection DataCollectionsub
        //{
        //    get { return this.dc_sub; }
        //    set { this.dc_sub = value; }
        //}

        [BrowsableAttribute(false)]
        public ChartStyle ChartStyle
        {
            get { return this.cs; }
            set { this.cs = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public XAxis XAxis
        {
            get { return this.xa; }
            set
            {
                if (value != null)
                {
                    this.xa = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public YAxis YAxis
        {
            get { return this.ya; }
            set
            {
                if (value != null)
                {
                    this.ya = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Y2Axis Y2Axis
        {
            get { return this.y2a; }
            set
            {
                if (value != null)
                {
                    this.y2a = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Grid Grid
        {
            get { return this.gd; }
            set
            {
                if (value != null)
                {
                    this.gd = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public XYLabel Label
        {
            get { return this.lb; }
            set
            {
                if (value != null)
                {
                    this.lb = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Title Title
        {
            get { return this.tl; }
            set
            {
                if (value != null)
                {
                    this.tl = value;
                }
            }
        }

        [BrowsableAttribute(false)]
        public List<DataCollection> DataCollections
        {
            get { return this.dclist; }
            set { this.dclist = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Legend Legend
        {
            get { return this.lg; }
            set
            {
                if (value != null)
                {
                    this.lg = value;
                }
            }
        }
        #endregion


        ////附加背景待定
        //public void AddSubs(Graphics g, ChartStyle cs, DataCollection dc_sub, XAxis xa, YAxis ya, Y2Axis y2a)
        //{
        //    dc_sub.Timeformat = dc.Timeformat;
        //    Color[] dataColors = SeriesColor.GetColors(dc_sub.ColorTable);
        //    foreach (DataSeries ds in dc_sub.DataSeriesList)
        //    {
        //        if (ds.LineChartType == LineCharts.LineChartTypeEnum.Background)
        //        {
        //            DataValue dv = (DataValue)ds.ValueList[0];
        //            DateTime start = TimeConverter.GetTime((int)Math.Round(dv.XValue));
        //            dv = (DataValue)ds.ValueList[1];
        //            DateTime end = TimeConverter.GetTime((int)Math.Round(dv.XValue));
        //            float xstart = 0; float xend = 0;
        //            foreach (float xtick in xa.XTickMark.Keys)
        //            {
        //                DateTime tickTime = DateTime.ParseExact(xa.XTickMark[xtick], dc_sub.Timeformat, System.Globalization.CultureInfo.CurrentCulture);
        //                if (start == tickTime)
        //                    xstart = xtick;
        //                if (end == tickTime)
        //                    xend = xtick;
        //            }
        //            if (xstart != 0 || xend != 0)
        //            {
        //                GraphColor gc = new GraphColor(Color.FromArgb(50, dataColors[(int)dv.YValue]), dataColors[(int)dv.YValue]);
        //                Pen aPen = new Pen(gc.Border, ds.LineStyle.Thickness);
        //                aPen.DashStyle = ds.LineStyle.Pattern;
        //                SolidBrush aBrush = new SolidBrush(gc.BackGround);

        //                PointF ps = cs.Point2D(new PointF(xstart, ya.YLimMax), xa, ya);
        //                PointF pe = cs.Point2D(new PointF(xend, ya.YLimMin), xa, ya);
        //                Rectangle rec = new Rectangle((int)ps.X, (int)ps.Y, (int)(pe.X - ps.X), (int)(pe.Y - ps.Y));
        //                g.DrawRectangle(aPen, rec);
        //                g.FillRectangle(aBrush, rec);
        //            }
        //            else
        //                return;
        //        }
        //        else if (ds.LineChartType == LineCharts.LineChartTypeEnum.Straight)
        //        {

        //        }
        //    }

        //}


    }


}