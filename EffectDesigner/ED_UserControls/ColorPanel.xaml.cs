using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Media = System.Windows.Media;

namespace ED_UserControls
{
    /// <summary>
    /// Логика взаимодействия для ColorPanel.xaml
    /// </summary>
    public partial class ColorPanel : UserControl
    {

        #region Private fields

        //Border selectedColorRange = null;   // В свойстве Tag находится Point с диапазоном Hue
        //Border pointColorRange = null;
        //int selectedColorIx = -1;
        System.Windows.Media.Brush[] brushArray;
        System.Windows.Point[] rangeArray;
        //bool colorFromPattern = true;       // управление режимами hsb_ValuesChanged
        //bool suspendUpdate = false;

        #endregion

        public ColorPanel()
        {
            InitializeComponent();
            brushArray = new System.Windows.Media.Brush[]
            {
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 0, 128)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 0, 255)),    /* Magenta */
                new Media.SolidColorBrush(Media.Color.FromRgb(128, 0, 255)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 0, 255)),      /* Blue*/
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 128, 255)),    /* Red */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 255, 255)),    /* Cyan */
                //new Media.SolidColorBrush(Media.Color.FromRgb(0, 255, 128)),    /* Green */
                new Media.SolidColorBrush(Media.Color.FromRgb(0, 255, 0)),      /* Green */
                //new Media.SolidColorBrush(Media.Color.FromRgb(128, 255, 0)),    /* Green */        
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 255, 0)),    /* Yellow */
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 128, 0)),    /* Orange */
                new Media.SolidColorBrush(Media.Color.FromRgb(255, 0, 0)),      /* Red */
                //new Media.SolidColorBrush(Media.Color.FromRgb(0, 0, 0)),        /* Black */
            };

            rangeArray = new System.Windows.Point[]
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
                border.Background = brushArray[i];
                border.Tag = rangeArray[i];
                border.MouseLeftButtonUp += new MouseButtonEventHandler(BorderMouseLeftUp);
                colorSelector.Children.Add(border);
            }
        }


        private void BorderMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            Border newColorRange = sender as Border;
            //if (SelectedPoint != null)
            //{
            //    if (newColorRange != null)
            //    {
            //        colorFromPattern = false;
            //        SetColorRange(colorSelector, newColorRange);
            //    }
            //}
        }

        private void OnGotMouseCapture(object sender, MouseEventArgs e)
        {

        }

        private void hsb_ValuesChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
