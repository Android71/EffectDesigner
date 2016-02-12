﻿/**
 * Restless Animal Development
 * This program is provided freely without restrictions, but please retain this notice in any distributions, modifications ,etc.
 * http://dev.restlessanimal.com
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using Lighting.Library;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.ComponentModel;

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

        //Grid controlBlock;
        //Label valueLabel;
        Grid sliderGridDown;
        Grid sliderGridUp;
        Grid sliderArea;
        //Border track;
        TextBlock valueLabel;

        enum Mode { Point, Range};

        Mode workMode = Mode.Point;
        Button modeBtn;
        

        #endregion

        /************************************************************************/

        #region Properties

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

        private static void OnPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = d as MultiSlider;
            ms.InsertSliders();
        }

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

        private static void OnStripModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }



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

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            ms.CoerceValue(MaximumProperty);
            //ms.RecalibrateSliders();
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

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            ms.CoerceValue(MinimumProperty);
            //ms.RecalibrateSliders();
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

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            foreach (var s in ms.sliders)
            {
                s.Orientation = (Orientation)e.NewValue;
            }
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

        private static void OnIsDirectionReversedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            foreach (var s in ms.sliders)
            {
                s.IsDirectionReversed = (bool)e.NewValue;
            }
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

        #region Routed Events
        /// <summary>
        ///  Identifies the SliderSet routed event.
        /// </summary>
        //public static readonly RoutedEvent SliderSetEvent =
        //    EventManager.RegisterRoutedEvent("SliderSet", RoutingStrategy.Bubble, typeof(MultiSliderSetRoutedEventHandler), typeof(MultiSlider));

        /// <summary>
        /// Occurs when the sliders are set. Sliders are set at initialization time, 
        /// each time the SliderCount property changes, and when Minimum, Maximum, or Cushion properties change.
        /// </summary>
        //public event MultiSliderSetRoutedEventHandler SliderSet
        //{
        //    add
        //    {
        //        AddHandler(MultiSlider.SliderSetEvent, value);
        //    }
        //    remove
        //    {
        //        RemoveHandler(MultiSlider.SliderSetEvent, value);
        //    }
        //}

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        //public static readonly RoutedEvent ValueChangedEvent =
        //    EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(MultiSliderRoutedEventHandler), typeof(MultiSlider));

        /// <summary>
        /// Occurs when one of the values on the multi-slider changes.
        /// </summary>
        //public event MultiSliderRoutedEventHandler ValueChanged
        //{
        //    add
        //    {
        //        AddHandler(MultiSlider.ValueChangedEvent, value);
        //    }
        //    remove
        //    {
        //        RemoveHandler(MultiSlider.ValueChangedEvent, value);
        //    }
        //}

        /// <summary>
        /// Identifies the SliderSelected routed event.
        /// </summary>
        //public static readonly RoutedEvent SliderSelectedEvent =
        //    EventManager.RegisterRoutedEvent("SliderSelected", RoutingStrategy.Bubble, typeof(MultiSliderRoutedEventHandler), typeof(MultiSlider));

        ///// <summary>
        ///// Occurs when one of the slider points is selected.
        ///// </summary>
        //public event MultiSliderRoutedEventHandler SliderSelected
        //{
        //    add
        //    {
        //        AddHandler(MultiSlider.SliderSelectedEvent, value);
        //    }
        //    remove
        //    {
        //        RemoveHandler(MultiSlider.SliderSelectedEvent, value);
        //    }
        //}
        #endregion

        /************************************************************************/

        #region Constructors
        static MultiSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSlider), new FrameworkPropertyMetadata(typeof(MultiSlider)));
        }

        public MultiSlider()
        {
            
        }
        #endregion

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            HSBcolor hsb = null;
            base.OnMouseWheel(e);
            if (SelectedPoint != null)
            {
                int bri = (int)(SelectedPoint.HSB.Brightness * 100);
                if (e.Delta > 0)
                {
                    if (bri + 5 > 100)
                    {
                        SelectedPoint.HSB.Brightness = 1;
                        hsb = new HSBcolor(SelectedPoint.HSB.Hue, SelectedPoint.HSB.Saturation, 1);
                        SelectedPoint.PointColor = hsb.HsbToRgb();
                    }
                    else
                    {
                        hsb = new HSBcolor(SelectedPoint.HSB.Hue, SelectedPoint.HSB.Saturation, (bri + 5.0) / 100.0);
                        SelectedPoint.PointColor = hsb.HsbToRgb();
                    }
                }
                else
                {
                    if (bri - 5 < 1)
                    {
                        SelectedPoint.HSB.Brightness = 0;
                        hsb = new HSBcolor(SelectedPoint.HSB.Hue, SelectedPoint.HSB.Saturation, 0.01);
                        SelectedPoint.PointColor = hsb.HsbToRgb();
                    }
                    else
                    {
                        hsb = new HSBcolor(SelectedPoint.HSB.Hue, SelectedPoint.HSB.Saturation, (bri - 5.0) / 100.0);
                        SelectedPoint.PointColor = hsb.HsbToRgb();
                    }
                }
            }
        }

        /************************************************************************/

        #region Public Methods

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

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPoint != null)
                RemovePatternItem();
        }

        private void RemovePatternItem()
        {
            
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

        private void UpdateModel()
        {
            PatternPoint previousPoint = null;

            Stopwatch watch = new Stopwatch();
            watch.Start();


            for (int i = 0; i < StripModel.Count; i++)
                StripModel[i].PointColor = Colors.Black;

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
                        StripModel[i].PointColor = Colors.Black;
                    previousPoint = pp;
                }
            }

            watch.Stop();
            Console.WriteLine("Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
        }

        private void MakeGradient(PatternPoint from, PatternPoint to)
        {
            HSBcolor hsb;
            int stepCount = to.LedPos - from.LedPos; ;
            double deltaHue = (to.HSB.Hue - from.HSB.Hue) / stepCount; ;
            double deltaSat = (to.HSB.Saturation - from.HSB.Saturation) / stepCount; ;
            double deltaBri = (to.HSB.Brightness - from.HSB.Brightness) / stepCount; ;
            StripModel[from.LedPos - 1].PointColor = from.PointColor;
            StripModel[to.LedPos - 1].PointColor = to.PointColor;

            for (int i = 0; i < stepCount; i++)
            {
                hsb = new HSBcolor(from.HSB.Hue + i * deltaHue,
                                   from.HSB.Saturation + i * deltaSat,
                                   from.HSB.Brightness + i * deltaBri);
                StripModel[from.LedPos + i].PointColor = hsb.HsbToRgb();
            }
        }

        private void AddPatternItem()
        {
            
        }

        private void RangeBtn_Checked(object sender, RoutedEventArgs e)
        {
            workMode = Mode.Range;
        }

        private void PointBtn_Checked(object sender, RoutedEventArgs e)
        {
            workMode = Mode.Point;
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        #endregion

        /************************************************************************/

        #region Other Methods (private)
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
                    //s.MouseDoubleClick += new MouseButtonEventHandler(OnSliderDoubleClick);
                    i++;
                }
                
                
                //if (Pattern[k].LedCount != 1)
                //{
                    //s = new SliderItem(this, i);
                    //sliders.Add(s);
                    //s.Minimum = Minimum;
                    //s.Maximum = Maximum;
                    //s.SmallChange = 1;
                    //s.TickFrequency = 1;
                    //s.LargeChange = 1;
                    //s.IsSnapToTickEnabled = true;
                    //s.PatternIx = k;
                    //s.Value = Pattern[k].LedPos;
                    //s.Variant = 1;      //LeftLimit
                    //s.GotMouseCapture += new System.Windows.Input.MouseEventHandler(SliderItemGotMouseCapture);
                    //s.ValueChanged += new RoutedPropertyChangedEventHandler<double>(OnSliderItemValueChanged);
                    //i++;
                //if (Pattern[k].LedCount != 1)
                //{
                //    s = new SliderItem(this, i);
                //    sliders.Add(s);
                //    s.Minimum = Minimum;
                //    s.Maximum = Maximum;
                //    s.SmallChange = 1;
                //    s.TickFrequency = 1;
                //    s.LargeChange = 1;
                //    s.IsSnapToTickEnabled = true;
                //    s.PatternIx = k;
                //    s.Value = Pattern[k].LedPos + Pattern[k].LedCount - 1;
                //    s.Variant = 2;      //RightLimit
                //    s.GotMouseCapture += new System.Windows.Input.MouseEventHandler(SliderItemGotMouseCapture);
                //    s.ValueChanged += new RoutedPropertyChangedEventHandler<double>(OnSliderItemValueChanged);
                //    i++;
                //}
                //else
                //{
                //    s = new SliderItem(this, i);
                //    sliders.Add(s);
                //    s.Minimum = Minimum;
                //    s.Maximum = Maximum;
                //    s.SmallChange = 1;
                //    s.TickFrequency = 1;
                //    s.LargeChange = 1;
                //    s.IsSnapToTickEnabled = true;
                //    s.PatternIx = k;
                //    s.Value = Pattern[k].LedPos;
                //    s.Variant = 0;      //Normal
                //    s.GotMouseCapture += new System.Windows.Input.MouseEventHandler(SliderItemGotMouseCapture);
                //    s.ValueChanged += new RoutedPropertyChangedEventHandler<double>(OnSliderItemValueChanged);
                //    i++;
                //}
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

        private void OnSliderClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {

                //Color clr = GetColorAtMouse();

                Point pt = e.GetPosition((UIElement)sender);
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
                    Pattern.Insert(0, new PatternPoint(Colors.Aquamarine, ledPos) { LedCount = 1 });
                    InsertSliders();
                    return;
                }

                // последняя точка
                if (rightPoint == null)
                {
                    Pattern.Add(new PatternPoint(Colors.Aquamarine, ledPos) { LedCount = 1 });
                    InsertSliders();
                    return;
                }

                // елси внутри диапазона
                if ( leftPoint.LedCount != 1)
                {
                    if (ledPos <= leftPoint.LedPos + leftPoint.LedCount - 1)
                        return;
                }

                int ix = Pattern.IndexOf(rightPoint);
                Pattern.Insert(ix, new PatternPoint(Colors.Aquamarine, ledPos) { LedCount = 1 });

                InsertSliders();
            }
        }       

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

        #region Dependency Property Metadata methods (private static)


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
                return Colors.Black;
        }

        
    }
}
