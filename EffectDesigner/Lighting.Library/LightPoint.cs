﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lighting.Library
{
    public class LightPoint : INotifyPropertyChanged
    {
        bool eventTriggered = false;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public LightPoint(Color color)
        {
            PointColor = color;
        }

        protected Color _pointColor;
        public virtual Color PointColor
        {
            get
            {
                return _pointColor;
            }

            set
            {
                if (_pointColor != value)
                {
                    _pointColor = value;
                    if (!eventTriggered)
                    {
                        eventTriggered = !eventTriggered;
                        HslColor = new HslColor(_pointColor.GetHue(), _pointColor.GetSaturation(), _pointColor.GetBrightness());
                        OnColorChanged();
                    }
                    else
                    {
                        eventTriggered = false;
                    }
                }
            }
        }

        protected HslColor _hsl;
        public HslColor HslColor
        {
            get { return _hsl; }
            set
            {
                if (_hsl != value)
                {
                    _hsl = value;
                    if (!eventTriggered)
                    {
                        eventTriggered = !eventTriggered;
                        PointColor = HslColor.ToRGB();
                        OnColorChanged();
                    }
                    else
                    {
                        eventTriggered = false;
                    }
                }
            }
        }

        protected void OnColorChanged()
        {
            OnPropertyChanged("PointColor");
        }
    }


    public static class ColorExtension
    {
        static double c13 = 1.0 / 3.0;
        static double c23 = 2.0 / 3.0;

        /// <summary>
        /// Converts HSL to RGB.
        /// </summary>
        /// <param name="h">Hue, must be in [0, 360].</param>
        /// <param name="s">Saturation, must be in [0..1].</param>
        /// <param name="l">Luminance, must be in [0..1].</param>
        public static Color ToRGB(this HslColor c)
        {
            double h = c.Hue;
            double s = c.Saturation;
            double l = c.Luminosity;
            if (s == 0)
            {
                // achromatic color (gray scale)
                return Color.FromArgb((int)(l * 255.0), (int)(l * 255.0), (int)(l * 255.0));
            }
            else
            {
                double q = (l < 0.5) ? (l * (1.0 + s)) : (l + s - (l * s));
                double p = (2.0 * l) - q;

                double Hk = h / 360.0;
                double[] T = new double[3];
                T[0] = Hk + (c13);    // Tr
                T[1] = Hk;                // Tb
                T[2] = Hk - (c13);    // Tg

                for (int i = 0; i < 3; i++)
                {
                    if (T[i] < 0) T[i] += 1.0;
                    if (T[i] > 1) T[i] -= 1.0;

                    if ((T[i] * 6) < 1)
                    {
                        T[i] = p + ((q - p) * 6.0 * T[i]);
                    }
                    else if ((T[i] * 2.0) < 1) //(1.0/6.0)<=T[i] && T[i]<0.5
                    {
                        T[i] = q;
                    }
                    else if ((T[i] * 3.0) < 2) // 0.5<=T[i] && T[i]<(2.0/3.0)
                    {
                        T[i] = p + (q - p) * (c23 - T[i]) * 6.0;
                    }
                    else T[i] = p;
                }

                return Color.FromArgb((int)(T[0] * 255.0), (int)(T[1] * 255.0), (int)(T[2] * 255.0));
                    
            }
        }
        //public static Color ToRGB(this HslColor color)
        //{
        //    double r = 0, g = 0, b = 0;
        //    if (color.Luminosity != 0)
        //    {
        //        if (color.Saturation == 0)
        //            r = g = b = color.Luminosity;
        //        else
        //        {
        //            double temp2;
        //            if (color.Luminosity < 0.5)
        //                temp2 = color.Luminosity * (1.0 + color.Saturation);
        //            else
        //                temp2 = color.Luminosity + color.Saturation - (color.Luminosity * color.Saturation);

        //            double temp1 = 2.0 * color.Luminosity - temp2;

        //            r = GetColorComponent(temp1, temp2, color.Hue + c13);
        //            g = GetColorComponent(temp1, temp2, color.Hue);
        //            b = GetColorComponent(temp1, temp2, color.Hue - c13);
        //        }
        //    }
        //    return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        //}

        //private static double GetColorComponent(double temp1, double temp2, double temp3)
        //{
        //    if (temp3 < 0.0)
        //        temp3 += 1.0;
        //    else if (temp3 > 1.0)
        //        temp3 -= 1.0;

        //    if (temp3 < c16)
        //        return temp1 + (temp2 - temp1) * 6.0 * temp3;
        //    else if (temp3 < 0.5)
        //        return temp2;
        //    else if (temp3 < c23)
        //        return temp1 + ((temp2 - temp1) * (c23 - temp3) * 6.0);
        //    else
        //        return temp1;
        //}



        //public static System.Windows.Media.SolidColorBrush Brush(this Color color)
        //{
        //    return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
        //}

        //public static System.Windows.Media.Color MediaColor(this Color color)
        //{
        //    return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
        //}

    }
}
