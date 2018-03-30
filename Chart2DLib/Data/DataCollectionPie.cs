using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Readearth.Chart2D.Data
{
    public class DataCollectionPie : DataCollection
    {
        private PieCharts.PieChartTypeEnum pieChartType = PieCharts.PieChartTypeEnum.Standard;
        private bool isBorderVis = true;
        private Color borderColor = Color.Black;

        public DataCollectionPie()
        {
        }

        public PieCharts.PieChartTypeEnum PieChartType
        {
            get { return pieChartType; }
            set { pieChartType = value; }
        }

        public bool IsBorderVis
        {
            get
            {
                foreach (DataSeries ds in this.DataSeriesList)
                {
                    if (ds.LineStyle.IsVisible == false)
                        isBorderVis = false;
                }
                return isBorderVis; 
            }
            set 
            { 
                isBorderVis = value;
                foreach (DataSeries ds in this.DataSeriesList)
                {
                    ds.LineStyle.IsVisible = isBorderVis;
                }
            }
        }

        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }

        public override Chart2DTypeEnum ChartType
        {
            get { return Chart2DTypeEnum.PieChart; }
        }

        public override int GetTypeCount(Enum name)
        {
            Type t = typeof(PieCharts.PieChartTypeEnum);
            int count = 0;
            if (!t.IsEnum || !t.IsEnumDefined(name.ToString()))
                return -999;
            else
            {
                if (pieChartType == (PieCharts.PieChartTypeEnum)name)
                    count += this.DataSeriesList.Count;
            }
            return count;
        }
    }
}
