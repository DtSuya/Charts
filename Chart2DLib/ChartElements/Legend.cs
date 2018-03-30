using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Readearth.Chart2D.BasicStyle;
using Readearth.Chart2D.Data;

namespace Readearth.Chart2D.ChartElements
{
    [TypeConverter(typeof(LegendConverter))]
    public class Legend
    {
        //private Chart2D chart2d;
        private bool isLegendVisible;
        private bool isInside;
        private LegendPositionEnum legendPosition;
        private bool isVertical;
        private bool isBorderVisible;
        private GraphColor legendColor;
        private TextStyle textStyle;
        public float xLgOffSet { get; set; }
        public float yLgOffSet { get; set; }

        public Legend()
        {
            //chart2d = ct2d;
            isLegendVisible = true;
            isInside = true; 
            legendPosition = LegendPositionEnum.NorthEast;
            isVertical = true;
            isBorderVisible = true;
            legendColor = new GraphColor();
            textStyle = new TextStyle();
            textStyle.TextSize = 8f;
            xLgOffSet = 8f;
            yLgOffSet = 8f;
        }
        #region 属性
        [Description("Background&Border colors color of the legend box."),
        Category("Appearance")]
        public GraphColor LegendColor
        {
            get { return legendColor; }
            set
            {
                legendColor = value;
            }
        }

        [Description("Indicates whether the legend border should be shown."),
        Category("Appearance")]
        public bool IsBorderVisible
        {
            get { return isBorderVisible; }
            set
            {
                isBorderVisible = value;
            }
        }

        [Description("Specifies the legend position in the chart ."),
        Category("Appearance")]
        public LegendPositionEnum LegendPosition
        {
            get { return legendPosition; }
            set
            {
                legendPosition = value;
                if (legendPosition == LegendPositionEnum.North || legendPosition == LegendPositionEnum.South)
                    isVertical = false;
                else
                    isVertical = true;
                //chart2d.AddChart();
            }
        }

        [Description("Style of the legend text."),
        Category("Appearance")]
        public TextStyle LgTextStyle
        {
            get { return textStyle; }
            set
            {
                textStyle = value;
                //chart2d.AddChart();
            }
        }

        [Description("Indicates whether the legend is shown in the chart."),
        Category("Appearance")]
        public bool IsLegendVisible
        {
            get { return isLegendVisible; }
            set
            {
                isLegendVisible = value;
                //chart2d.AddChart();
            }
        }

        public enum LegendPositionEnum
        {
            [Description("右侧")]
            East,
            [Description("左侧")]
            West,
            [Description("底部")]
            South,
            [Description("顶部")]
            North,
            [Description("右上角")]
            NorthEast,
            [Description("右下角")]
            SouthEast,
            [Description("左上角")]
            NorthWest,
            [Description("左下角")]
            SouthWest
        }

        public bool IsVertical
        {
            get { return isVertical; }
            //set { isVertical = value; }
            
        }

        //直角坐标下设置有效（默认内部），极坐标下置于外部
        public bool IsInside
        {
            get { return isInside; }
            set { isInside = value; 
                //chart2d.AddChart(); 
            }
        }
        #endregion

