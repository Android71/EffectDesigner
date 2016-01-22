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
using System.Windows.Controls.Primitives;

namespace EffectDesigner
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

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

            Thumb thumb = e.Source as Thumb;

            //thumb.Margin.Left += e.HorizontalChange;
            horizDelta.Text = e.HorizontalChange.ToString();
            Thickness th = new Thickness(thumb.Margin.Left, thumb.Margin.Top, thumb.Margin.Right, thumb.Margin.Bottom);
            th.Left += e.HorizontalChange;

            thumb.Margin = th;
            //Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
            //Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);


        }
    }
}
