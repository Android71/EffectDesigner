/**
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

namespace Xam.Wpf.Controls
{
    /// <summary>
    /// Represents a slider control that can have multiple slider points.
    /// </summary>
    public class MultiSlider : Control
    {

        /************************************************************************/

        #region Private Fields and Vars
        private List<SliderItem> sliders;

        SliderItem selectedSliderItem = null;

        Grid controlBlock;
        Label valueLabel;

        #endregion

        /************************************************************************/

        #region Properties



        //public SliderItem SelectedSliderItem
        //{
        //    get { return (SliderItem)GetValue(SelectedSliderItemProperty); }
        //    set { SetValue(SelectedSliderItemProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SelectedSliderItemProperty =
        //                        DependencyProperty.Register("SelectedItem", typeof(SliderItem), typeof(MultiSlider), 
        //                        new PropertyMetadata(null, OnSelectedSliderItemChanged));

        //private static void OnSelectedSliderItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
            
        //}

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
            MultiSlider ms = d as MultiSlider;
            ms.SetSelection(ms.Pattern.IndexOf((PatternPoint)e.NewValue));
        }

        private void SetSelection(int index)
        {
            if (selectedSliderItem != null)
                selectedSliderItem.IsSelected = false;
            sliders[index].IsSelected = true;
            selectedSliderItem = sliders[index];
            valueLabel.Content = ((int)sliders[index].Value).ToString();
        }

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

        //public int SelectedPatternIx
        //{
        //    get { return (int)GetValue(SelectedPatternIxProperty); }
        //    set { SetValue(SelectedPatternIxProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for SelectedPatternIx.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SelectedPatternIxProperty =
        //    DependencyProperty.Register("SelectedPatternIx", typeof(int), typeof(MultiSlider), new PropertyMetadata(-1, OnSelectedPatternIxChanged));

        //private static void OnSelectedPatternIxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    MultiSlider ms = d as MultiSlider;

        //    SliderItem s = ms.sliders[(int)e.NewValue];
        //    if (ms.selectedSliderItem != null)
        //        ms.selectedSliderItem.IsSelected = false;
        //    s.IsSelected = true;
        //    ms.selectedSliderItem = s;
        //}



        ///// <summary>
        ///// Registers a dependency property as backing store for the SliderCount property
        ///// </summary>
        //public static readonly DependencyProperty SliderCountProperty =
        //    DependencyProperty.Register("SliderCount", typeof(int), typeof(MultiSlider),
        //    new FrameworkPropertyMetadata(DefaultSliderCount, FrameworkPropertyMetadataOptions.None,
        //        new PropertyChangedCallback(OnSliderCountPropertyChanged)
        //        ), new ValidateValueCallback(IsValidSliderCount));

        ///// <summary>
        ///// Gets or sets the slider count.
        ///// </summary>
        //public int SliderCount
        //{
        //    get { return (int)GetValue(SliderCountProperty); }
        //    set { SetValue(SliderCountProperty, value); }
        //}

        /// <summary>
        /// Registers a dependency property as backing store for the Minimum property
        /// </summary>
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

        /// <summary>
        /// Registers a dependency property as backing store for the Cushion property
        /// </summary>
        //public static readonly DependencyProperty CushionProperty =
        //    DependencyProperty.Register("Cushion", typeof(double), typeof(MultiSlider),
        //    new FrameworkPropertyMetadata(DefaultCushion, FrameworkPropertyMetadataOptions.None,
        //        new PropertyChangedCallback(OnCushionPropertyChanged)
        //        ), new ValidateValueCallback(IsValidCushion));

        /// <summary>
        /// Gets or sets the cushion value (MinCushion - MaxCushion),
        /// a percentage that indicates how close one slider can get to another.
        /// </summary>
        //public double Cushion
        //{
        //    get { return (double)GetValue(CushionProperty); }
        //    set { SetValue(CushionProperty, value); }
        //}

        /// <summary>
        /// Registers a dependency property as backing store for the Orientation property
        /// </summary>
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

        /// <summary>
        /// Registers a dependency property as backing store for the IsDirectionReversed property
        /// </summary>
        //public static readonly DependencyProperty IsDirectionReversedProperty =
        //    DependencyProperty.Register("IsDirectionReversed", typeof(bool), typeof(MultiSlider),
        //    new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None,
        //        new PropertyChangedCallback(OnIsDirectionReversedPropertyChanged)));

        /// <summary>
        /// Gets or sets the direction of increasing value.
        /// </summary>
        //public bool IsDirectionReversed
        //{
        //    get { return (bool)GetValue(IsDirectionReversedProperty); }
        //    set { SetValue(IsDirectionReversedProperty, value); }
        //}

        public static readonly DependencyProperty TrackBrushProperty =
            DependencyProperty.Register("TrackBrush", typeof(Brush), typeof(MultiSlider),
            new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.None));

        public Brush TrackBrush
        {
            get { return (Brush)GetValue(TrackBrushProperty); }
            set { SetValue(TrackBrushProperty, value); }
        }

        
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

        /************************************************************************/

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Pattern != null)
                InsertSliders();            
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        #endregion

        /************************************************************************/

        #region Other Methods (private)
        /// <summary>
        /// Inserts the sliders into the template. This method is called
        /// when the template is applied.
        /// </summary>
        /// <param name="sliderCount">The total number of possible sliders.</param>
        private void InsertSliders()
        {
            int sliderCount = Pattern.Count;
            //double[] values = new double[] { Minimum, 330, 660, Maximum };
            sliders = new List<SliderItem>();
            controlBlock = Template.FindName("PART_ControlBlock", this) as Grid;
            valueLabel = controlBlock.Children[1] as Label;

            Grid sliderGridDown = Template.FindName("PART_SliderGrid_Down", this) as Grid;
            Grid sliderGridUp = Template.FindName("PART_SliderGrid_Up", this) as Grid;
            if ((sliderGridDown == null) || (sliderGridUp == null)) return;
            sliderGridDown.Children.Clear();
            sliderGridUp.Children.Clear();

            for (int k = 0; k < sliderCount; k++)
            {
                SliderItem s = new SliderItem(this, k);
                sliders.Add(s);
                s.Minimum = Minimum;
                s.Maximum = Maximum;
                s.Value = Pattern[k].LedPos;
                s.GotMouseCapture += new System.Windows.Input.MouseEventHandler(MultiSliderSliderGotMouseCapture);
                s.ValueChanged += new RoutedPropertyChangedEventHandler<double>(OnSliderValueChanged);
                s.Orientation = Orientation.Horizontal;
                s.IsDirectionReversed = false;
                if (k % 2 == 0)
                    // чётный 
                    sliderGridUp.Children.Add(sliders[k]);
                else
                    // нечётный
                    sliderGridDown.Children.Add(sliders[k]);
            }            
        }

        private void MultiSliderSliderGotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            SliderItem s = sender as SliderItem;
            SelectedPoint = Pattern[sliders.IndexOf(s)];
            SelectedPoint.LedPos = (int)s.Value;
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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
            valueLabel.Content = ((int)s.Value).ToString();
            SelectedPoint.LedPos = (int)s.Value;
        }
        
        #endregion

        /************************************************************************/

        #region Dependency Property Metadata methods (private static)


        private static bool IsValidMinimum(object value)
        {
            return IsValidDouble(value);
        }

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            ms.CoerceValue(MaximumProperty);
            //ms.RecalibrateSliders();
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

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            ms.CoerceValue(MinimumProperty);
            //ms.RecalibrateSliders();
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

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            foreach (var s in ms.sliders)
            {
                s.Orientation = (Orientation)e.NewValue;
            }
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
    }
}
