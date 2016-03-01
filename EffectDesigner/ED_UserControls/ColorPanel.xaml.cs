using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Media = System.Windows.Media;
using Lighting.Library;
using System.ComponentModel;
using System;
using ED_CustomControls;

namespace ED_UserControls
{
    /// <summary>
    /// Логика взаимодействия для ColorPanel.xaml
    /// </summary>
    public partial class ColorPanel : UserControl
    {

        #region Private fields

        Border selectedColorRange = null;   // В свойстве Tag находится Point с диапазоном Hue
        Border newColorRange = null;
        Point hueRange;
        Media.Brush[] colorRanges;
        Point[] rangePointArray;
        bool pointFromPattern = true;       // управление режимами 
        bool suspendUpdate = false;

        #endregion

        #region SelectedPoint DP
        public PatternPoint SelectedPoint
        {
            get { return (PatternPoint)GetValue(SelectedPointProperty); }
            set { SetValue(SelectedPointProperty, value); }
        }

        public static readonly DependencyProperty SelectedPointProperty =
                                DependencyProperty.Register("SelectedPoint", typeof(PatternPoint), typeof(ColorPanel),
                                    new FrameworkPropertyMetadata(null, OnSelectedPointChanged));

        private static void OnSelectedPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPanel panel = d as ColorPanel;
            PatternPoint pp = (PatternPoint)e.OldValue;
            panel.pointFromPattern = true;

            if (panel.SelectedPoint != null)
            {
                if (panel.SelectedPoint.Variant == PointVariant.Lightness)
                {
                    panel.hue.IsEnabled = false;
                    panel.sat.IsEnabled = false;
                    panel.bri.IsEnabled = true;
                }
                else
                {
                    panel.hue.IsEnabled = true;
                    panel.sat.IsEnabled = true;
                    panel.bri.IsEnabled = true;
                }

                if (pp != null)
                    (pp as INotifyPropertyChanged).PropertyChanged -= panel.OnColorChanged;
                (panel.SelectedPoint as INotifyPropertyChanged).PropertyChanged += panel.OnColorChanged;
                panel.SetColorMetrics();
                panel.SetColorRange();
                panel.realColor.Background = panel.SelectedPoint.PointBrush;
            }
        }

        private void OnColorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PointColor" || e.PropertyName == "PointLightness")
            {
                SetColorMetrics();
                realColor.Background = SelectedPoint.PointBrush;
                if (e.PropertyName == "PointLightness")
                    SetSliders();
            }
        }

        private void SetColorMetrics()
        {
            briValue.Text = Math.Round(SelectedPoint.HslColor.Lightness * 100, 0).ToString();
            satValue.Text = Math.Round(SelectedPoint.HslColor.Saturation * 100, 0).ToString();
            hueValue.Text = Math.Round(SelectedPoint.HslColor.Hue, 0).ToString();
            rValue.Text = SelectedPoint.PointColor.R.ToString();
            gValue.Text = SelectedPoint.PointColor.G.ToString();
            bValue.Text = SelectedPoint.PointColor.B.ToString();
        }

        private void SetSliders()
        {
            Point p = (Point)selectedColorRange.Tag;
            if (!hue.IsActive && !hue.IsActive && !bri.IsActive)
            {
                suspendUpdate = true;
                hue.Minimum = p.X;
                hue.Maximum = p.Y;

                if (pointFromPattern)
                {
                    hue.Value = Math.Round(SelectedPoint.HslColor.Hue, 0);
                    sat.Value = Math.Round(SelectedPoint.HslColor.Saturation * 100, 0);
                    bri.Value = Math.Round(SelectedPoint.HslColor.Lightness * 100, 0);
                }
                else
                {
                    hue.Value = Math.Round(p.X + (p.Y - p.X) / 2, 0);
                    sat.Value = 100;
                    bri.Value = Math.Round(SelectedPoint.HslColor.Lightness * 100, 0);
                }

                hue.ScaleRange = hueRange;
                sat.HueValue = hue.Value;

                suspendUpdate = false;
                hsb_ValuesChanged(null, null);
            }
        }


        #endregion

        /************************************************************************/

        #region Constructor
        public ColorPanel()
        {
            InitializeComponent();
            colorRanges = new System.Windows.Media.Brush[]
            {
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 0, 128)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 0, 255)),    /* Magenta */
                new Media.SolidColorBrush(Media.Color.FromRgb(128, 0, 255)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 0, 255)),      /* Blue*/
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 128, 255)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 255, 255)),    /* Cyan */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 255, 0)),      /* Green */
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 255, 0)),    /* Yellow */
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 128, 0)),    /* Orange */
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 0, 0)),      /* Red */
            };

            rangePointArray = new System.Windows.Point[]
            {
                new System.Windows.Point(330, 360),
                new System.Windows.Point(300, 330),
                new System.Windows.Point(270, 300),
                new System.Windows.Point(240, 270),
                new System.Windows.Point(210, 240),
                new System.Windows.Point(180, 210),
                new System.Windows.Point(90, 180),
                //new System.Windows.Point(60, 150),
                //new System.Windows.Point(90, 120),
                new System.Windows.Point(60, 90),
                new System.Windows.Point(30, 60),
                new System.Windows.Point(0, 30),
                //new System.Windows.Point(0, 0),
            };

            Border border;
            for (int i = 0; i < 10; i++)
            {
                border = new Border();
                border.Margin = new Thickness(3);
                border.BorderBrush = new Media.SolidColorBrush(Media.Colors.Black);
                border.BorderThickness = new Thickness(0);
                border.Background = colorRanges[i];
                border.Tag = rangePointArray[i];
                border.MouseLeftButtonUp += new MouseButtonEventHandler(BorderMouseLeftUp);
                colorSelector.Children.Add(border);
            }
        }

        #endregion


        private void BorderMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            newColorRange = sender as Border;

            if (SelectedPoint != null)
            {
                if (SelectedPoint.Variant != PointVariant.Lightness)
                {
                    if (newColorRange != null)
                    {
                        pointFromPattern = false;
                        //SetColorRange(colorSelector, newColorRange);
                        SetColorRange();
                    }
                }
            }
        }

        private void SetColorRange()
        {
            int i = 0;

            if (selectedColorRange != null)
            {
                selectedColorRange.Margin = new Thickness(3);
                selectedColorRange.BorderThickness = new Thickness(0);
            }

            if (pointFromPattern)
            {
                Point p;
                HslColor hsl = SelectedPoint.HslColor;
                foreach (Border range in colorSelector.Children)
                {
                    p = (Point)range.Tag;
                    if (hsl.Hue >= p.X && hsl.Hue <= p.Y)
                        break;
                    i++;
                }

                newColorRange = colorSelector.Children[i] as Border;
            }

            selectedColorRange = newColorRange;
            hueRange = (Point)selectedColorRange.Tag;
            selectedColorRange.Margin = new Thickness(0);
            selectedColorRange.BorderThickness = new Thickness(1);

            SetSliders();

        }

        private void hsb_ValuesChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HslSlider s = sender as HslSlider;
            if ( sender == null)
            {
                if (!suspendUpdate)
                {
                    if (!pointFromPattern)
                        SelectedPoint.HslColor = new HslColor(hue.Value, sat.Value / 100.0, bri.Value / 100.0);
                }
                return;
            }
            if ( s.IsActive )
            {

                if (s == hue)
                    sat.HueValue = hue.Value;
                SelectedPoint.HslColor = new HslColor(hue.Value, sat.Value / 100.0, bri.Value / 100.0);
            }
        }
    }
}
