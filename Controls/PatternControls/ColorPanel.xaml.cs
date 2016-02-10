using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lighting.Library;

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
        Brush[] brushArray;
        Point[] rangeArray;
        bool colorFromPattern = true;       // управление режимами hsb_ValuesChanged
        bool suspendUpdate = false;

        #endregion

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
            HSBcolor hsb = HSBcolor.RgbToHsb(panel.SelectedPoint.PointColor);
            int ix = panel.colorSelector.Children.Count - 2 - (int)(hsb.Hue / 30);
            panel.pointColorRange = (Border)panel.colorSelector.Children[ix];
            panel.hue.IsEnabled = true;
            panel.sat.IsEnabled = true;
            panel.bri.IsEnabled = true;
            panel.colorFromPattern = true;
            panel.SetColorRange(panel.pointColorRange);
            panel.colorFromPattern = false;
        }
        #endregion

        public ColorPanel()
        {
            InitializeComponent();
            brushArray = new Brush[]
            {
                new SolidColorBrush(Color.FromRgb(255, 0, 128)),    /* Red */
                new SolidColorBrush(Color.FromRgb(255, 0, 255)),    /* Red */
                new SolidColorBrush(Color.FromRgb(128, 0, 255)),    /* Red */
                new SolidColorBrush(Color.FromRgb(0, 0, 255)),      /* Blue*/
                new SolidColorBrush(Color.FromRgb(0, 128, 255)),    /* Red */
                new SolidColorBrush(Color.FromRgb(0, 255, 255)),    /* Red */
                new SolidColorBrush(Color.FromRgb(0, 255, 128)),    /* Red */
                new SolidColorBrush(Color.FromRgb(0, 255, 0)),      /* Green */
                new SolidColorBrush(Color.FromRgb(128, 255, 0)),    /* Red */        
                new SolidColorBrush(Color.FromRgb(255, 255, 0)),    /* Yellow */
                new SolidColorBrush(Color.FromRgb(255, 128, 0)),    /* Red */
                new SolidColorBrush(Color.FromRgb(255, 0, 0)),      /* Red */
                new SolidColorBrush(Color.FromRgb(0, 0, 0)),        /* Black */
            };

            rangeArray = new Point[]
            {
                new Point(330, 360),
                new Point(300, 330),
                new Point(270, 300),
                new Point(240, 270),
                new Point(210, 240),
                new Point(180, 210),
                new Point(150, 180),
                new Point(120, 150),
                new Point(90, 120),
                new Point(60, 90),
                new Point(30, 60),
                new Point(0, 30),
                new Point(0, 0),
            };

            Border border;
            for (int i = 0; i < 13; i++)
            {
                border = new Border();
                border.Margin = new Thickness(3);
                border.BorderBrush = new SolidColorBrush(Colors.Black);
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
        
        private void SetColorRange(Border colorRange)
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
                hue.Minimum = ((Point)selectedColorRange.Tag).X;
                hue.Maximum = ((Point)selectedColorRange.Tag).Y;
                suspendUpdate = false;
                SetValues();
            //}
        }

        private void SetValues()
        {
            suspendUpdate = true;       // для единовременного обновления
            if (colorFromPattern)
            {
                
                hue.Value = SelectedPoint.HSB.Hue;
                sat.Value = SelectedPoint.HSB.Saturation * 100;
                bri.Value = SelectedPoint.HSB.Brightness * 100;
            }
            else
            {
                suspendUpdate = true;
                hue.Value = hue.Minimum + 15;
                sat.Value = 100;
                bri.Value = 100;
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
                    SetColorRange(newColorRange);
                }
            }
        }

        private void hsb_ValuesChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HSBcolor hsb;
            Color rgb;
            if (!suspendUpdate)
            {
                hsb = new HSBcolor();
                hsb.Hue = (int)hue.Value;
                hsb.Saturation = (int)sat.Value / 100.0d;
                hsb.Brightness = (int)bri.Value / 100.0d;
                rgb = hsb.HsbToRgb();
                if (!colorFromPattern)
                    SelectedPoint.PointColor = rgb;
                rValue.Text = rgb.R.ToString();
                gValue.Text = rgb.G.ToString();
                bValue.Text = rgb.B.ToString();
            }
        }
    }
}
