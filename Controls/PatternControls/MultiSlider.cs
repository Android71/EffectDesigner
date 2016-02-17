/**
 * Restless Animal Development
 * This program is provided freely without restrictions, but please retain this notice in any distributions, modifications ,etc.
 * http://dev.restlessanimal.com
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
//using System.Windows.Media;
using Lighting.Library;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Media = System.Windows.Media;
using System.Drawing;
using System.Threading;

namespace Xam.Wpf.Controls
{
    /// <summary>
    /// Represents a slider control that can have multiple slider points.
    /// </summary>
    public class MultiSlider : Control
    {

        /************************************************************************/

        #region Private Fields Vars Enums

        private List<SliderItem> sliders;

        SliderItem selectedSliderItem = null;
        int clickedIx = -1;

        Grid sliderGridDown;
        Grid sliderGridUp;
        Grid sliderArea;
        TextBlock valueLabel;

        enum Mode { Point, Range};

        Mode workMode = Mode.Point;
        Button modeBtn;
        

        #endregion

        /************************************************************************/

        #region DP Properties

        #region SelectedPoint DP
        public PatternPoint SelectedPoint
        {
            get { return (PatternPoint)GetValue(SelectedPointProperty); }
            set { SetValue(SelectedPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPointProperty =
                                DependencyProperty.Register("SelectedPoint", typeof(PatternPoint), typeof(MultiSlider), 
                                    new FrameworkPropertyMetadata(null, OnSelectedPointChanged));


        

        #endregion

        #region Pattern DP

        public ObservableNotifiableCollection<PatternPoint> Pattern
        {
            get { return (ObservableNotifiableCollection<PatternPoint>)GetValue(PatternProperty); }
            set { SetValue(PatternProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Pattern.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PatternProperty =
                                DependencyProperty.Register("Pattern", typeof(ObservableNotifiableCollection<PatternPoint>), typeof(MultiSlider),
                                    new PropertyMetadata(null, OnPatternChanged));

        #endregion

        #region StripModel DP



        public ObservableNotifiableCollection<PatternPoint> StripModel
        {
            get { return (ObservableNotifiableCollection<PatternPoint>)GetValue(StripModelProperty); }
            set { SetValue(StripModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StripModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StripModelProperty =
            DependencyProperty.Register("StripModel", typeof(ObservableNotifiableCollection<PatternPoint>), typeof(MultiSlider), new PropertyMetadata(null, OnStripModelChanged));



        #endregion

        #region Minimum DP

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(MultiSlider),
            new FrameworkPropertyMetadata(1.0d, FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnMinimumPropertyChanged),
                new CoerceValueCallback(MinimumPropertyCoerce)
                ), new ValidateValueCallback(IsValidMinimum));

        /// <summary>
        /// Gets or sets the minumum value for the multi-slider.
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        #endregion

        #region Maximum DP

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(MultiSlider),
            new FrameworkPropertyMetadata(1000.0d, FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnMaximumPropertyChanged),
                new CoerceValueCallback(MaximumPropertyCoerce)
                ), new ValidateValueCallback(IsValidMaximum));

        /// <summary>
        /// Gets or sets the maximum value for the multi-slider.
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        #endregion

        #region Orientation DP

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(MultiSlider),
            new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnOrientationPropertyChanged)));

        /// <summary>
        /// Gets or sets the orientation of the multi-slider.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region  IsDirectionReversed DP

        public static readonly DependencyProperty IsDirectionReversedProperty =
            DependencyProperty.Register("IsDirectionReversed", typeof(bool), typeof(MultiSlider),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(OnIsDirectionReversedPropertyChanged)));

        /// <summary>
        /// Gets or sets the direction of increasing value.
        /// </summary>
        public bool IsDirectionReversed
        {
            get { return (bool)GetValue(IsDirectionReversedProperty); }
            set { SetValue(IsDirectionReversedProperty, value); }
        }

        #endregion

        #region TrackBrush DP

        public static readonly DependencyProperty TrackBrushProperty =
            DependencyProperty.Register("TrackBrush", typeof(Brush), typeof(MultiSlider),
            new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.None));

        public Brush TrackBrush
        {
            get { return (Brush)GetValue(TrackBrushProperty); }
            set { SetValue(TrackBrushProperty, value); }
        }



        #endregion

        #endregion

        /************************************************************************/

        #region DP Event handlers

        private static void OnSelectedPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int ix;
            PatternPoint pp = (PatternPoint)e.OldValue;
            MultiSlider ms = d as MultiSlider;
            //if (ms.clickedIx == -1)
            //{
            ix = ms.Pattern.IndexOf(ms.SelectedPoint);
            if (ms.selectedSliderItem != null)
                ms.selectedSliderItem.IsSelected = false;
            if ((ms.clickedIx != -1) && (ms.sliders[ms.clickedIx].Variant == 2))
                ms.selectedSliderItem = ms.sliders[ix + 1];
            else
                ms.selectedSliderItem = ms.sliders.FirstOrDefault(p => p.PatternIx == ix);
            ms.selectedSliderItem.IsSelected = true;
            //}
            ms.clickedIx = -1;
            ms.valueLabel.Text = ((int)ms.selectedSliderItem.Value).ToString();

            if (pp != null)
                (pp as INotifyPropertyChanged).PropertyChanged -= ms.OnPointColorChanged;
            (ms.SelectedPoint as INotifyPropertyChanged).PropertyChanged += ms.OnPointColorChanged;

        }

        private void OnPointColorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PointColor")
                UpdateModel();
        }

        
        private static void OnStripModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            ms.UpdateModel();
        }

        private static void OnPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = d as MultiSlider;
            ms.InsertSliders();
        }

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            ms.CoerceValue(MaximumProperty);
            //ms.RecalibrateSliders();
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            foreach (var s in ms.sliders)
            {
                s.Orientation = (Orientation)e.NewValue;
            }
        }

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            ms.CoerceValue(MinimumProperty);
            //ms.RecalibrateSliders();
        }

        private static void OnIsDirectionReversedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            foreach (var s in ms.sliders)
            {
                s.IsDirectionReversed = (bool)e.NewValue;
            }
        }


        #endregion

        /************************************************************************/

        #region Other Methods (private)


        private void SliderItemGotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (selectedSliderItem != null)
                selectedSliderItem.IsSelected = false;
            SliderItem si = sender as SliderItem;
            clickedIx = si.Position;
            selectedSliderItem = si;
            selectedSliderItem.IsSelected = true;
            SelectedPoint = Pattern[si.PatternIx];
            valueLabel.Text = ((int)selectedSliderItem.Value).ToString();
            //SelectedPoint.LedPos = (int)selectedSliderItem.Value;
        }

        private void OnSliderItemValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderItem s = sender as SliderItem;
            int ix = sliders.IndexOf(s);
            if (e.NewValue > e.OldValue)
            {
                if (ix != sliders.Count - 1)
                {
                    if (e.NewValue >= sliders[ix + 1].Value - 1)
                        s.Value = sliders[ix + 1].Value - 1;
                }
            }
            else
            {
                if (ix != 0)
                {
                    if (e.NewValue <= sliders[ix - 1].Value + 1)
                        s.Value = sliders[ix - 1].Value + 1;
                }
            }
            valueLabel.Text = ((int)s.Value).ToString();
            if (( s.Variant == 1)||(s.Variant == 0))
                SelectedPoint.LedPos = (int)s.Value;
            if (s.Variant == 1)
                SelectedPoint.LedCount = (int)(sliders[ix + 1].Value - s.Value + 1);
            if(s.Variant == 2)
                SelectedPoint.LedCount = (int)s.Value - (int)sliders[ix - 1].Value  + 1;
            //Console.WriteLine("LedPos: {0}   LedCount:  {1}", SelectedPoint.LedPos, SelectedPoint.LedCount);
            UpdateModel();
            
        }

        #endregion

        /************************************************************************/

        #region Constructors and Initilizers
        static MultiSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSlider), new FrameworkPropertyMetadata(typeof(MultiSlider)));
        }

        public MultiSlider()
        {
            
        }
        

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //if (Pattern != null)
            //    InsertSliders(); 
            sliderGridDown = Template.FindName("PART_SliderGrid_Down", this) as Grid;
            sliderGridUp = Template.FindName("PART_SliderGrid_Up", this) as Grid;
            sliderArea = Template.FindName("PART_Sliders", this) as Grid;
            valueLabel = Template.FindName("PART_CurrentPosition", this) as TextBlock;
            modeBtn = Template.FindName("PART_ModeBtn", this) as Button;
            //track = Template.FindName("PART_Track", this) as Border;

            //        sliderArea.AddHandler(
            //Grid.PreviewMouseLeftButtonDownEvent,
            //(MouseButtonEventHandler)OnSliderClick,
            //true
            //);


            sliderArea.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnSliderClick);
            //controlBlock = Template.FindName("PART_ControlBlock", this) as Grid;
            //pointBtn = Template.FindName("PART_PointBtn", this) as RadioButton;
            //pointBtn.Checked += PointBtn_Checked;
            //rangeBtn = Template.FindName("PART_RangeBtn", this) as RadioButton;
            //rangeBtn.Checked += RangeBtn_Checked;
            //addButton = Template.FindName("PART_AddBtn", this) as Button;
            modeBtn.Click += ModeButton_Click;
            //removeButton = Template.FindName("PART_RemoveBtn", this) as Button;
            //removeButton.Click += RemoveButton_Click;
            //valueLabel = controlBlock.Children[1] as Label;
        }
        /// <summary>
        /// Inserts the sliders into the template. 
        /// </summary>
        private void InsertSliders()
        {
            //int sliderCount = Pattern.Count;
            sliders = new List<SliderItem>();
            if ((sliderGridDown == null) || (sliderGridUp == null)) return;
            sliderGridDown.Children.Clear();
            sliderGridUp.Children.Clear();

            int i = 0;
            SliderItem s;
            //SliderItem s0;

            for (int k = 0; k < Pattern.Count; k++)
            {
                int num = 1;
                if (Pattern[k].LedCount != 1)
                    num = 2;
                for ( int j = 0; j < num; j++)
                {
                    s = new SliderItem(this, i);
                    sliders.Add(s);
                    s.Minimum = Minimum;
                    s.Maximum = Maximum;
                    s.SmallChange = 1;
                    s.TickFrequency = 1;
                    s.LargeChange = 1;
                    s.IsSnapToTickEnabled = true;
                    s.PatternIx = k;
                    s.Value = Pattern[k].LedPos;
                    if (( num == 2 ) && ( j == 0))

                        s.Variant = 1;
                    if ((num == 2) && (j == 1))
                    {
                        s.Variant = 2;
                        s.Value = Pattern[k].LedPos + Pattern[k].LedCount - 1;
                    }
                    s.GotMouseCapture += new System.Windows.Input.MouseEventHandler(SliderItemGotMouseCapture);
                    s.ValueChanged += new RoutedPropertyChangedEventHandler<double>(OnSliderItemValueChanged);
                    i++;
                }
            }

            for (int k = 0; k < sliders.Count; k++)
            { 
                if (k % 2 == 0)
                    // чётный 
                    sliderGridUp.Children.Add(sliders[k]);
                else
                    // нечётный
                    sliderGridDown.Children.Add(sliders[k]);
            }
        }

        #endregion

        /************************************************************************/

        #region Input Handlers

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            HSBcolor hsb = null;
            base.OnMouseWheel(e);
            //Color color;
            if (SelectedPoint != null)
            {
                //color = SelectedPoint.PointColor;
                //PatternPoint pp = StripModel[SelectedPoint.LedPos - 1];
                hsb = new HSBcolor(SelectedPoint.Hue, SelectedPoint.Saturation, SelectedPoint.Brightness);
                int bri = (int)(hsb.Brightness * 100.0);
                if (e.Delta > 0)
                {
                    if (bri + 5 > 100)
                    {
                        //hsb = color.HsbColor();

                        hsb.Brightness = 1.0;
                        //pp.PointBrightness = 1.0;
                        //SelectedPoint.PointBrightness = 1.0;
                        //SelectedPoint.HSB.Brightness = 1;
                        //hsb = new HSBcolor(SelectedPoint.HSB.Hue, SelectedPoint.HSB.Saturation, 1);
                        //pp.PointColor = hsb.RgbColor;
                        SelectedPoint.PointColor = hsb.RgbColor;
                    }
                    else
                    {
                        hsb.Brightness = (bri + 5.0) / 100.0;
                        //pp.PointBrightness = hsb.Brightness;
                        //SelectedPoint.PointBrightness = hsb.Brightness;
                        //hsb = new HSBcolor(SelectedPoint.HSB.Hue, SelectedPoint.HSB.Saturation, (bri + 5.0) / 100.0);
                        //pp.PointColor = hsb.RgbColor;
                        SelectedPoint.PointColor = hsb.RgbColor;
                    }
                }
                else
                {
                    if (bri - 5 < 1)
                    {
                        hsb.Brightness = 0.01;
                        //pp.PointColor = hsb.RgbColor;
                        SelectedPoint.PointColor = hsb.RgbColor;
                    }
                    else
                    {
                        hsb.Brightness = (bri - 5.0) / 100.0;
                        //pp.PointColor = hsb.RgbColor;
                        SelectedPoint.PointColor = hsb.RgbColor;
                    }
                }
                UpdateModel();
            }
        }

        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (workMode == Mode.Point)
            {
                workMode = Mode.Range;
                modeBtn.Content = "Диапазон";
            }
            else
            {
                workMode = Mode.Point;
                modeBtn.Content = "Точка";
            }
        }

        private void OnSliderClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {

                //Color clr = GetColorAtMouse();

                System.Windows.Point pt = e.GetPosition((UIElement)sender);
                //Console.WriteLine("X: {0} Y: {1}", pt.X, pt.Y);
                //Console.WriteLine("ActualWidth: {0}", sliderArea.ActualWidth);
                int ledPos = (int)(pt.X * Maximum / sliderArea.ActualWidth);
                //Console.WriteLine("LedPos: {0}", ledPos);
                PatternPoint leftPoint = null;
                PatternPoint rightPoint = null;
                foreach (PatternPoint pp in Pattern)
                {
                    if (ledPos > pp.LedPos)
                        leftPoint = pp;
                    //if (ledPos < pp.LedPos)
                    //    rightPos = pp;
                }
                for (int i = Pattern.Count - 1; i >= 0; i--)
                {
                    if (ledPos < Pattern[i].LedPos)
                        rightPoint = Pattern[i];
                }

                // первая точка
                if (leftPoint == null)
                {
                    Pattern.Insert(0, new PatternPoint(Color.Aquamarine, ledPos) { LedCount = 1 });
                    InsertSliders();
                    return;
                }

                // последняя точка
                if (rightPoint == null)
                {
                    Pattern.Add(new PatternPoint(Color.Aquamarine, ledPos) { LedCount = 1 });
                    InsertSliders();
                    return;
                }

                // елси внутри диапазона
                if (leftPoint.LedCount != 1)
                {
                    if (ledPos <= leftPoint.LedPos + leftPoint.LedCount - 1)
                        return;
                }

                int ix = Pattern.IndexOf(rightPoint);
                Pattern.Insert(ix, new PatternPoint(Color.Aquamarine, ledPos) { LedCount = 1 });

                InsertSliders();
            }
        }


        #endregion

        /************************************************************************/

        #region Main Methods

        private void UpdateModel()
        {
            PatternPoint previousPoint = null;

            Stopwatch watch = new Stopwatch();
            watch.Start();


            for (int i = 0; i < StripModel.Count; i++)
                StripModel[i].PointColor = Color.Black;


            foreach (PatternPoint pp in Pattern)
            {
                if (previousPoint != null)
                {
                    if (pp.LedCount == 1)
                    {
                        // какая бы не была предыдуяя точка строим градиент
                        MakeGradient( previousPoint, pp );
                        previousPoint = pp; 
                    }
                    else
                    {
                        // диапазон
                        MakeGradient(previousPoint, pp);
                        for (int i = 0; i < pp.LedCount; i++)
                            StripModel[pp.LedPos + i].PointColor = pp.PointColor;
                        previousPoint = new PatternPoint(pp.PointColor, pp.LedPos + pp.LedCount - 1);
                    }
                }
                else
                {
                    // первая точка
                    for (int i = 0; i < pp.LedPos; i++)
                        StripModel[i].PointColor = Color.Black;
                    previousPoint = pp;
                }
            }

            watch.Stop();
            Console.WriteLine("Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
        }

        private void MakeGradient(PatternPoint from, PatternPoint to)
        {
            HSBcolor hsb;
            //Color from = fromPoint.PointColor;
            //Color to = toPoint.PointColor;
            int stepCount = to.LedPos - from.LedPos; 
            //Console.WriteLine("From: {0}     To: {1}", from.LedPos, to.LedPos);
            double deltaHue = (to.Hue - from.Hue) / stepCount; 
            double deltaSat = (to.Saturation - from.Saturation) / stepCount; 
            double deltaBri = (to.Brightness - from.Brightness) / stepCount; 
            StripModel[from.LedPos - 1].PointColor = from.PointColor;
            StripModel[to.LedPos - 1].PointColor = to.PointColor;

            for (int i = 1; i < stepCount; i++)
            {
                hsb = new HSBcolor(from.Hue + i * deltaHue,
                                   from.Saturation + i * deltaSat,
                                   from.Brightness + i * deltaBri);
                StripModel[from.LedPos + i - 1].PointColor = hsb.RgbColor;
            }
        }
        
        #endregion

        /************************************************************************/

        #region Protected Methods
        #endregion

        /************************************************************************/


        /************************************************************************/

        #region DP Metadata methods (private static)


        private static bool IsValidMinimum(object value)
        {
            return IsValidDouble(value);
        }

        private static object MinimumPropertyCoerce(DependencyObject d, object value)
        {
            double max = ((MultiSlider)d).Maximum;
            double min = (double)value;
            return Math.Min(max, min);
        }

        private static bool IsValidMaximum(object value)
        {
            return IsValidDouble(value);
        }

        private static object MaximumPropertyCoerce(DependencyObject d, object value)
        {
            double min = ((MultiSlider)d).Minimum;
            double max = (double)value;
            return Math.Max(max, min);
        }

        private static bool IsValidDouble(object value)
        {
            Double v = (Double)value;
            return !Double.IsInfinity(v) && !Double.IsNaN(v);
        }

        #endregion

        /************************************************************************/

        #region TODO

        // ContrastColor value converter
        //http://andora.us/blog/2011/03/03/choosing-foreground-using-luminosity-contrast-ratio/
        Color ContrastColor(Color color)
        {
            int d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return Color.FromArgb(0, (byte)d, (byte)d, (byte)d);
        }

        #endregion

        /************************************************************************/

        #region PInovoke methods

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr window, IntPtr dc);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        public Color GetColorAtMouse()
        {
            POINT p;
            if (GetCursorPos(out p))
            {

                IntPtr desk = GetDesktopWindow();
                IntPtr dc = GetWindowDC(desk);
                int a = (int)GetPixel(dc, p.X, p.Y);
                ReleaseDC(desk, dc);
                return Color.FromArgb(255, (byte)((a >> 0) & 0xff), (byte)((a >> 8) & 0xff), (byte)((a >> 16) & 0xff));
            }
            else
                return Color.Black;
        }

        #endregion

    }
}
