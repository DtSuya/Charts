using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Readearth.Data;

namespace Readearth.Chart2D.Additional
{
    //public class DataSource
    //{
    //    private Chart2D chart2d;
    //    private DataCollection m_dc;
    //    private DataCollection sub_dc;
    //    private DataTable m_dataSource;
    //    private string dsfield = "SeriesName";
    //    private string xname = "XAxis";
    //    private string yname = "YAxis";
    //    private string isY2name = "0";
    //    private Database m_Database;

    //    public DataTable SourceTable
    //    {
    //        get { return m_dataSource; }
    //    }
    //    public string Xname
    //    {
    //        get { return xname; }
    //        set { xname = value; chart2d.Label.XLabel  = value; chart2d.XAxis.XTickMark = new Dictionary<float, string>(); }
    //    }
    //    public string Yname
    //    {
    //        get { return yname; }
    //        set { yname = value; chart2d.Label.YLabel  = value; }
    //    }
    //    public string Y2name
    //    {
    //        get { return isY2name; }
    //        set { isY2name = value; chart2d.Label.Y2Label  = value; }
    //    }

    //    public DataSource(Chart2D ct2d)
    //    {
    //        chart2d = ct2d;
    //        m_dc = chart2d.C2DataCollection ;
    //        sub_dc = chart2d.DataCollectionsub;
    //        m_dataSource = new DataTable();
    //        m_Database = new Database();

    //    }
    //    public void AddBackground(string backName,int grade, DateTime start, DateTime end, bool isVertical)
    //    {
    //        DataSeries sub_ds = new DataSeries();
    //        sub_ds.LineChartType = LineCharts.LineChartTypeEnum.Background;
    //        sub_ds.LineStyle.Pattern = System.Drawing.Drawing2D.DashStyle.Dash;
    //        sub_ds.LineStyle.Thickness = 2f;
    //        sub_ds.SeriesName = backName;
    //        sub_ds.IsY2Data = isVertical;
    //        if (isVertical)
    //        {
    //            sub_ds.AddValue(new DataValue(start, grade, typeof(DateTime)));
    //            sub_ds.AddValue(new DataValue(end, grade, typeof(DateTime)));
    //        }
    //        else
    //        {
    //        }
    //        sub_dc.Add(sub_ds);
    //    }
    //    public void AddStraightLine(string lineName, int grade, float lineValue, bool isVertical)
    //    {
    //        DataSeries sub_ds = new DataSeries();
    //        sub_ds.LineChartType = LineCharts.LineChartTypeEnum.Straight;
    //        sub_ds.LineStyle.Thickness = 2f;
    //        sub_ds.SeriesName = lineName;
    //        sub_ds.IsY2Data = isVertical;
    //        if(!isVertical)
    //            sub_ds.AddValue(new DataValue(grade, lineValue, typeof(double)));
            
    //        sub_dc.Add(sub_ds);
    //    }
    //    //数据源:表（数据列名，横轴数据，纵轴数据，是否列入第二纵轴）
    //    public bool AddData(string sqlselect, LineCharts.LineChartTypeEnum SeriesType)
    //    {
    //        if(string.IsNullOrWhiteSpace(sqlselect))
    //            return false;
    //        if (SeriesType == LineCharts.LineChartTypeEnum.Background || SeriesType == LineCharts.LineChartTypeEnum.Straight)
    //            return false;
    //        m_dataSource.Dispose();
    //        m_dc.DataSeriesList.Clear();
    //        try
    //        {
    //            DataTable Sources = m_Database.GetDataTable(sqlselect);

    //            m_dataSource = Sources;
    //            dsfield = m_dataSource.Columns[0].ColumnName;
    //            Xname = m_dataSource.Columns[1].ColumnName;
    //            Yname = m_dataSource.Columns[2].ColumnName;
    //            Y2name = m_dataSource.Columns[3].ColumnName;

