using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Readearth.Chart2D.BasicStyle
{
    public class SymbolStyle
    {
        private SymbolTypeEnum symbolType;
        private float symbolSize;
        private GraphColor symbolColor;
        private float borderThickness;

        public SymbolStyle()
        {
            symbolType = SymbolTypeEnum.None;
            symbolSize = 6.0f;
            symbolColor = new GraphColor();
            borderThickness = 1f;
        }

        public float BorderThickness
        {
            get { return borderThickness; }
            set { borderThickness = value; }
        }

        public GraphColor SymbolColor
        {
            get { return symbolColor; }
            set { symbolColor = value; }
        }

        public float SymbolSize
        {
            get { return symbolSize; }
            set { symbolSize = value; }
        }

        public SymbolTypeEnum SymbolType
        {
            get { return symbolType; }
            set { symbolType = value; }
        }

        public enum SymbolTypeEnum
        {
            [Description("��")]
            None = 0,
            [Description("������")]
            Box = 1,
            [Description("����")]
            Circle = 2,
            [Description("бʮ��")]
            Cross = 3,
            [Description("����")]
            Diamond = 4,
            [Description("Բ��")]
            Dot = 5,
            [Description("������")]
            InvertedTriangle = 6,
            [Description("�ο�����")]
            OpenDiamond = 7,
            [Description("�οյ�����")]
            OpenInvertedTriangle = 8,
            [Description("�ο�����")]
            OpenTriangle = 9,
            [Description("�ο�������")]
            Square = 10,
            [Description("��������")]
            Star = 11,
            [Description("������")]
            Triangle = 12,
            [Description("��ʮ��")]
            Plus = 13
        }

        internal void DrawSymbol(Graphics g, PointF pt)
        {
            Pen aPen = new Pen(SymbolColor.Border, BorderThickness);
            SolidBrush aBrush = new SolidBrush(SymbolColor.Fill);
            float x = pt.X;
            float y = pt.Y;
            float size = SymbolSize;
            float halfSize = size / 2.0f;
            RectangleF aRectangle = new RectangleF(x - halfSize, y - halfSize, size, size);
       
            switch (SymbolType)
            {
                case SymbolTypeEnum.None:
                    break;
                case SymbolTypeEnum.Square:
                    g.DrawLine(aPen, x - halfSize, y - halfSize, x + halfSize, y - halfSize);
                    g.DrawLine(aPen, x + halfSize, y - halfSize, x + halfSize, y + halfSize);
                    g.DrawLine(aPen, x + halfSize, y + halfSize, x - halfSize, y + halfSize);
                    g.DrawLine(aPen, x - halfSize, y + halfSize, x - halfSize, y - halfSize);
                    break;
                case SymbolTypeEnum.OpenDiamond:
                    g.DrawLine(aPen, x, y - halfSize, x + halfSize, y);
                    g.DrawLine(aPen, x + halfSize, y, x, y + halfSize);
                    g.DrawLine(aPen, x, y + halfSize, x - halfSize, y);
                    g.DrawLine(aPen, x - halfSize, y, x, y - halfSize);
                    break;
                case SymbolTypeEnum.Circle:
                    g.DrawEllipse(aPen,x-halfSize,y-halfSize,size,size);
                    break;
                case SymbolTypeEnum.OpenTriangle:
                    g.DrawLine(aPen, x, y - halfSize, x + halfSize, y + halfSize);
                    g.DrawLine(aPen, x + halfSize, y + halfSize, x - halfSize, y + halfSize);
                    g.DrawLine(aPen, x - halfSize, y + halfSize, x, y - halfSize);
                    break;
                case SymbolTypeEnum.Cross:
                    g.DrawLine(aPen, x - halfSize, y - halfSize, x + halfSize, y + halfSize);
                    g.DrawLine(aPen, x + halfSize, y - halfSize, x - halfSize, y + halfSize);
                    break;
                case SymbolTypeEnum.Star:
                    g.DrawLine(aPen, x, y - halfSize, x, y + halfSize);
                    g.DrawLine(aPen, x - halfSize, y, x + halfSize, y);
                    g.DrawLine(aPen, x - halfSize, y - halfSize, x + halfSize, y + halfSize);
                    g.DrawLine(aPen, x + halfSize, y - halfSize, x - halfSize, y + halfSize);
                    break;
                case SymbolTypeEnum.OpenInvertedTriangle:
                    g.DrawLine(aPen, x - halfSize, y - halfSize, x + halfSize, y - halfSize);
                    g.DrawLine(aPen, x + halfSize, y - halfSize, x, y + halfSize);
                    g.DrawLine(aPen, x, y + halfSize, x - halfSize, y - halfSize);
                    break;
                case SymbolTypeEnum.Plus:
                    g.DrawLine(aPen, x, y - halfSize, x, y + halfSize);
                    g.DrawLine(aPen, x - halfSize, y, x + halfSize, y);
                    break;
                case SymbolTypeEnum.Dot:
                    g.FillEllipse(aBrush, aRectangle);
                    g.DrawEllipse(aPen, aRectangle);
                    break;
                case SymbolTypeEnum.Box:
                    g.FillRectangle(aBrush, aRectangle);
                    g.DrawLine(aPen, x - halfSize, y - halfSize, x + halfSize, y - halfSize);
                    g.DrawLine(aPen, x + halfSize, y - halfSize, x + halfSize, y + halfSize);
                    g.DrawLine(aPen, x + halfSize, y + halfSize, x - halfSize, y + halfSize);
                    g.DrawLine(aPen, x - halfSize, y + halfSize, x - halfSize, y - halfSize);
                    break;
                case SymbolTypeEnum.Diamond:
                    PointF[] pta = new PointF[4];
                    pta[0].X = x;
                    pta[0].Y = y - halfSize;
                    pta[1].X = x + halfSize;
                    pta[1].Y = y;
                    pta[2].X = x;
                    pta[2].Y = y + halfSize;
                    pta[3].X = x - halfSize;
                    pta[3].Y = y;                    
                    g.FillPolygon(aBrush, pta);
                    g.DrawPolygon(aPen, pta);
                    break;
                case SymbolTypeEnum.InvertedTriangle:
                    PointF[] ptb = new PointF[3];
                    ptb[0].X = x-halfSize;
                    ptb[0].Y = y - halfSize;
                    ptb[1].X = x + halfSize;
                    ptb[1].Y = y - halfSize;
                    ptb[2].X = x;
                    ptb[2].Y = y + halfSize;
                    g.FillPolygon(aBrush, ptb);
                    g.DrawPolygon(aPen, ptb);
                    break;
                case SymbolTypeEnum.Triangle:
                    PointF[] ptc = new PointF[3];
                    ptc[0].X = x;
                    ptc[0].Y = y - halfSize;
                    ptc[1].X = x + halfSize;
                    ptc[1].Y = y + halfSize;
                    ptc[2].X = x - halfSize;
                    ptc[2].Y = y + halfSize;
                    g.FillPolygon(aBrush, ptc);
                    g.DrawPolygon(aPen, ptc);
                    break;
            }
        }

        public SymbolStyle Clone()
        {
            SymbolStyle newSS = new SymbolStyle();
            newSS.BorderThickness = this.BorderThickness;
            newSS.SymbolColor = this.SymbolColor;
            newSS.SymbolSize = this.SymbolSize;
            newSS.SymbolType = this.SymbolType;
            return newSS;
        }
    }
}
