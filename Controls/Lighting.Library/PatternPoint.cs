using System;
//using System.Windows.Media;
using System.Drawing;

namespace Lighting.Library
{
    public class PatternPoint : LightPoint
    {
        //public PatternPoint(Color color)
        //{
        //    _pointColor = color;
        //}

        public PatternPoint(Color color, int ledPosition) : base(color)
        {
            _ledPos = ledPosition;
        }

        private int _ledPos;
        public virtual int LedPos
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

        double _pointBrightness;
        public double PointBrightness
        {
            get { return _pointBrightness; }
            set { if (_pointBrightness != value) { _pointBrightness = value; OnPropertyChanged("PointBrightness"); } }
        }

        //private Color _pointColor = Color.Black;
        public override Color PointColor
        {
            get
            {
                return _pointColor;
            }

            set
            {
                HSBcolor hsb;
                if ( _pointColor != value)
                {
                    _pointColor = value;
                    _hsb = null;
                    PointBrightness = Brightness;
                    PointBrush.Color = value.MediaColor();
                    if (value != Color.Black)
                    {
                        hsb = new HSBcolor(Hue, Saturation, Brightness);
                        hsb.Brightness = 1.0;
                        PureBrush.Color = hsb.MediaColor;
                    }
                    else
                        PureBrush.Color = value.MediaColor();
                    OnPropertyChanged("PointColor");
                    //OnPropertyChanged("PointBrush");
                    //OnPropertyChanged("PureBrush");
                }
            }
        }

        System.Windows.Media.SolidColorBrush _brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black); 
        public System.Windows.Media.SolidColorBrush PointBrush
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

        System.Windows.Media.SolidColorBrush _pureBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
        public System.Windows.Media.SolidColorBrush PureBrush
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

        

        //public void PropertyChanged(object onPointPropertuesChanged)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
