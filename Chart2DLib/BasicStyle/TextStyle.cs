using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Readearth.Chart2D.BasicStyle
{
    public class TextStyle
    {
        //private Chart2D chart2d;
        private Color textColor = Color.Black;
        private Font textfont = new Font("Microsoft YaHei", 10, FontStyle.Regular);
        private float textSize  = 10;

        public TextStyle()
        { }

        public TextStyle(Font font, Color color)
        {
            TextFont = font;
            TextColor = color;
            textSize = TextFont.Size;
        }

        public float TextSize
        {
            get { return textSize; }
            set { textSize = value; textfont = new Font(textfont.Name, value, textfont.Style, textfont.Unit); }
        }
        public Font TextFont
        {
            get { return textfont; }
            set { textfont = value; textSize = textfont.Size; }
        }
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public TextStyle Clone()
        {
            TextStyle newTS = new TextStyle(this.TextFont, this.TextColor);
            return newTS;
        }
    }
}
