using System.Windows;
using Lighting.Library;
using System.Drawing;

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
            LightPoint lp = new LightPoint(Color.FromArgb(169, 104, 54));
            lp.HslColor = new HslColor(30, 0.53, 0.44);
        }
    }
}
