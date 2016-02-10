using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Lighting.Library
{
    public class HSBcolor
    {

        public HSBcolor() { }

        public HSBcolor(double hue, double sat, double bri) : this()
        {
            Hue = hue;
            Saturation = sat;
            Brightness = bri;
        }

        double _hue;
        public double Hue
        {
            get
            {
                return _hue;
            }

            set
            {
                if (value > 360.0)
                {
                    _hue = value - 360;
                    return;
                }
                if (value < 0)
                {
                    _hue = 360 + value;
                    return;
                }
                _hue = value;
            }

        }

        public double Saturation { get; set; }

        double _brightness;
        public double Brightness
        {
            get { return _brightness; }
            set
            {
                if (value > 1)
                {
                    _brightness = 1;
                    return;
                }
                if (value < 0)
                {
                    _brightness = 0;
                    return;
                }
                _brightness = value;
            }
        }

        public Color HsbToRgb()
        {
            double r = 0;
            double g = 0;
            double b = 0;

            if (Saturation == 0)
            {
                r = g = b = Brightness;
            }
            else
            {
                // the color wheel consists of 6 sectors. Figure out which sector you're in.
                double sectorPos = Hue / 60.0;
                int sectorNumber = (int)(Math.Floor(sectorPos));
                // get the fractional part of the sector
                double fractionalSector = sectorPos - sectorNumber;

                // calculate values for the three axes of the color. 
                double p = Brightness * (1.0 - Saturation);
                double q = Brightness * (1.0 - (Saturation * fractionalSector));
                double t = Brightness * (1.0 - (Saturation * (1 - fractionalSector)));

                // assign the fractional colors to r, g, and b based on the sector the angle is in.
                switch (sectorNumber)
                {
                    case 0:
                        r = Brightness;
                        g = t;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = Brightness;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = Brightness;
                        b = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = Brightness;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        b = Brightness;
                        break;
                    case 5:
                        r = Brightness;
                        g = p;
                        b = q;
                        break;
                }
            }


            return System.Windows.Media.Color.FromRgb(
                Convert.ToByte((r * 255.0)),
                Convert.ToByte((g * 255.0)),
                Convert.ToByte((b * 255.0))
                );


        }

        public static HSBcolor RgbToHsb(Color color)
        {
            HSBcolor hsb = new HSBcolor();

            double r = ((double)color.R / 255.0);
            double g = ((double)color.G / 255.0);
            double b = ((double)color.B / 255.0);

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            hsb.Hue = 0.0;
            if (max == r && g >= b)
            {
                if (max - min == 0) hsb.Hue = 0.0;
                else hsb.Hue = 60 * (g - b) / (max - min);
            }
            else if (max == r && g < b)
            {
                hsb.Hue = 60 * (g - b) / (max - min) + 360;
            }
            else if (max == g)
            {
                hsb.Hue = 60 * (b - r) / (max - min) + 120;
            }
            else if (max == b)
            {
                hsb.Hue = 60 * (r - g) / (max - min) + 240;
            }

            hsb.Saturation = (max == 0) ? 0.0 : (1.0 - ((double)min / (double)max));

            hsb.Brightness = (double)max;

            return hsb;
        }
    }

}
