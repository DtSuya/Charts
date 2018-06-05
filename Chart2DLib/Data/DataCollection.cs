using System;
using System.Collections;
using System.Drawing;
namespace Readearth.Chart2D.Data
{
    public class DataCollection
    {
        private int dataSeriesIndex;
        private ArrayList dataSeriesList;
        private Chart2DTypeEnum chartType = Chart2DTypeEnum.None;
        private int[,] cmap;
        //private string colorTablename = "BaseColors.txt";//Ä¬ÈÏÅäÉ«°å
        //private string timeformat = "yyyy-MM-dd HH:mm:ss";

        public DataCollection()
        {
            dataSeriesList = new ArrayList();
            chartType = Chart2DTypeEnum.LineChart;
        }
        private DataCollection(Chart2DTypeEnum chart2dType)
        {
            dataSeriesList = new ArrayList();
            chartType = chart2dType;
        }

        public virtual Chart2DTypeEnum ChartType
        {
            get { return chartType; }
            set { chartType = value; }
        }

        public int[,] CMap
        {
            get { return cmap; }
            set
            {
                cmap = value; 
                for (int i = 0; i < cmap.GetLength(0); i++)
                {
                    for (int j = 0; j < cmap.GetLength(1); j++)
                    {
                        if (!(cmap[i, j] >= 0 && cmap[i, j] <= 255))
                            cmap[i, j] = 0;
                    }
                }
                SetSeriesColors();
            }
        }

        //public string ColorTable
        //{
        //    get { return colorTablename; }
        //    set { colorTablename = value; }
        //}

        //public string Timeformat
        //{
        //    get { return timeformat; }
        //    set { timeformat = value; }
        //}

        public ArrayList DataSeriesList
        {
            get { return dataSeriesList; }
            set { dataSeriesList = value; }
        }
        public int DataSeriesIndex
        {
            get { return dataSeriesIndex; }
            set { dataSeriesIndex = value; }
        }

        public virtual void Add(DataSeries ds)
        {
            dataSeriesList.Add(ds);
            if (ds.SeriesName == "")
            {
                ds.SeriesName = "DataSeries" + dataSeriesList.Count.ToString();
            }
        }

        public void Insert(int dataSeriesIndex, DataSeries ds)
        {
            dataSeriesList.Insert(dataSeriesIndex, ds);
            if (ds.SeriesName == "")
            {
                dataSeriesIndex = dataSeriesIndex + 1;
                ds.SeriesName = "DataSeries" + dataSeriesIndex.ToString();
            }
        }

        public void Remove(string dataSeriesName)
        {
            if (dataSeriesList != null)
            {
                for (int i = 0; i < dataSeriesList.Count; i++)
                {
                    DataSeries ds = (DataSeries)dataSeriesList[i];
                    if (ds.SeriesName == dataSeriesName)
                    {
                        dataSeriesList.RemoveAt(i);
                    }
                }
            }
        }

        public void RemoveAll()
        {
            dataSeriesList.Clear();
        }

        public virtual int GetTypeCount(Enum name)
        {
            Type t;
            int count = 0;
            switch (chartType)
            {
                case Chart2DTypeEnum.LineChart:
                    t = typeof(LineCharts.LineChartTypeEnum);
                    if (!t.IsEnum || !t.IsEnumDefined(name.ToString()))
                        return -999;
                    foreach (DataSeries ds in DataSeriesList)
                    {
                        if (ds.LineChartType == (LineCharts.LineChartTypeEnum)name)
                            count++;
                    }
                    break;
                case Chart2DTypeEnum.AreaChart:
                    t = typeof(AreaCharts.AreaChartTypeEnum);
                    if (!t.IsEnum || !t.IsEnumDefined(name.ToString()))
                        return -999;
                    count += dataSeriesList.Count;
                    break;
                case Chart2DTypeEnum.BarChart:
                    t = typeof(BarCharts.BarChartTypeEnum);
                    if (!t.IsEnum || !t.IsEnumDefined(name.ToString()))
                        return -999;
                    count += dataSeriesList.Count;
                    break;
                case Chart2DTypeEnum.PieChart:
                    t = typeof(PieCharts.PieChartTypeEnum);
                    if (!t.IsEnum || !t.IsEnumDefined(name.ToString()))
                        return -999;
                    break;
                case Chart2DTypeEnum.PolorChart:
                    t = typeof(PolarCharts.PolarChartTypeEnum);
                    if (!t.IsEnum || !t.IsEnumDefined(name.ToString()))
                        return -999;
                    break;
                default:
                    return -999;
            }
            return count;
        }

        private void SetSeriesColors()
        {
            for (int n = 0; n < dataSeriesList.Count; n++)
            {
                DataSeries ds = (DataSeries)dataSeriesList[n];
                if (n < cmap.GetLength(0))
                {
                    if(ChartType == Chart2DTypeEnum.LineChart)
                        ds.LineStyle.LineColor = Color.FromArgb(cmap[n, 0], cmap[n, 1], cmap[n, 2], cmap[n, 3]);
                    else if (ChartType == Chart2DTypeEnum.AreaChart || ChartType == Chart2DTypeEnum.BarChart || ChartType == Chart2DTypeEnum.PieChart)
                    {
                        ds.FillColor = Color.FromArgb(cmap[n, 0], cmap[n, 1], cmap[n, 2], cmap[n, 3]);
                        //ds.LineStyle.LineColor = Color.Black;
                    }
                    else if (ChartType == Chart2DTypeEnum.PolorChart)
                    {
                        DataCollectionPolar dc = this as DataCollectionPolar;
                        if(dc.PolarChartType == PolarCharts.PolarChartTypeEnum.Radar || dc.PolarChartType == PolarCharts.PolarChartTypeEnum.Spline)
                            ds.LineStyle.LineColor = Color.FromArgb(cmap[n, 0], cmap[n, 1], cmap[n, 2], cmap[n, 3]);
                        else if(dc.PolarChartType == PolarCharts.PolarChartTypeEnum.RadarPolygon || dc.PolarChartType == PolarCharts.PolarChartTypeEnum.Rose)
                            ds.FillColor = Color.FromArgb(cmap[n, 0], cmap[n, 1], cmap[n, 2], cmap[n, 3]);
                    }

                }
                else
                {
                    if (ChartType == Chart2DTypeEnum.LineChart)
                        ds.LineStyle.LineColor = Color.Black;
                    else if (ChartType == Chart2DTypeEnum.AreaChart || ChartType == Chart2DTypeEnum.BarChart || ChartType == Chart2DTypeEnum.PieChart || ChartType == Chart2DTypeEnum.PolorChart)
                    {
                        ds.LineStyle.LineColor = Color.Black;
                        ds.FillColor = Color.Red;
                    }
                }
            }
 
        }
    }
}
