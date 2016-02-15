using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
//using System.Windows.Media;
using Lighting.Library;
using System.ComponentModel;
using System.Drawing;
using Media = System.Windows.Media;

namespace PatternControls
{
    /// <summary>
    /// Логика взаимодействия для ColorPanel.xaml
    /// </summary>
    public partial class ColorPanel : UserControl
    {

        #region Private fields

        Border selectedColorRange = null;   // В свойстве Tag находится Point с диапазоном Hue
        Border pointColorRange = null;
        //int selectedColorIx = -1;
        System.Windows.Media.Brush[] brushArray;
        System.Windows.Point[] rangeArray;
        bool colorFromPattern = true;       // управление режимами hsb_ValuesChanged
        bool suspendUpdate = false;

        #endregion

        /************************************************************************/

        #region DP Properties

        /************************************************************************/

        #region SelectedPoint DP
        public PatternPoint SelectedPoint
        {
            get { return (PatternPoint)GetValue(SelectedPointProperty); }
            set { SetValue(SelectedPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPointProperty =
                                DependencyProperty.Register("SelectedPoint", typeof(PatternPoint), typeof(ColorPanel),
                                    new FrameworkPropertyMetadata(null, OnSelectedPointChanged));

        private static void OnSelectedPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPanel panel = d as ColorPanel;
            PatternPoint pp = (PatternPoint)e.OldValue;
            //HSBcolor hsb = HSBcolor.RgbToHsb(panel.SelectedPoint.PointColor);
            HSBcolor hsb = panel.SelectedPoint.HSB;
            int ix = panel.colorSelector.Children.Count - 2 - (int)(hsb.Hue / 30);
            panel.pointColorRange = (Border)panel.colorSelector.Children[ix];
            panel.hue.IsEnabled = true;
            panel.sat.IsEnabled = true;
            panel.bri.IsEnabled = true;
            panel.colorFromPattern = true;
            panel.SetColorRange(panel.SelectedPoint, panel.pointColorRange);
            panel.colorFromPattern = false;
            if (pp != null)
                (pp as INotifyPropertyChanged).PropertyChanged -= panel.OnPointBrightnessChanged;
            (panel.SelectedPoint as INotifyPropertyChanged).PropertyChanged += panel.OnPointBrightnessChanged;

        }

        private void OnPointBrightnessChanged(object sender, PropertyChangedEventArgs e)
        {
            int x = 0;
            if (e.PropertyName == "PointBrightness")
                x = 5;
        }

        #endregion

        /************************************************************************/

        #endregion

        public ColorPanel()
        {
            InitializeComponent();
            brushArray = new System.Windows.Media.Brush[]
            {
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 0, 128)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 0, 255)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(128, 0, 255)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 0, 255)),      /* Blue*/
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 128, 255)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 255, 255)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 255, 128)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 255, 0)),      /* Green */
                new Media.SolidColorBrush(Media.Color.FromRgb(128, 255, 0)),    /* Red */        
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 255, 0)),    /* Yellow */
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 128, 0)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 0, 0)),      /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 0, 0)),        /* Black */
            };

            rangeArray = new System.Windows.Point[]
            {
                new System.Windows.Point(330, 360),
                new System.Windows.Point(300, 330),
                new System.Windows.Point(270, 300),
                new System.Windows.Point(240, 270),
                new System.Windows.Point(210, 240),
                new System.Windows.Point(180, 210),
                new System.Windows.Point(150, 180),
                new System.Windows.Point(120, 150),
                new System.Windows.Point(90, 120),
                new System.Windows.Point(60, 90),
                new System.Windows.Point(30, 60),
                new System.Windows.Point(0, 30),
                new System.Windows.Point(0, 0),
            };

            Border border;
            for (int i = 0; i < 13; i++)
            {
                border = new Border();
                border.Margin = new Thickness(3);
                border.BorderBrush = new Media.SolidColorBrush(Media.Colors.Black);
                border.BorderThickness = new Thickness(0);
                border.Background = brushArray[i];
                border.Tag = rangeArray[i];
                border.MouseLeftButtonUp += new MouseButtonEventHandler(BorderMouseLeftUp);
                colorSelector.Children.Add(border);
            }

            // устанавливаем выбор на черном цвете

            selectedColorRange = colorSelector.Children[brushArray.Length - 1] as Border;
            selectedColorRange.Margin = new Thickness(0);
            selectedColorRange.BorderThickness = new Thickness(1);
        }
        
        private void SetColorRange(object source, Border colorRange)
        {
            //if (selectedColorRange != colorRange)
            //{
                if (selectedColorRange != null)
                {
                    selectedColorRange.Margin = new Thickness(3);
                    selectedColorRange.BorderThickness = new Thickness(0);
                }
                colorRange.Margin = new Thickness(0);
                colorRange.BorderThickness = new Thickness(1);
                selectedColorRange = colorRange;
                suspendUpdate = true;
                hue.Minimum = ((System.Windows.Point)selectedColorRange.Tag).X;
                hue.Maximum = ((System.Windows.Point)selectedColorRange.Tag).Y;
                suspendUpdate = false;
                SetValues(source);
            //}
        }
        
        private void SetValues(object source)
        {
            suspendUpdate = true;       // для единовременного обновления
            if (colorFromPattern)
            {
                
                hue.Value = SelectedPoint.Hue;
                sat.Value = SelectedPoint.Saturation * 100;
                bri.Value = SelectedPoint.Brightness * 100;
            }
            else
            {
                suspendUpdate = true;
                hue.Value = hue.Minimum + 15;
                sat.Value = 100;
                bri.Value = SelectedPoint.Brightness * 100;
                //bri.Value = 100;
            }
            suspendUpdate = false;
            hsb_ValuesChanged(null, null);
        }

        private void BorderMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            Border newColorRange = sender as Border;
            if (SelectedPoint != null)
            {
                if (newColorRange != null)
                {
                    colorFromPattern = false;
                    SetColorRange(colorSelector, newColorRange);
                }
            }
        }

        bool isMouseCaptured = false;
        // изменение значения любого слайдера
        private void hsb_ValuesChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HSBcolor hsb;
            //Color rgb;
            //Slider s = sender as Slider;
            //isMouseCaptured = s.IsMouseCaptured;
            if (!suspendUpdate)
            {
                hsb = new HSBcolor();
                hsb.Hue = (int)hue.Value;
                hsb.Saturation = (int)sat.Value / 100.0d;
                hsb.Brightness = (int)bri.Value / 100.0d;
                //rgb = hsb.HsbToRgb();
                //hsb.RgbColor
                if (!colorFromPattern)
                    SelectedPoint.PointColor = hsb.RgbColor;
                rValue.Text = hsb.RgbColor.R.ToString();
                gValue.Text = hsb.RgbColor.G.ToString();
                bValue.Text = hsb.RgbColor.B.ToString();
            }
        }

        private void hue_GotMouseCapture(object sender, MouseEventArgs e)
        {
            isMouseCaptured = true;
            colorFromPattern = false;
            //var x = (sender as Slider).IsMouseCaptured;
        }

        private void bri_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //var x = 5;
        }
    }
}
