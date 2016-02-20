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
//using System.Windows.Media;
using Media = System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Drawing;
using Lighting.Library;

namespace EffectDesigner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            ObservableNotifiableCollection<PatternPoint> pattern = new ObservableNotifiableCollection<PatternPoint>();
            pattern.Add(new PatternPoint(Color.Blue, 2) { LedCount = 1 });
            pattern.Add(new PatternPoint(Color.Red, 50) { LedCount = 40 });
            pattern.Add(new PatternPoint(Color.Yellow, 120) { LedCount = 1 });
            pattern.Add(new PatternPoint(Color.Green, 170) { LedCount = 1 });
            Pattern = pattern;



            ObservableNotifiableCollection<PatternPoint> stripModel = new ObservableNotifiableCollection<PatternPoint>();
            for (int i = 0; i < 170; i++)
                stripModel.Add(new PatternPoint(Color.Red, i));
            StripModel = stripModel;
        }

        private ObservableNotifiableCollection<PatternPoint> _pattern;
        public ObservableNotifiableCollection<PatternPoint> Pattern
        {
            get { return _pattern; }
            set { if (value != _pattern) _pattern = value; OnPropertyChanged("Pattern"); }
        }

        private ObservableNotifiableCollection<PatternPoint> _stripModel;
        public ObservableNotifiableCollection<PatternPoint> StripModel
        {
            get { return _stripModel; }
            set { if (value != _stripModel) _stripModel = value; OnPropertyChanged("StripModel"); }
        }

        private PatternPoint _selectedPoint;
        public PatternPoint SelectedPoint
        {
            get { return _selectedPoint; }
            set { if (_selectedPoint != value) _selectedPoint = value; OnPropertyChanged("SelectedPoint"); }
        }

        private int _selectedPatternIx = -1;
        public int SelectedPatternIx
        {
            get { return _selectedPatternIx; }
            set { if (_selectedPatternIx != value) _selectedPatternIx = value; OnPropertyChanged("SelectedPatternIx"); }
        }
    }
}
