using System;
using System.Windows.Media;

namespace Lighting.Library
{
    public class PatternPoint : LightPoint
    {
        public PatternPoint(Color color, int ledPosition) : base(color)
        {
            LedPos = ledPosition;
        }

        double _pointBrightness;
        public double PointBrightness
        {
            get { return _pointBrightness; }
            set { if (_pointBrightness != value) _pointBrightness = value; OnPropertyChanged("PointBrightness"); }
        }
        public override Color PointColor
        {
            get
            {
                return base.PointColor;
            }

            set
            {
                Color rgb;
                base.PointColor = value;
                PointBrightness = HSB.Brightness;
                if (value != Colors.Black)
                {
                    HSBcolor hsb = new HSBcolor(HSB.Hue, HSB.Saturation, 1);
                    rgb = hsb.HsbToRgb();
                }
                else
                    rgb = Colors.Black;
                if (PureBrush != null)
                    PureBrush.Color = rgb;
                else
                {
                    PureBrush = new SolidColorBrush(rgb);
                }
                //OnPropertyChanged("PointColor");
            }
        }

        SolidColorBrush _pureBrush;
        public SolidColorBrush PureBrush
        {
            get { return _pureBrush; }
            set
            {
                if (_pureBrush != value)
                {
                    _pureBrush = value;
                    OnPropertyChanged("PureBrush");
                }
            }
        }

        private int _ledPos;
        public int LedPos
        {
            get { return _ledPos; }

            set { if (_ledPos != value) { _ledPos = value; OnPropertyChanged("LedPos"); } }
        }

        private int _ledCount = 0;
        public int LedCount
        {
            get { return _ledCount; }

            set { if (_ledCount != value) { _ledCount = value; OnPropertyChanged("LedCount"); } }
        }

        public void PropertyChanged(object onPointPropertuesChanged)
        {
            throw new NotImplementedException();
        }
    }
}
