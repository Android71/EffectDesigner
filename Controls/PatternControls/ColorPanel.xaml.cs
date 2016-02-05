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

        Border selectedColorRange = null;
        Brush[] brushArray;
        Point[] rangeArray;
        bool colorFromPattern = true;

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
            int ix = 0;
            PatternPoint pp = (PatternPoint)e.NewValue;
            HSBcolor hsb = HSBcolor.RgbToHsb(pp.PointColor);
            if ((hsb.Hue >= 0) && (hsb.Hue < 30)) { ix = 11; goto M1; }
            if ((hsb.Hue >= 30) && (hsb.Hue < 60)) { ix = 10; goto M1; }
            if ((hsb.Hue >= 60) && (hsb.Hue < 90)) { ix = 9; goto M1; }
            if ((hsb.Hue >= 90) && (hsb.Hue < 120)) { ix = 8; goto M1; }
            if ((hsb.Hue >= 120) && (hsb.Hue < 150)) { ix = 7; goto M1; }
            if ((hsb.Hue >= 150) && (hsb.Hue < 180)) { ix = 6; goto M1; }
            if ((hsb.Hue >= 180) && (hsb.Hue < 210)) { ix = 5; goto M1; }
            if ((hsb.Hue >= 210) && (hsb.Hue < 240)) { ix = 4; goto M1; }
            if ((hsb.Hue >= 240) && (hsb.Hue < 270)) { ix = 3; goto M1; }
            if ((hsb.Hue >= 270) && (hsb.Hue < 300)) { ix = 2; goto M1; }
            if ((hsb.Hue >= 300) && (hsb.Hue < 330)) { ix = 1; goto M1; }
            if ((hsb.Hue >= 330) && (hsb.Hue < 360)) { ix = 0; goto M1; }
            M1:
            panel.colorFromPattern = true;
            panel.SetSelection(ix, hsb, pp.PointColor);
            panel.colorFromPattern = false;
        }

        private void SetSelection(int index, HSBcolor hsb, Color rgb)
        {
            if (selectedColorRange != colorSelector.Children[index])
            {
                Border border = colorSelector.Children[index] as Border;
                if (selectedColorRange != null)
                {
                    selectedColorRange.Margin = new Thickness(3);
                    selectedColorRange.BorderThickness = new Thickness(0);
                }
                border.Margin = new Thickness(0);
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                border.BorderThickness = new Thickness(1);
                selectedColorRange = border;
                hue.Minimum = ((Point)selectedColorRange.Tag).X;
                hue.Maximum = ((Point)selectedColorRange.Tag).Y - 0.1d;
                hue.IsEnabled = true;
                sat.IsEnabled = true;
                bri.IsEnabled = true;
                hue.Value = hsb.Hue;
                sat.Value = hsb.Saturation * 100;
                bri.Value = hsb.Brightness * 100;
                rValue.Text = rgb.R.ToString();
                gValue.Text = rgb.G.ToString();
                bValue.Text = rgb.B.ToString();
            }
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
                new SolidColorBrush(Color.FromRgb(0, 0, 0)),
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
                border.Background = brushArray[i];
                border.Tag = rangeArray[i];
                border.MouseLeftButtonUp += new MouseButtonEventHandler(BorderMouseLeftUp);
                colorSelector.Children.Add(border);
            }

            selectedColorRange = colorSelector.Children[brushArray.Length - 1] as Border;
            selectedColorRange.Margin = new Thickness(0);
            selectedColorRange.BorderBrush = new SolidColorBrush(Colors.Black);
            selectedColorRange.BorderThickness = new Thickness(1);
            hue.IsEnabled = false;
            sat.IsEnabled = false;
            bri.IsEnabled = false;
        }

        private void BorderMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                if (selectedColorRange != null)
                {
                    selectedColorRange.Margin = new Thickness(3);
                    selectedColorRange.BorderThickness = new Thickness(0);
                }
                border.Margin = new Thickness(0);
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                border.BorderThickness = new Thickness(1);
                selectedColorRange = border;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            hue.GotFocus += Hue_GotFocus;
        }

        private void Hue_GotFocus(object sender, RoutedEventArgs e)
        {
            var x = sender;
        }

        private void hue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HSBcolor hsb;
            Color rgb;
            if (!colorFromPattern)
            {
                hsb = new HSBcolor();
                hsb.Hue = (int)hue.Value;
                hsb.Saturation = (int)sat.Value / 100.0d;
                hsb.Brightness = (int)bri.Value / 100.0d;
                rgb = hsb.HsbToRgb();
                SelectedPoint.PointColor = rgb;
                rValue.Text = rgb.R.ToString();
                gValue.Text = rgb.G.ToString();
                bValue.Text = rgb.B.ToString();
            }
            //HSBcolor hsb = new HSBcolor(hue.Value, sat.Value, bri.Value);
            
        }
    }
}