    //            if (m_dataSource.Rows.Count > 0)
    //                return DataBind(SeriesType);
    //            else
    //                return false;
    //        }
    //        catch { return false; }
    //    }
    //    public bool AddData(DataTable Sources, LineCharts.LineChartTypeEnum SeriesType)
    //    {
    //        if (SeriesType == LineCharts.LineChartTypeEnum.Background || SeriesType == LineCharts.LineChartTypeEnum.Straight)
    //            return false;
    //        if (Sources.Rows.Count < 1)
    //            return false;
    //        m_dataSource.Dispose();
    //        m_dc.DataSeriesList.Clear();
    //        try
    //        {
    //            m_dataSource = Sources;
    //            dsfield = m_dataSource.Columns[0].ColumnName;
    //            Xname = m_dataSource.Columns[1].ColumnName;
    //            Yname = m_dataSource.Columns[2].ColumnName;
    //            Y2name = m_dataSource.Columns[3].ColumnName;
    //            if (m_dataSource.Rows.Count > 0)
    //                return DataBind(SeriesType);
    //            else
    //                return false;
    //        }
    //        catch { return false; }
    //    }

    //    public bool AppendData(string sqlselect, LineCharts.LineChartTypeEnum SeriesType)
    //    {
    //        if (string.IsNullOrWhiteSpace(sqlselect))
    //            return false;
    //        if (SeriesType == LineCharts.LineChartTypeEnum.Background || SeriesType == LineCharts.LineChartTypeEnum.Straight)
    //            return false;
    //        try
    //        {
    //            DataTable Sources = m_Database.GetDataTable(sqlselect);

    //            if (Sources.Rows.Count > 0)
    //            {
    //                foreach (DataRow dr in Sources.Rows)
    //                {
    //                    m_dataSource.ImportRow(dr);
    //                }
    //                return DataBind(SeriesType);
    //            }
    //            else
    //                return false;
    //        }
    //        catch { return false; }
    //    }
    //    public bool AppendData(DataTable Sources, LineCharts.LineChartTypeEnum SeriesType)
    //    {
    //        if (SeriesType == LineCharts.LineChartTypeEnum.Background || SeriesType == LineCharts.LineChartTypeEnum.Straight)
    //            return false;
    //        if (Sources.Rows.Count < 1)
    //            return false;
    //        try
    //        {
    //            foreach (DataRow dr in Sources.Rows)
    //            {
    //                m_dataSource.ImportRow(dr);
    //            }
    //            return DataBind(SeriesType);
    //        }
    //        catch { return false; }
    //    }

    //    public void Clear()
    //    {
    //        m_dataSource.Clear();
    //        m_dc.RemoveAll();
    //    }

    //    //添加到数据集
    //    private bool DataBind(LineCharts.LineChartTypeEnum SeriesType)
    //    {
    //        if (m_dataSource.Rows.Count > 1)
    //        {
    //            List<string> dsnames = GetDistinctFields(m_dataSource, dsfield);
    //            List<string> xmarks = GetDistinctFields(m_dataSource, xname);
    //            try
    //            {
    //                foreach (string dsname in dsnames)
    //                {
    //                    DataSeries ds = new DataSeries();
    //                    ds.SeriesName = dsname;
    //                    ds.LineChartType = SeriesType;
    //                    try
    //                    {
    //                        ds.IsY2Data = Convert.ToBoolean(m_dataSource.Select(dsfield + " = '" + dsname + "'")[0][isY2name]);
    //                    }
    //                    catch { }

    //                    foreach (string xmark in xmarks)
    //                    {
    //                        float value = 0;
    //                        try
    //                        {
    //                            value = Convert.ToSingle(m_dataSource.Select(dsfield + " = '" + dsname + "' and " + xname + " = '" + xmark + "'")[0][yname]);
    //                        }
    //                        catch { continue; }
    //                        if (xname.Contains("时间")|| xname.Contains("Time"))
    //                            ds.AddValue(new DataValue(xmark, value, typeof(DateTime)));
    //                        else if (xname.Contains("值"))
    //                            ds.AddValue(new DataValue(xmark, value, typeof(double)));
    //                        else
    //                            ds.AddValue(new DataValue(xmark, value, typeof(string)));
    //                    }
    //                    m_dc.Add(ds);
    //                }
    //            }
    //            catch { return false; }
    //        }
    //        return true;
    //    }

    //    //获取字段唯一值
    //    private List<string> GetDistinctFields(DataTable table, string columnname)
    //    {
    //        List<string> values = new List<string>();
    //        DataView DataView = new DataView(table);
    //        DataView.Sort = columnname + " asc";
    //        DataTable list = DataView.ToTable(true, columnname);
    //        foreach (DataRow dr in list.Rows)
    //        {
    //            values.Add(dr[0].ToString());
    //        }
    //        return values;
    //    }
    //}

}
