using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Data;
using Readearth.Chart2D.Data;

namespace Readearth.Chart2D.BasicStyle
{
    public class GraphColor
    {
        private Color fill = Color.White;
        private Color border = Color.Black;

        public GraphColor()
        { }
        public GraphColor(Color fillColor, Color borderColor)
        {
            Fill = fillColor;
            Border = borderColor;
        }

        public Color Fill
        {
            get { return fill; }
            set { fill = value; }
        }
        public Color Border
        {
            get { return border; }
            set { border = value; }
        }

        public GraphColor Clone()
        {
            GraphColor newGC = new GraphColor(this.Fill, this.Border);
            return newGC;
        }
    }

    //static class SeriesColor
    //{
    //    //设置数据列颜色
    //    private static Color[] dataColors;

    //    //数据集统一配色
    //    private static void SetSeriesColor(DataCollection dc)
    //    {
    //        //dataColors = GetColors(dc.ColorTable);

    //        if (dataColors != null)
    //        {
    //            int n = 0;
    //            foreach (DataSeries ds in dc.DataSeriesList)
    //            {
    //                if (ds.RenderType == RenderType.ColorBySeries)
    //                {
    //                    ds.SymbolStyle.SymbolColor.Border = ds.LineStyle.LineColor = dataColors[n];
    //                    n++;
    //                    if (n >= dataColors.Length)
    //                        n = 0;
    //                }
    //            }
    //        }
    //    }

    //    private static void SetSeriesColor(Color[] dataColors, DataCollection dc)
    //    {
    //        if (dataColors != null)
    //        {
    //            int n = 0;
    //            foreach (DataSeries ds in dc.DataSeriesList)
    //            {
    //                if (ds.RenderType == RenderType.ColorBySeries)
    //                {
    //                    ds.SymbolStyle.SymbolColor.Border = ds.LineStyle.LineColor = dataColors[n];
    //                    n++;
    //                    if (n >= dataColors.Length)
    //                        n = 0;
    //                }
    //            }
    //        }
    //    }

    //    //自定义配色板
    //    private static Color[] GetColors(string tablename)
    //    {
    //        string path = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
    //        path = path.Remove(path.LastIndexOf('\\')).TrimEnd('\\');
    //        string txtpath = path.Remove(path.LastIndexOf('\\')) + "\\ColorTable\\" + tablename;
    //        char sp = ' ';
    //        List<Color> list = new List<Color>();
    //        try
    //        {
    //            String line;
    //            String[] split = null;
    //            DataRow row = null;
    //            StreamReader sr = new StreamReader(txtpath, System.Text.Encoding.UTF8);
    //            //创建与数据源对应的数据列 
    //            line = sr.ReadLine();
    //            split = line.Split(sp);

    //            //将数据填入数据表 
    //            int j = 0;
    //            while ((line = sr.ReadLine()) != null)
    //            {
    //                j = 0;
    //                split = line.Split(sp);
    //                list.Add(Color.FromArgb(Convert.ToInt32(split[1]), Convert.ToInt32(split[2]), Convert.ToInt32(split[3]), Convert.ToInt32(split[4])));
    //            }
    //            sr.Close();
    //        }
    //        catch (Exception vErr)
    //        {
    //            return null;
    //        }
    //        if (list.Count > 0)
    //        {
    //            Color[] colors = new Color[list.Count];
    //            for (int i = 0; i < list.Count; i++)
    //            {
    //                colors[i] = list[i];
    //            }
    //            return colors;
    //        }
    //        else
    //            return null;
    //    }
    //}

}
