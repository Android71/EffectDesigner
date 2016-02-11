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
            pattern.Add(new PatternPoint(Colors.Blue, 20) { LedCount = 1 });
            pattern.Add(new PatternPoint(Colors.Red, 90) { LedCount = 230 });
            pattern.Add(new PatternPoint(Colors.Yellow, 800) { LedCount = 1 });
            pattern.Add(new PatternPoint(Colors.Green, 1440) { LedCount = 1 });
            Pattern = pattern;

            ObservableNotifiableCollection<ColorTablePoint> colorTable = new ObservableNotifiableCollection<ColorTablePoint>();
            colorTable.Add(new ColorTablePoint(Color.FromRgb(255, 0, 128), new Point(330, 360)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(255, 0, 255), new Point(300, 330)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(128, 0, 255), new Point(270, 300)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(0, 0, 255), new Point(240, 270)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(0, 128, 255), new Point(210, 240)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(0, 255, 255), new Point(180, 210)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(0, 255, 128), new Point(150, 180)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(0, 255, 0), new Point(120, 150)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(128, 255, 0), new Point(90, 120)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(255, 255, 0), new Point(60, 90)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(255, 128, 0), new Point(30, 60)));
            colorTable.Add(new ColorTablePoint(Color.FromRgb(255, 0, 0), new Point(0, 30)));
            ColorTable = colorTable;

            ObservableNotifiableCollection<PatternPoint> stripModel = new ObservableNotifiableCollection<PatternPoint>();
            for (int i = 0; i < 1440; i++)
                stripModel.Add(new PatternPoint(Colors.Black, i));
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

        private ObservableNotifiableCollection<ColorTablePoint> _colorTable;
        public ObservableNotifiableCollection<ColorTablePoint> ColorTable
        {
            get { return _colorTable; }
            set { if (value != _colorTable) _colorTable = value; OnPropertyChanged("ColorTable"); }
        }

        private void MultiSlider_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Retrieve the coordinate of the mouse position.
            Point pt = e.GetPosition((UIElement)sender);

            //// Perform the hit test against a given portion of the visual object tree.
            //HitTestResult result = VisualTreeHelper.HitTest(slidersAre, pt);
        }
    }
}
