using System;
using System.ComponentModel;
//using System.Windows.Media;
using System.Drawing;

namespace Lighting.Library
{
    // Универсальный тип для использования в Universe, Strip и Pattern
    public class LightPoint : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public LightPoint(Color color)
        {
            PointColor = color;
            //HsbColor = HSBcolor.FromMediaColor(color);
        }

        protected Color _pointColor = Color.Black;
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
                    _hsb = null;
                    OnPropertyChanged("PointColor");
                }
            }
        }

        protected HSBcolor _hsb = null;
        public HSBcolor HSB
        {
            get
            {
                if (_hsb == null)
                {
                    _hsb = new HSBcolor();

                    double r = ((double)_pointColor.R / 255.0);
                    double g = ((double)_pointColor.G / 255.0);
                    double b = ((double)_pointColor.B / 255.0);

                    double max = Math.Max(r, Math.Max(g, b));
                    double min = Math.Min(r, Math.Min(g, b));

                    _hsb.Hue = 0.0;
                    if (max == r && g >= b)
                    {
                        if (max - min == 0) _hsb.Hue = 0.0;
                        else _hsb.Hue = 60 * (g - b) / (max - min);
                    }
                    else if (max == r && g < b)
                    {
                        _hsb.Hue = 60 * (g - b) / (max - min) + 360;
                    }
                    else if (max == g)
                    {
                        _hsb.Hue = 60 * (b - r) / (max - min) + 120;
                    }
                    else if (max == b)
                    {
                        _hsb.Hue = 60 * (r - g) / (max - min) + 240;
                    }

                    _hsb.Saturation = (max == 0) ? 0.0 : (1.0 - ((double)min / (double)max));

                    _hsb.Brightness = (double)max;
                }
                return _hsb;
            } 
        }

        public double Hue
        {
            get { return HSB.Hue; }
        }
        public double Saturation
        {
            get { return HSB.Saturation; }
        }
        public double Brightness
        {
            get { return HSB.Brightness; }
        }

    }

    
}
