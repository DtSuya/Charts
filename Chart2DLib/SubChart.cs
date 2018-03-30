using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using Readearth.Chart2D.BasicStyle;

namespace Readearth.Chart2D
{
    public class SubChart
    {
        private int rows = 1;
        private int cols = 1;
        private int margin = 0;
        private GraphColor totalChartColor = new GraphColor();
        private Rectangle totalChartArea;
        private Rectangle[,] subRectangles;
        private Chart2D[,] subCharts;

        /// <summary>
        /// 初始化默认组图容器（默认长宽为800*500，行列数均为1，边距为0）
        /// </summary>
        public SubChart()
        {
            totalChartArea = new Rectangle(0, 0, 500, 800);
            totalChartColor = new GraphColor(Color.White, Color.White);
            subCharts = new Chart2D[rows, cols];
            subRectangles = SetSubChart();
        }
        /// <summary>
        /// 初始化指定大小的组图容器
        /// </summary>
        /// <param name="subSize"></param>
        public SubChart(Size subSize)
        {
            totalChartArea = new Rectangle(0, 0, subSize.Width, subSize.Height);
            totalChartColor = new GraphColor(Color.White, Color.White);
            subCharts = new Chart2D[rows, cols];
            subRectangles = SetSubChart();
        }
        /// <summary>
        /// 初始化指定大小、指定行列数的组图容器
        /// </summary>
        /// <param name="subSize"></param>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        public SubChart(Size subSize, int nRow, int nCol)
        {
            totalChartArea = new Rectangle(0, 0, subSize.Width, subSize.Height);
            totalChartColor = new GraphColor(Color.White, Color.White);
            rows = nRow;
            cols = nCol;
            subCharts = new Chart2D[rows, cols];
            subRectangles = SetSubChart();
        }

        /// <summary>
        /// 所含Chart2D数组
        /// </summary>
        public Chart2D[,] SubCharts
        {
            get { return subCharts; }
            set { subCharts = value;  }
        }

        /// <summary>
        /// 行数
        /// </summary>
        public int Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                subRectangles = SetSubChart();
            }
        }

        /// <summary>
        /// 列数
        /// </summary>
        public int Cols
        {
            get { return cols; }
            set
            {
                cols = value;
                subRectangles = SetSubChart();
            }
        }

        /// <summary>
        /// 边距
        /// </summary>
        public int Margin
        {
            get { return margin; }
            set
            {
                margin = value;
                subRectangles = SetSubChart();
            }
        }

        /// <summary>
        /// 总绘图区
        /// </summary>
        public Rectangle TotalChartArea
        {
            get { return totalChartArea; }
        }

        /// <summary>
        /// 总绘图区颜色
        /// </summary>
        public GraphColor TotalChartColor
        {
            get { return totalChartColor; }
            set { totalChartColor = value; }
        }

        /// <summary>
        /// 设置当前对象的大小
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public void SetSubChartSize(int height, int width)
        {
            totalChartArea = new Rectangle(0, 0, width, height); 
            subRectangles = SetSubChart();
        }

        private Rectangle[,] SetSubChart()
        {
            Rectangle[,] subRectangle = new Rectangle[Rows, Cols];
            int subWidth = (TotalChartArea.Width - 2 * Margin) / Cols;
            int subHeight= (TotalChartArea.Height- 2 * Margin) / Rows;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    int x = TotalChartArea.X + Margin + j * subWidth;
                    int y = TotalChartArea.Y + Margin + i * subHeight;
                    subRectangle[i, j] = new Rectangle(x, y, subWidth, subHeight);
                    if(subCharts[i, j] != null)
                        subCharts[i, j].SetChartSize(subHeight, subWidth);
                }
            }
            return subRectangle;
        }

        /// <summary>
        /// 获取单个chart的大小
        /// </summary>
        /// <returns></returns>
        public Size GetSingleChartSize()
        {
            int subWidth = (TotalChartArea.Width - 2 * Margin) / Cols;
            int subHeight = (TotalChartArea.Height - 2 * Margin) / Rows;
            Size iSize = new Size(subWidth, subHeight);
            return iSize;
        }

        /// <summary>
        /// 获取当前所含Chart2D数组绘制的Bitmap
        /// </summary>
        /// <returns></returns>
        public Bitmap AddSubCharts()
        {
            Bitmap bmp = new Bitmap(totalChartArea.Width, totalChartArea.Height);
            Graphics g = Graphics.FromImage(bmp);
            // Draw total chart area:
            Pen aPen = new Pen(TotalChartColor.Border, 1f);
            SolidBrush aBrush = new SolidBrush(TotalChartColor.Fill);
            g.FillRectangle(aBrush, TotalChartArea);
            g.DrawRectangle(aPen, TotalChartArea);
            aPen.Dispose();
            aBrush.Dispose();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (subCharts[i, j] != null)
                    {
                        Bitmap chartbmp = subCharts[i, j].AddChart();
                        g.DrawImage(chartbmp, subRectangles[i, j], subCharts[i, j].ChartStyle.ChartArea, GraphicsUnit.Pixel);
                    }
                }
            }
            g.Dispose();
            return bmp;
        }

        /// <summary>
        /// 添加Chart2D数组，并返回当前所含Chart2D数组绘制的Bitmap
        /// </summary>
        /// <param name="charts"></param>
        /// <returns></returns>
        public Bitmap AddSubCharts(Chart2D[,] charts)
        {
            if (charts == null)
                return null;
            else if (charts.GetLength(0) > rows || charts.GetLength(1) > cols)
                return null;

            Bitmap bmp = new Bitmap(totalChartArea.Width, totalChartArea.Height);
            Graphics g = Graphics.FromImage(bmp);
            // Draw total chart area:
            Pen aPen = new Pen(TotalChartColor.Border, 1f);
            SolidBrush aBrush = new SolidBrush(TotalChartColor.Fill);
            g.FillRectangle(aBrush, TotalChartArea);
            g.DrawRectangle(aPen, TotalChartArea);
            aPen.Dispose();
            aBrush.Dispose();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    subCharts[i, j] = charts[i, j];
                    if (subCharts[i, j] != null)
                    {
                        Bitmap chartbmp = subCharts[i, j].AddChart();
                        g.DrawImage(chartbmp, subRectangles[i, j], subCharts[i, j].ChartStyle.ChartArea, GraphicsUnit.Pixel);
                    }
                }
            }
            g.Dispose();
            return bmp;
        }

        /// <summary>
        /// 将一个Chart2D添加到指定位置，并返回当前所含Chart2D数组绘制的Bitmap
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        /// <returns></returns>
        public Bitmap AddSubChart(Chart2D chart, int rowNum, int colNum)
        {
            if (rowNum >= rows || colNum >= cols)
                return null;
            else
                subCharts[rowNum, colNum] = chart;

            Bitmap bmp = new Bitmap(totalChartArea.Width, totalChartArea.Height);
            Graphics g = Graphics.FromImage(bmp);
            // Draw total chart area:
            Pen aPen = new Pen(TotalChartColor.Border, 1f);
            SolidBrush aBrush = new SolidBrush(TotalChartColor.Fill);
            g.FillRectangle(aBrush, TotalChartArea);
            g.DrawRectangle(aPen, TotalChartArea);
            aPen.Dispose();
            aBrush.Dispose();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (subCharts[i, j] != null)
                    {
                        Bitmap chartbmp = subCharts[i, j].AddChart();
                        g.DrawImage(chartbmp, subRectangles[i, j], chart.ChartStyle.ChartArea, GraphicsUnit.Pixel);
                    }
                }
            }
            g.Dispose();
            return bmp;
        }

    }
}
