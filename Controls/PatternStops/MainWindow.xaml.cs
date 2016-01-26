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

namespace PatternStops
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MultiSlider_SliderSet(object sender, Xam.Wpf.Controls.MultiSliderSetRoutedEventArgs e)
        {
            if (listBox != null)
                listBox.ItemsSource = e.SliderValues;
        }

        private void MultiSlider_ValueChanged(object sender, Xam.Wpf.Controls.MultiSliderRoutedEventArgs e)
        {
            //position.Text = e.Position.ToString();
            position.Text = ((int)e.SliderValues[e.Position]).ToString();
        }
    }
}
