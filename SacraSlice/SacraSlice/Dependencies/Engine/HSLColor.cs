using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine
{
    public struct HSLColor
    {
        /// <summary>
        /// Hue: 0 - 360
        /// </summary>
        public float H;

        /// <summary>
        /// Saturation: Percentage 0 - 1
        /// </summary>
        public float S;

        /// <summary>
        /// Luminance: Percentage 0 - 1
        /// </summary>
        public float L;

        public HSLColor(float h, float s, float l)
        {
            H = h;
            S = s;
            L = l;
        }

        public Color ToRgbColor()
        {
            var color = new Color();
            color.A = 255;

            var c = ((1 - Math.Abs(2 * L - 1)) * S);

            var x = (c * (1 - Math.Abs((H / 60f) % 2 - 1)));

            var m = L - c / 2;

            float r = 0; float g = 0; float b = 0;

            if (H >= 0 && H < 60)
            {
                r = c;
                g = x;
                b = 0;

            }
            else if (H >= 60 && H < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (H >= 120 && H < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (H >= 180 && H < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (H >= 240 && H < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else if (H >= 300 && H < 360)
            {
                r = c;
                g = 0;
                b = x;
            }

            color.R = (byte)((r + m) * 255f);
            color.G = (byte)((g + m) * 255f);
            color.B = (byte)((b + m) * 255f);

            return color;

        }

        public override string ToString()
        {
            return H + " " + S + " " + L;
        }

    }
}
