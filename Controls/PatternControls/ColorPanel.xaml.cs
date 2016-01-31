using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PatternControls
{
    /// <summary>
    /// Логика взаимодействия для ColorPanel.xaml
    /// </summary>
    public partial class ColorPanel : UserControl
    {
        Border selectedColorRange = null;
        Brush[] brushArray;
        Point[] rangeArray;
        public ColorPanel()
        {
            InitializeComponent();
            brushArray = new Brush[]
            {
                new SolidColorBrush(Color.FromRgb(255, 0, 128)),
                 new SolidColorBrush(Color.FromRgb(255, 0, 255)),
                  new SolidColorBrush(Color.FromRgb(128, 0, 255)),
                   new SolidColorBrush(Color.FromRgb(0, 0, 255)),
                    new SolidColorBrush(Color.FromRgb(0, 128, 255)),
                     new SolidColorBrush(Color.FromRgb(0, 255, 255)),
                      new SolidColorBrush(Color.FromRgb(0, 255, 128)),
                       new SolidColorBrush(Color.FromRgb(0, 255, 0)),
                        new SolidColorBrush(Color.FromRgb(128, 255, 0)),
                         new SolidColorBrush(Color.FromRgb(255, 255, 0)),
                          new SolidColorBrush(Color.FromRgb(255, 128, 0)),
                           new SolidColorBrush(Color.FromRgb(255, 0, 0)),
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
                //border.Width = 25;
                border.Margin = new Thickness(2);
                border.Background = brushArray[i];
                border.Tag = rangeArray[i];
                border.MouseLeftButtonUp += new MouseButtonEventHandler(BorderMouseLeftUp);
                colorSelector.Children.Add(border);
            }
            
        }

        private void BorderMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                if (selectedColorRange != null)
                {
                    selectedColorRange.Margin = new Thickness(2);
                    selectedColorRange.BorderThickness = new Thickness(0);
                }
                border.Margin = new Thickness(0);
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                border.BorderThickness = new Thickness(1);
                selectedColorRange = border;
            }
        }
    }
}
