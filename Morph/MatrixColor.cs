using System;
using System.Drawing;

namespace ch.appl.psoft.Morph
{
    public class MatrixColor
    {
        // Farbe für Matrix
        public int id;
        public string title;
        public Color farbe;

        public MatrixColor(int id, string title, int argb)
        {
            this.id = id;
            this.title = title;
            this.farbe = Color.FromArgb(argb);
        }

        public string toCSS()
        {
            string rhex = Convert.ToString(this.farbe.R, 16).PadLeft(2, '0');
            string ghex = Convert.ToString(this.farbe.G, 16).PadLeft(2, '0');
            string bhex = Convert.ToString(this.farbe.B, 16).PadLeft(2, '0');

            return "#" + rhex + ghex + bhex;
        }
    }
}
