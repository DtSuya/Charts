using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Readearth.Chart2D.Data
{
    public class DataCollectionArea : DataCollection
    {
        private AreaCharts.AreaChartTypeEnum areaChartType = AreaCharts.AreaChartTypeEnum.Area;
        private float areaAxis = 0;//起始值

        public DataCollectionArea()
        {
        }

        public AreaCharts.AreaChartTypeEnum AreaChartType
        {
            get { return areaChartType; }
            set { areaChartType = value; }
        }

        public float AreaAxis
        {
            get { return areaAxis; }
            set { areaAxis = value; }
        }

        public override Chart2DTypeEnum ChartType
        {
            get { return Chart2DTypeEnum.AreaChart; }
        }

        public override int GetTypeCount(Enum name)
        {
            Type t = typeof(AreaCharts.AreaChartTypeEnum);
            int count = 0;
            if (!t.IsEnum || !t.IsEnumDefined(name.ToString()))
                return -999;
            else
            {
                if (areaChartType == (AreaCharts.AreaChartTypeEnum)name)
                    count += this.DataSeriesList.Count;
            }
            return count;
        }
    }
}
