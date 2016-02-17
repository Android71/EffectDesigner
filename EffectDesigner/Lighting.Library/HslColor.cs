using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lighting.Library
{
    public struct HslColor
    {
            public HslColor(double h, double s, double l)
        {
            Hue = h;
            Saturation = s;
            Luminosity = l;
        }

        public double Hue { get; private set; }

        public double Saturation { get; private set; }

        public double Luminosity { get; private set; }
    }
}
