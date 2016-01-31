using System.Windows.Media;

namespace Lighting.Library
{
    public class PatternPoint : LightPoint
    {
        public PatternPoint(Color color, int ledPosition) : base(color)
        {
            LedPos = ledPosition;
        }

        private int _ledPos;
        public int LedPos
        {
            get { return _ledPos; }

            set { if (_ledPos != value) { _ledPos = value; OnPropertyChanged("LedPos"); } }
        }
    }
}
