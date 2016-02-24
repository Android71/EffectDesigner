using System;
//using System.Windows.Media;
using Media = System.Windows.Media;
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
            Variant = 0;
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

        // 0 - GradientPoint
        // 1 - RangePoint
        // 2 - LightnessPoint
        public int Variant { get; set; }

        double _pointLightness;
        public double PointLightness
        {
            get { return Math.Round( _pointLightness, 2 ); }
            set { if (_pointLightness != value) { _pointLightness = value; OnPropertyChanged("PointLightness"); } }
        }

        protected override void OnColorChanged()
        {
            double lightness = HslColor.Lightness;
            base.OnColorChanged();
            PointLightness = HslColor.Lightness;
            PointBrush.Color = PointColor.MediaColor();
            if (HslColor.Lightness < 0.2)
                lightness = 0.2;
            HslColor hsl = new HslColor(HslColor.Hue, HslColor.Saturation, lightness);
            PureBrush.Color = hsl.ToRGB().MediaColor();
        }

        Media.SolidColorBrush _brush = new Media.SolidColorBrush(Media.Color.FromRgb(0,0,0));
        public Media.SolidColorBrush PointBrush
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

        Media.SolidColorBrush _pureBrush = new Media.SolidColorBrush(Media.Color.FromRgb(0, 0, 0));
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
    }
}
