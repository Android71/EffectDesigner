using System.ComponentModel;
using System.Windows.Media;

namespace Lighting.Library
{
    // Универсальный тип для использования в Universe, Strip и Pattern
    public class LightPoint : INotifyPropertyChanged
    {

        public LightPoint(Color color)
        {
            PointColor = color;
            //HsbColor = HSBcolor.FromMediaColor(color);
        }

        

        private Color _pointColor = Colors.Black;
        public virtual Color PointColor
        {
            get
            {
                if (PointBrush != null)
                    return PointBrush.Color;
                else
                    return _pointColor;
            }

            set
            {
                if (PointBrush != null)
                    PointBrush.Color = value;
                else
                {
                    PointBrush = new SolidColorBrush(value);
                }
                OnPropertyChanged("PointColor");
            }
        }

        SolidColorBrush _brush;
        public SolidColorBrush PointBrush
        {
            get { return _brush; }
            set
            {
                if (_brush != value)
                {
                    _brush = value;
                    OnPropertyChanged("PointBrush");
                }
            }
        }

        HSBcolor hsb;
        public virtual HSBcolor HSB
        {
            get { hsb = HSBcolor.RgbToHsb(PointColor); return hsb; }
            private set { }
        }

        public double DeltaHue { get; set; }

        public double DeltaSaturetion { get; set; }

        public double DeltaBrightness { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
}
