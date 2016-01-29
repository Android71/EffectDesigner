using System.Windows;
using System.ComponentModel;
using Lighting.Library;
using System.Windows.Media;

namespace PatternStops
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ObservableNotifiableCollection<PatternPoint> pattern = new ObservableNotifiableCollection<PatternPoint>();
            pattern.Add(new PatternPoint(Colors.Blue, 2));
            pattern.Add(new PatternPoint(Colors.Red, 254));
            pattern.Add(new PatternPoint(Colors.Yellow, 607));
            pattern.Add(new PatternPoint(Colors.Green, 903));
            Pattern = pattern;
        }

        private ObservableNotifiableCollection<PatternPoint> _pattern;
        public ObservableNotifiableCollection<PatternPoint> Pattern
        {
            get { return _pattern; }
            set { if (value != _pattern) _pattern = value; OnPropertyChanged("Pattern"); }
        }

        private PatternPoint _selectedPoint;

        public PatternPoint SelectedPoint
        {
            get { return _selectedPoint; }
            set { if (_selectedPoint != value ) _selectedPoint = value; OnPropertyChanged("SelectedPoint"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _selectedPatternIx = -1;

        public int SelectedPatternIx
        {
            get { return _selectedPatternIx; }
            set { if ( _selectedPatternIx != value ) _selectedPatternIx = value; OnPropertyChanged("SelectedPatternIx"); }
        }

    }
}
