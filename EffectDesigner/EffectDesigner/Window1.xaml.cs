using System.Windows;
using Lighting.Library;
using System.Drawing;
using System.ComponentModel;

namespace EffectDesigner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Window1 : Window , INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Window1()
        {
            InitializeComponent();
            DataContext = this;
            SelectedPoint = new PatternPoint(Color.FromArgb(169, 104, 54), 1);
            //SelectedPoint = new PatternPoint(Color.FromArgb(100, 100, 48), 1);
            //lp.HslColor = new HslColor(30, 0.53, 0.44);
        }

        private PatternPoint _selectedPoint;
        public PatternPoint SelectedPoint
        {
            get { return _selectedPoint; }
            set { if (_selectedPoint != value) _selectedPoint = value; OnPropertyChanged("SelectedPoint"); }
        }

        private void mouseWheel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (SelectedPoint != null)
            //{
            //    HslColor hsl = SelectedPoint.HslColor;
            //    double delta = (double)e.NewValue - (double)e.OldValue;
            //    if (delta > 0)
            //    {
            //        if (SelectedPoint.HslColor.Lightness + delta >= 1.0)
            //            SelectedPoint.HslColor = new HslColor(hsl.Hue, hsl.Saturation, 1.0);
            //        else
            //            SelectedPoint.HslColor = new HslColor(hsl.Hue, hsl.Saturation, hsl.Lightness + delta);
            //    }
            //    else
            //    {
            //        if (SelectedPoint.HslColor.Lightness + delta <= 0.0)
            //            SelectedPoint.HslColor = new HslColor(hsl.Hue, hsl.Saturation, 0.0);
            //        else
            //            SelectedPoint.HslColor = new HslColor(hsl.Hue, hsl.Saturation, hsl.Lightness + delta);
            //    }
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedPoint = new PatternPoint(Color.FromArgb(100, 100, 48), 1);
        }
    }
}
