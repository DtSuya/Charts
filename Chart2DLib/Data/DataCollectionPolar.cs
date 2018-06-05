using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Readearth.Chart2D.Data
{
    public class DataCollectionPolar : DataCollection
    {
        private PolarCharts.PolarChartTypeEnum polarChartType = PolarCharts.PolarChartTypeEnum.Radar;
        private bool isBorderVis = true;
        private float roseBarWidth = 0.9f;
        private bool DataCheck = true;

        public DataCollectionPolar()
        {
        }

        public PolarCharts.PolarChartTypeEnum PolarChartType
        {
            get { return polarChartType; }
            set { polarChartType = value; }
        }

        public override void Add(DataSeries ds)
        {
            ds.LineStyle.IsVisible = isBorderVis;
            this.DataSeriesList.Add(ds);
            if (ds.SeriesName == "")
            {
                ds.SeriesName = "DataSeries" + this.DataSeriesList.Count.ToString();
            }
        }

        public bool IsBorderVis
        {
            get
            {
                if (polarChartType == PolarCharts.PolarChartTypeEnum.Radar || polarChartType == PolarCharts.PolarChartTypeEnum.Spline)
                    isBorderVis = true;
                else if (polarChartType == PolarCharts.PolarChartTypeEnum.RadarPolygon || polarChartType == PolarCharts.PolarChartTypeEnum.Rose)
                    foreach (DataSeries ds in this.DataSeriesList)
                    {
                        if (ds.LineStyle.IsVisible == false)
                            isBorderVis = false;
                    }
                return isBorderVis;
            }
            set
            {
                if (polarChartType == PolarCharts.PolarChartTypeEnum.RadarPolygon || polarChartType == PolarCharts.PolarChartTypeEnum.Rose)
                    isBorderVis = value;
                foreach (DataSeries ds in this.DataSeriesList)
                {
                    ds.LineStyle.IsVisible = isBorderVis;
                }
            }
        }

        public float RoseBarWidth
        {
            get { return roseBarWidth; }
            set 
            {
                if (value > 0 && value <= 1)
                    roseBarWidth = value;
                else
                    roseBarWidth = 0.9f;
            }
        }

        public bool CheckNegative()
        {
            DataCheck = true;
            if (polarChartType == PolarCharts.PolarChartTypeEnum.Rose)
            {
                foreach (DataSeries ds in this.DataSeriesList)
                {
                    foreach (PointF p in ds.PointList)
                    {
                        if (p.Y < 0)
                            DataCheck = false;
                    }
                }
            }
            return DataCheck;
        }

        public override Chart2DTypeEnum ChartType
        {
            get { return Chart2DTypeEnum.PolorChart; }
        }

        public override int GetTypeCount(Enum name)
        {
            Type t = typeof(PolarCharts.PolarChartTypeEnum);
            int count = 0;
            if (!t.IsEnum || !t.IsEnumDefined(name.ToString()))
                return -999;
            else
            {
                if (polarChartType == (PolarCharts.PolarChartTypeEnum)name)
                    count += this.DataSeriesList.Count;
            }
            return count;
        }
    }
}
