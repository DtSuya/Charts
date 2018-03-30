using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Readearth.Chart2D.Data
{
    public class DataCollectionBar : DataCollection
    {
        private BarCharts.BarChartTypeEnum barChartType = BarCharts.BarChartTypeEnum.Vertical;
        private float barWidth = 0.8f;

        public DataCollectionBar()
        {
        }

        public BarCharts.BarChartTypeEnum BarChartType
        {
            get { return barChartType; }
            set { barChartType = value; }
        }

        public float BarWidth
        {
            get { return barWidth; }
            set { barWidth = value; }
        }

        public override Chart2DTypeEnum ChartType
        {
            get { return Chart2DTypeEnum.BarChart; }
        }


        public override int GetTypeCount(Enum name)
        {
            Type t= typeof(BarCharts.BarChartTypeEnum);
            int count = 0;
            if (!t.IsEnum || !t.IsEnumDefined(name.ToString()))
                return -999;
            else
            {
                if (barChartType == (BarCharts.BarChartTypeEnum)name)
                    count += this.DataSeriesList.Count;
            }
            return count;
        }
    }
}