        //legendWidthO = 图例自身legendWidth + xOffset
        //legendHeightO = 图例自身legendHeight + yOffset
        internal void GetLegendSize(Graphics g, List<DataCollection> dclist, ChartStyle cs, out float legendWidthO, out float legendHeightO)
        {
            SizeF size = g.MeasureString("A", LgTextStyle.TextFont);
            int numberOfDataSeries = 0;
            foreach (DataCollection dc in dclist)
            {
                numberOfDataSeries += dc.DataSeriesList.Count;
            }
            if (isLegendVisible == false || numberOfDataSeries < 1)
            {
                legendWidthO = legendHeightO = 0;
                return;
            }
            string[] legendLabels = new string[numberOfDataSeries];
            int n = 0;
            foreach (DataCollection dc in dclist)
            {
                foreach (DataSeries ds in dc.DataSeriesList)
                {
                    legendLabels[n] = ds.SeriesName;
                    n++;
                }
            }

            if (IsVertical)
            {
                float aWidth = size.Width;
                for (int i = 0; i < legendLabels.Length; i++)
                {
                    size = g.MeasureString(legendLabels[i], LgTextStyle.TextFont);
                    float tempWidth = size.Width;
                    if (aWidth < tempWidth)
                        aWidth = tempWidth;
                }
                legendWidthO = aWidth + 3 * size.Height * 0.28f + 20 + xLgOffSet;//图例自身 = 字长+3个间隔(0.28倍行高)+符号长20, 原：aWidth + 30.0f
                legendHeightO = numberOfDataSeries * size.Height * 1.3f + yLgOffSet;//图例自身 = 行数*字高*1.3(倍行距), 原：18.0f(固定行高) * numberOfDataSeries
            }
            else
            {
                float legendWidth = 0;
                if (isInside)
                    legendWidth = cs.PlotArea.Width - xLgOffSet;
                else
                    legendWidth = cs.ChartArea.Width - 2 * cs.XOffset - xLgOffSet ;

                //图例文字总长度
                float totallength = size.Height * 0.28f;//第一个间隔
                foreach (string legendLabel in legendLabels)
                {
                    SizeF sizeT = g.MeasureString(legendLabel, LgTextStyle.TextFont);
                    float alength = sizeT.Width + size.Height * 0.28f * 2 + 20;
                    totallength += alength;
                }
                
                //求行数
                int row = 1;
                if (totallength < legendWidth)
                {
                    legendWidth = totallength;
                }
                else
                {
                    float length = 0;
                    for (int i = 0; i < numberOfDataSeries; i++)
                    {
                        SizeF sizeT = g.MeasureString(legendLabels[i], LgTextStyle.TextFont);
                        float alength = sizeT.Width + size.Height * 0.28f * 2 + 20;
                        if (length + alength > legendWidth)
                        {
                            row++;
                            length = 0;
                        }
                        length += alength;
                    }
                }
                legendHeightO = size.Height * 1.3f * row + yLgOffSet;
                legendWidthO = legendWidth + xLgOffSet;
            }
        }

