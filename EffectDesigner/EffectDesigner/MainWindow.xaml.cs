using System.Windows;
//using System.Windows.Media;
using System.ComponentModel;
using System.Drawing;
using Lighting.Library;
using ArtNet;
using ArtNet.Packets;
using System.Net;
using System;

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

            multiSlider.SendPacket += SendDMXPacket;

            InitArtNet();

            

            ObservableNotifiableCollection<PatternPoint> pattern = new ObservableNotifiableCollection<PatternPoint>();

            //pattern.Add(new PatternPoint(Color.Blue, 2) { LedCount = 1 });
            //pattern.Add(new PatternPoint(Color.Red, 40) { LedCount = 30, Variant = PointVariant.Range });
            //pattern.Add(new PatternPoint(Color.Yellow, 120) { LedCount = 1 });
            //pattern.Add(new PatternPoint(Color.Orange, 140) { LedCount = 5, Variant = PointVariant.Range });
            //pattern.Add(new PatternPoint(Color.Green, 170) { LedCount = 1 });
            //multiSlider.Maximum = 170;

            pattern.Add(new PatternPoint(Color.Blue, 2) { LedCount = 1 });
            pattern.Add(new PatternPoint(Color.Red, 40) { LedCount = 30, Variant = PointVariant.Range });
            pattern.Add(new PatternPoint(Color.Yellow, 120) { LedCount = 1 });
            pattern.Add(new PatternPoint(Color.FromArgb(51, 105, 255), 140) { LedCount = 1, Variant = PointVariant.Lightness });
            pattern.Add(new PatternPoint(Color.Green, 170) { LedCount = 1 });
            multiSlider.Maximum = 170;

            //pattern.Add(new PatternPoint(Color.FromArgb(0, 0, 153), 1) { LedCount = 1 });
            //pattern.Add(new PatternPoint(Color.FromArgb(51, 105, 255), 5) { LedCount = 1, Variant = PointVariant.Lightness });
            //pattern.Add(new PatternPoint(Color.FromArgb(0, 119, 255), 8) { LedCount = 1, Variant = PointVariant.Lightness });
            //pattern.Add(new PatternPoint(Color.FromArgb(0, 136, 204), 11) { LedCount = 1, Variant = PointVariant.Lightness });
            //pattern.Add(new PatternPoint(Color.FromArgb(0, 133, 153), 14) { LedCount = 1 });
            //multiSlider.Maximum = 14;

            Pattern = pattern;
            
            LightMarkers = new ObservableNotifiableCollection<PatternPoint>();

            ObservableNotifiableCollection<PatternPoint> stripModel = new ObservableNotifiableCollection<PatternPoint>();
            for (int i = 0; i < multiSlider.Maximum; i++)
                stripModel.Add(new PatternPoint(Color.Red, i));
            StripModel = stripModel;
        }

        private ObservableNotifiableCollection<PatternPoint> _pattern;
        public ObservableNotifiableCollection<PatternPoint> Pattern
        {
            get { return _pattern; }
            set { if (value != _pattern) _pattern = value; OnPropertyChanged("Pattern"); }
        }

        private ObservableNotifiableCollection<PatternPoint> _lightMarkers;
        public ObservableNotifiableCollection<PatternPoint> LightMarkers
        {
            get { return _lightMarkers; }
            set { if (value != _lightMarkers) _lightMarkers = value; OnPropertyChanged("LightMarkers"); }
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

        void SendDMXPacket(object sender, EventArgs e)
        {
            Color c;
            short uniNo = short.Parse(uni.Text);
            for (int i = 0; i < StripModel.Count; i++)
            {
                c = StripModel[i].PointColor;

                DMXdata[i * 3] = gamma[c.G];
                DMXdata[i * 3 + 1] = gamma[c.R];
                DMXdata[i * 3 + 2] = gamma[c.B];
            }
            artDMXPacket.Universe = uniNo;
            socket.SendTo(AN6USPI, artDMXPacket);
        }

        
        void InitArtNet()
        {
            string hostName = Dns.GetHostName();
            //Console.WriteLine("Local hostname: {0}", hostName);
            IPHostEntry myself = Dns.GetHostEntry(hostName);
            foreach (IPAddress address in myself.AddressList)
            {
                if ((address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) && (address.ToString().StartsWith("2")))
                {
                    localIP = address;
                }
            }
            socket = new ArtNetSocket();
            //socket.NewPacket += new EventHandler<NewPacketEventArgs<ArtNetPacket>>(newPacket);
            socket.Open(localIP, subnetMask);
            artDMXPacket.DmxData = DMXdata;
        }

        ArtNetSocket socket;
        IPAddress localIP;
        IPAddress subnetMask = IPAddress.Parse("255.0.0.0");
        IPAddress AN6USPI = IPAddress.Parse("2.0.0.2");

        byte[] DMXdata = new byte[512];

        ArtNetDmxPacket artDMXPacket = new ArtNetDmxPacket();

        byte[] gamma = new byte[]
                      { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  1,  1,  1,  1,
                        1,  1,  1,  1,  1,  1,  1,  1,  1,  2,  2,  2,  2,  2,  2,  2,
                        2,  3,  3,  3,  3,  3,  3,  3,  4,  4,  4,  4,  4,  5,  5,  5,
                        5,  6,  6,  6,  6,  7,  7,  7,  7,  8,  8,  8,  9,  9,  9, 10,
                       10, 10, 11, 11, 11, 12, 12, 13, 13, 13, 14, 14, 15, 15, 16, 16,
                       17, 17, 18, 18, 19, 19, 20, 20, 21, 21, 22, 22, 23, 24, 24, 25,
                       25, 26, 27, 27, 28, 29, 29, 30, 31, 32, 32, 33, 34, 35, 35, 36,
                       37, 38, 39, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 50,
                       51, 52, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 66, 67, 68,
                       69, 70, 72, 73, 74, 75, 77, 78, 79, 81, 82, 83, 85, 86, 87, 89,
                       90, 92, 93, 95, 96, 98, 99,101,102,104,105,107,109,110,112,114,
                      115,117,119,120,122,124,126,127,129,131,133,135,137,138,140,142,
                      144,146,148,150,152,154,156,158,160,162,164,167,169,171,173,175,
                      177,180,182,184,186,189,191,193,196,198,200,203,205,208,210,213,
                      215,218,220,223,225,228,231,233,236,239,241,244,247,249,252,255 };
        
    }
}
