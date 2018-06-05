using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Readearth.Chart2D.Additional
{
    static class DataDeal
    {
        public static float FloatAccur(float data)
        {
            float reviseData = 0;
            if (Math.Abs(data - Math.Round(data, 7)) < 0.000002)
                reviseData = (float)Math.Round(data, 6);
            else
                reviseData = data;
            return reviseData;
        }
    }
}
