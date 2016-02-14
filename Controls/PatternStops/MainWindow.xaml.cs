using System.Windows;
using System.ComponentModel;
using Lighting.Library;
//using System.Windows.Media;
using System.Drawing;

namespace PatternStops
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
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
                stripModel.Add(new PatternPoint(Color.Black, i));
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
            set { if (_selectedPoint != value ) _selectedPoint = value; OnPropertyChanged("SelectedPoint"); }
        }

        private int _selectedPatternIx = -1;
        public int SelectedPatternIx
        {
            get { return _selectedPatternIx; }
            set { if ( _selectedPatternIx != value ) _selectedPatternIx = value; OnPropertyChanged("SelectedPatternIx"); }
        }

        //private ObservableNotifiableCollection<ColorTablePoint> _colorTable;
        //public ObservableNotifiableCollection<ColorTablePoint> ColorTable
        //{
        //    get { return _colorTable; }
        //    set { if (value != _colorTable) _colorTable = value; OnPropertyChanged("ColorTable"); }
        //}
    }
}