        internal void AddLegend(Graphics g, List<DataCollection> dclist, ChartStyle cs, Title tl)
        {
            if (!IsLegendVisible)
            {
                return;
            }
            float legendWidthO; float legendHeightO;
            GetLegendSize(g, dclist, cs, out legendWidthO, out legendHeightO);
            if (legendWidthO == 0 || legendHeightO == 0)
                return;
            float hWidth = (legendWidthO - xLgOffSet) / 2;
            float hHeight = (legendHeightO - yLgOffSet) / 2;

            //确定图例中心点(图例位置)
            float xc = 0f;
            float yc = 0f;
            float xshift = xLgOffSet + hWidth;
            float yshift = yLgOffSet + hHeight;
            if (isInside)
            {
                switch (LegendPosition)
                {
                    case LegendPositionEnum.NorthWest:
                        xc = cs.PlotArea.Left + xshift;
                        yc = cs.PlotArea.Top + yshift;
                        break;
                    case LegendPositionEnum.North:
                        xc = cs.PlotArea.Left + cs.PlotArea.Width / 2;
                        yc = cs.PlotArea.Top + yshift;
                        break;
                    case LegendPositionEnum.NorthEast:
                        xc = cs.PlotArea.Right - xshift;
                        yc = cs.PlotArea.Top + yshift;
                        break;
                    case LegendPositionEnum.East:
                        xc = cs.PlotArea.Right - xshift;
                        yc = cs.PlotArea.Top + cs.PlotArea.Height / 2;
                        break;
                    case LegendPositionEnum.SouthEast:
                        xc = cs.PlotArea.Right - xshift;
                        yc = cs.PlotArea.Bottom - yshift;
                        break;
                    case LegendPositionEnum.South:
                        xc = cs.PlotArea.Left + cs.PlotArea.Width / 2;
                        yc = cs.PlotArea.Bottom - yshift;
                        break;
                    case LegendPositionEnum.SouthWest:
                        xc = cs.PlotArea.Left + xshift;
                        yc = cs.PlotArea.Bottom - yshift;
                        break;
                    case LegendPositionEnum.West:
                        xc = cs.PlotArea.Left + xshift;
                        yc = cs.PlotArea.Top + cs.PlotArea.Height / 2;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (LegendPosition)
                {
                    case LegendPositionEnum.NorthWest:
                        xc = cs.ChartArea.Left + cs.XOffset + xshift;
                        yc = cs.ChartArea.Top + cs.YOffset + yshift;
                        break;
                    case LegendPositionEnum.North:
                        SizeF tlsize = g.MeasureString("A", tl.TitleStyle.TextFont);
                        xc = cs.ChartArea.Left + cs.ChartArea.Width / 2;
                        yc = cs.ChartArea.Top + cs.YOffset + tlsize.Height + yshift;
                        break;
                    case LegendPositionEnum.NorthEast:
                        xc = cs.ChartArea.Right - cs.XOffset - xshift;
                        yc = cs.ChartArea.Top + cs.YOffset + yshift;
                        break;
                    case LegendPositionEnum.East:
                        xc = cs.ChartArea.Right - cs.XOffset - xshift;
                        yc = cs.ChartArea.Top + cs.ChartArea.Height / 2;
                        break;
                    case LegendPositionEnum.SouthEast:
                        xc = cs.ChartArea.Right - cs.XOffset - xshift;
                        yc = cs.ChartArea.Bottom - cs.YOffset - yshift;
                        break;
                    case LegendPositionEnum.West:
                        xc = cs.ChartArea.Left + cs.XOffset + xshift;
                        yc = cs.ChartArea.Top + cs.ChartArea.Height / 2;
                        break;
                    case LegendPositionEnum.SouthWest:
                        xc = cs.ChartArea.Left + cs.XOffset + xshift;
                        yc = cs.ChartArea.Bottom - cs.YOffset - yshift;
                        break;
                    case LegendPositionEnum.South:
                        xc = cs.ChartArea.Left + cs.ChartArea.Width / 2;
                        yc = cs.ChartArea.Bottom - cs.YOffset - yshift;
                        break;
                    default:
                        break;
                }
            }
            DrawLegend(g, xc, yc, hWidth, hHeight, dclist, cs);
        }

        private void DrawLegend(Graphics g, float xCenter, float yCenter,
            float hWidth, float hHeight, List<DataCollection> dclist, ChartStyle cs)
        {
            SizeF size = g.MeasureString("A", LgTextStyle.TextFont);
            float spacing = size.Height * 0.28f;
            //float textHeight = 9.0f; 
            //float spacing = 8.0f; //行间距
            float lineLength = 20.0f; //线符号长
            float htextHeight = size.Height / 2.0f;//半行高（点符号y）
            float hlineLength = lineLength / 2.0f; //半长（点符号x）
            Rectangle legendRectangle;
            Pen aPen = new Pen(LegendColor.Border, 1f);
            SolidBrush aBrush = new SolidBrush(LegendColor.Fill);

            if (isLegendVisible)
            {
                legendRectangle = new Rectangle((int)xCenter - (int)hWidth, (int)yCenter - (int)hHeight,
                    (int)(2.0f * hWidth), (int)(2.0f * hHeight));
                g.FillRectangle(aBrush, legendRectangle);
                if (IsBorderVisible)
                {
                    g.DrawRectangle(aPen, legendRectangle);
                }

                if (IsVertical)
                {
                    int row = 1;
                    foreach (DataCollection dc in dclist)
                    {
                        if(dc.ChartType == Chart2DTypeEnum.LineChart)
                        {
                            foreach (DataSeries ds in dc.DataSeriesList)
                            {
                                // Draw lines and symbols:
                                float xSymbol = legendRectangle.X + spacing + hlineLength;
                                float xText = legendRectangle.X + 2 * spacing + lineLength;
                                float yText = legendRectangle.Y + row * spacing + (2 * row - 1) * htextHeight;
                                if (ds.LineStyle.IsVisible)
                                {
                                    aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.LineThickness);
                                    aPen.DashStyle = ds.LineStyle.LinePattern;
                                    PointF ptStart = new PointF(legendRectangle.X + spacing, yText);
                                    PointF ptEnd = new PointF(legendRectangle.X + spacing + lineLength, yText);
                                    g.DrawLine(aPen, ptStart, ptEnd);
                                }
                                ds.SymbolStyle.DrawSymbol(g, new PointF(xSymbol, yText));
                                // Draw text:
                                StringFormat sFormat = new StringFormat();
                                sFormat.Alignment = StringAlignment.Near;
                                g.DrawString(ds.SeriesName, LgTextStyle.TextFont, new SolidBrush(LgTextStyle.TextColor),
                                                new PointF(xText, yText - htextHeight), sFormat);
                                row++;
                            }
                        }
                        else if (dc.ChartType == Chart2DTypeEnum.AreaChart || dc.ChartType == Chart2DTypeEnum.BarChart || dc.ChartType == Chart2DTypeEnum.PieChart)
                        {
                            int n = 0;
                            foreach (DataSeries ds in dc.DataSeriesList)
                            {
                                // Draw rectangles:
                                if(ds.FillColor != Color.Empty)
                                    aBrush = new SolidBrush(ds.FillColor);
                                else
                                    aBrush = new SolidBrush(Color.FromArgb(150, ds.LineStyle.LineColor));

                                float xText = legendRectangle.X + 2 * spacing + lineLength;
                                float yText = legendRectangle.Y + row * spacing + 2 * (row - 1) * htextHeight;

                                RectangleF dsRectangle = new RectangleF(legendRectangle.X + spacing, yText + size.Height / 6, lineLength, size.Height * 2 / 3);
                                g.FillRectangle(aBrush, dsRectangle);
                                if (ds.LineStyle.IsVisible)
                                {
                                    aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.LineThickness);
                                    aPen.DashStyle = ds.LineStyle.LinePattern;
                                    g.DrawRectangle(aPen, dsRectangle.X, dsRectangle.Y, dsRectangle.Width, dsRectangle.Height);
                                }
                                // Draw text:
                                StringFormat sFormat = new StringFormat();
                                sFormat.Alignment = StringAlignment.Near;
                                g.DrawString(ds.SeriesName, LgTextStyle.TextFont, new SolidBrush(LgTextStyle.TextColor),
                                                new PointF(xText, yText), sFormat);
                                n++;
                                row++;
                            }
                        }
                    }
                }
                else
                {
                    int row = 1; int col = 1; float length = 0;
                    foreach (DataCollection dc in dclist)
                    {
                        if (dc.ChartType == Chart2DTypeEnum.LineChart)
                        {
                            for (int i = 0; i < dc.DataSeriesList.Count; i++)
                            {
                                DataSeries ds = (DataSeries)dc.DataSeriesList[i];
                                SizeF SeriesName = g.MeasureString(ds.SeriesName, LgTextStyle.TextFont);
                                float alength = SeriesName.Width + spacing * 2 + lineLength;
                                if (length + alength < legendRectangle.Width)
                                    col++;
                                else
                                {
                                    row++;
                                    length = 0;
                                }
                                // Draw lines and symbols:
                                float xSymbol = legendRectangle.X + length + spacing + hlineLength;
                                float xText = legendRectangle.X + length + spacing * 2 + lineLength;
                                float yText = legendRectangle.Y + row * spacing + (2 * row - 1) * htextHeight;
                                if (ds.LineStyle.IsVisible)
                                {
                                    aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.LineThickness);
                                    aPen.DashStyle = ds.LineStyle.LinePattern;
                                    PointF ptStart = new PointF(legendRectangle.X + length + spacing, yText);
                                    PointF ptEnd = new PointF(legendRectangle.X + length + spacing + lineLength, yText);
                                    g.DrawLine(aPen, ptStart, ptEnd);
                                }
                                ds.SymbolStyle.DrawSymbol(g, new PointF(xSymbol, yText));
                                // Draw text:
                                StringFormat sFormat = new StringFormat();
                                sFormat.Alignment = StringAlignment.Near;
                                g.DrawString(ds.SeriesName, LgTextStyle.TextFont, new SolidBrush(LgTextStyle.TextColor),
                                                new PointF(xText, yText - htextHeight), sFormat);

                                length += alength;
                            }
                        }
                        else if (dc.ChartType == Chart2DTypeEnum.AreaChart || dc.ChartType == Chart2DTypeEnum.BarChart || dc.ChartType == Chart2DTypeEnum.PieChart)
                        {
                            int n = 0;
                            for (int i = 0; i < dc.DataSeriesList.Count; i++)
                            {
                                DataSeries ds = (DataSeries)dc.DataSeriesList[i];
                                SizeF SeriesName = g.MeasureString(ds.SeriesName, LgTextStyle.TextFont);
                                float alength = SeriesName.Width + spacing * 2 + lineLength;
                                if (length + alength < legendRectangle.Width)
                                    col++;
                                else
                                {
                                    row++;
                                    length = 0;
                                }
                                // Draw rectangles: 
                                if(ds.FillColor != Color.Empty)
                                    aBrush = new SolidBrush(ds.FillColor);
                                else
                                    aBrush = new SolidBrush(Color.FromArgb(150, ds.LineStyle.LineColor));
                                //Color fillColor = Color.FromArgb(dc.CMap[n, 0], dc.CMap[n, 1],
                                //    dc.CMap[n, 2], dc.CMap[n, 3]);
                                //aBrush = new SolidBrush(fillColor);

                                float xText = legendRectangle.X + length + 2 * spacing + lineLength;
                                float yText = legendRectangle.Y + row * spacing + 2 * (row - 1) * htextHeight;

                                RectangleF dsRectangle = new RectangleF(xText - spacing - lineLength, yText + size.Height / 6, lineLength, size.Height * 2 / 3);
                                g.FillRectangle(aBrush, dsRectangle);
                                if (ds.LineStyle.IsVisible)
                                {
                                    aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.LineThickness);
                                    aPen.DashStyle = ds.LineStyle.LinePattern;
                                    g.DrawRectangle(aPen, dsRectangle.X, dsRectangle.Y, dsRectangle.Width, dsRectangle.Height);
                                }
                                // Draw text:
                                StringFormat sFormat = new StringFormat();
                                sFormat.Alignment = StringAlignment.Near;
                                g.DrawString(ds.SeriesName, LgTextStyle.TextFont, new SolidBrush(LgTextStyle.TextColor),
                                                new PointF(xText, yText), sFormat);
                                n++;
                                length += alength;
                            }
                        }
                    }
                }
            }
            aPen.Dispose();
            aBrush.Dispose();
        }

        public Legend Clone()
        {
            Legend newLG = new Legend();
            newLG.IsLegendVisible = this.IsLegendVisible;
            newLG.IsInside = this.IsInside;
            newLG.LegendPosition = this.LegendPosition;
            newLG.xLgOffSet = this.xLgOffSet;
            newLG.yLgOffSet = this.yLgOffSet;
            newLG.LegendColor = this.LegendColor.Clone();
            newLG.IsBorderVisible = this.IsBorderVisible;
            newLG.LgTextStyle = this.LgTextStyle.Clone();
            newLG.isVertical = this.IsVertical;
            return newLG;
        }
    }

    public class LegendConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(Legend));
        }
    }
}
