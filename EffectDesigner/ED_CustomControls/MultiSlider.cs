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

namespace ED_CustomControls
{
    /// <summary>
    /// Represents a slider control that can have multiple slider points.
    /// </summary>
    public class MultiSlider : Control
    {

        /************************************************************************/

        #region Private Fields Vars Enums

        private List<SliderItem> sliders = new List<SliderItem>();

        SliderItem selectedSliderItem = null;
        int selectedPointIx = -1;
        int clickedIx = -1;
        bool blockOnPointColorChanged = false;

        ItemsControl display;
        Grid sliderGridDown;
        Grid sliderGridUp;
        Grid sliderArea;
        TextBlock valueLabel;

        enum Mode { Gradient, Range, Lightness};

        Mode workMode = Mode.Gradient;
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

        #region StripPoint DP


        //public PatternPoint StripPoint
        //{
        //    get { return (PatternPoint)GetValue(StripPointProperty); }
        //    set { SetValue(StripPointProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for StripPoint.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty StripPointProperty =
        //    DependencyProperty.Register("StripPoint", typeof(PatternPoint), typeof(MultiSlider), new PropertyMetadata(null, OnStripPointChanged));

        



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
            
            PatternPoint ppOld = (PatternPoint)e.OldValue;
            //PatternPoint ppNew = (PatternPoint)e.NewValue;

            MultiSlider ms = d as MultiSlider;
            if (ms.SelectedPoint != null)
            {
                ms.selectedPointIx = ms.Pattern.IndexOf(ms.SelectedPoint);

                if (ms.selectedSliderItem != null)
                {
                    if (ms.selectedSliderItem.PatternIx != ms.selectedPointIx)
                    {
                        ms.selectedSliderItem.IsSelected = false;
                        ms.selectedSliderItem = ms.sliders.FirstOrDefault(p => p.PatternIx == ms.selectedPointIx);
                        ms.selectedSliderItem.IsSelected = true;
                    }
                }
                else
                {
                    ms.selectedSliderItem = ms.sliders.FirstOrDefault(p => p.PatternIx == ms.selectedPointIx);
                    ms.selectedSliderItem.IsSelected = true;
                }

                ms.valueLabel.Text = ((int)ms.selectedSliderItem.Value).ToString();
            }
            if (ppOld != null)
                (ppOld as INotifyPropertyChanged).PropertyChanged -= ms.OnPointColorChanged;
            if (ms.SelectedPoint != null)
                (ms.SelectedPoint as INotifyPropertyChanged).PropertyChanged += ms.OnPointColorChanged;


        }

        private void OnPointColorChanged(object sender, PropertyChangedEventArgs e)
        {
            PatternPoint pp = SelectedPoint;
            if (!blockOnPointColorChanged)
            {
                if (e.PropertyName == "PointColor")
                    //if (SelectedPoint.Variant == PointVariant.Lightness)
                        UpdateModel();
            }
                //else
                //    RecalcLightnessPoint();
        }

        

        //private static void OnStripPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    MultiSlider ms = d as MultiSlider;
        //    if (e.NewValue as PatternPoint != null)
        //        ms.SelectedPoint = null;
        //}


        private static void OnStripModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = (MultiSlider)d;
            ms.UpdateModel();
        }

        private static void OnPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSlider ms = d as MultiSlider;
            //ms.Maximum = (e.NewValue as Pattern).Count
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


        private void SliderItemGotMouseCapture(object sender, MouseEventArgs e)
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
            if (( s.Variant == SliderVariant.RangeLeftLimit) || (s.Variant == SliderVariant.Gradient) || (s.Variant == SliderVariant.Lightness))
                SelectedPoint.LedPos = (int)s.Value;
            if (s.Variant == SliderVariant.RangeLeftLimit)
                SelectedPoint.LedCount = (int)(sliders[ix + 1].Value - s.Value + 1);
            if(s.Variant == SliderVariant.RangeRightLimit)
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
            display = Template.FindName("PART_Display", this) as ItemsControl;
            sliderGridDown = Template.FindName("PART_SliderGrid_Down", this) as Grid;
            sliderGridUp = Template.FindName("PART_SliderGrid_Up", this) as Grid;
            sliderArea = Template.FindName("PART_Sliders", this) as Grid;
            valueLabel = Template.FindName("PART_CurrentPosition", this) as TextBlock;
            modeBtn = Template.FindName("PART_ModeBtn", this) as Button;
            


            sliderArea.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnSliderClick);
            
            modeBtn.Click += ModeButton_Click;
            
        }
        /// <summary>
        /// Inserts the sliders into the template. 
        /// </summary>
        private void InsertSliders()
        {
            
            if ((sliderGridDown == null) || (sliderGridUp == null)) return;
            sliderGridDown.Children.Clear();
            sliderGridUp.Children.Clear();
            if (sliders.Count != 0)
            {
                for (int k = 0; k < sliders.Count; k++)
                {
                    sliders[k].GotMouseCapture -= new MouseEventHandler(SliderItemGotMouseCapture);
                    sliders[k].ValueChanged -= new RoutedPropertyChangedEventHandler<double>(OnSliderItemValueChanged);
                }
                sliders.Clear();
            }

            int i = 0;
            SliderItem s = null;

            foreach (PatternPoint pp in Pattern)
            {
                if (pp.Variant == PointVariant.Range)
                {
                    s = new SliderItem(this, i, SliderVariant.RangeLeftLimit, pp.LedPos, Pattern.IndexOf(pp));
                    sliders.Add(s);
                    i++;
                    s = new SliderItem(this, i, SliderVariant.RangeRightLimit, pp.LedPos + pp.LedCount - 1, Pattern.IndexOf(pp));
                    goto M1;
                }
                if (pp.Variant == PointVariant.Gradient)
                {
                    s = new SliderItem(this, i, SliderVariant.Gradient, pp.LedPos, Pattern.IndexOf(pp));
                    goto M1;
                }
                if (pp.Variant == PointVariant.Lightness)
                {
                    s = new SliderItem(this, i, SliderVariant.Lightness, pp.LedPos, Pattern.IndexOf(pp));
                    goto M1;
                }
                M1:
                sliders.Add(s);
                i++;
            }

            for (int k = 0; k < sliders.Count; k++)
            {
                sliders[k].Maximum = Maximum;
                sliders[k].GotMouseCapture += new MouseEventHandler(SliderItemGotMouseCapture);
                sliders[k].ValueChanged += new RoutedPropertyChangedEventHandler<double>(OnSliderItemValueChanged);
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            PatternPoint pp;
            base.OnKeyDown(e);
            if (e.Key == Key.Delete)
            {
                if (SelectedPoint != null)
                {
                    pp = Pattern.FirstOrDefault(p => p.LedPos == SelectedPoint.LedPos);
                    Pattern.Remove(pp);
                    if (selectedSliderItem != null)
                        selectedSliderItem = null;
                    InsertSliders();
                    UpdateModel();
                    SelectedPoint = Pattern[0];
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            PatternPoint pp = SelectedPoint;
            base.OnMouseWheel(e);
            //Color color;
            if (SelectedPoint != null)
            {
                int bri = (int)Math.Round(pp.HslColor.Lightness * 100.0, 0);
                if (e.Delta > 0)
                {
                    if (bri + 1 > 100)
                    {
                        //hsb = color.HsbColor();

                        pp.HslColor = new HslColor(pp.HslColor.Hue, pp.HslColor.Saturation, 1.0);
                        //pp.PointBrightness = 1.0;
                        //SelectedPoint.PointBrightness = 1.0;
                        //SelectedPoint.HSB.Brightness = 1;
                        //hsb = new HSBcolor(SelectedPoint.HSB.Hue, SelectedPoint.HSB.Saturation, 1);
                        //pp.PointColor = hsb.RgbColor;
                        //SelectedPoint.PointColor = hsb.RgbColor;
                    }
                    else
                    {
                        pp.HslColor = new HslColor(pp.HslColor.Hue, pp.HslColor.Saturation, (bri + 1.0) / 100.0);
                        //pp.PointBrightness = hsb.Brightness;
                        //SelectedPoint.PointBrightness = hsb.Brightness;
                        //hsb = new HSBcolor(SelectedPoint.HSB.Hue, SelectedPoint.HSB.Saturation, (bri + 5.0) / 100.0);
                        //pp.PointColor = hsb.RgbColor;
                        //SelectedPoint.PointColor = hsb.RgbColor;
                    }
                }
                else
                {
                    if (bri - 1 < 0)
                    {
                        pp.HslColor = new HslColor(pp.HslColor.Hue, pp.HslColor.Saturation, 0.01);
                        //hsb.Brightness = 0.01;
                        ////pp.PointColor = hsb.RgbColor;
                        //SelectedPoint.PointColor = hsb.RgbColor;
                    }
                    else
                    {
                        //hsb.Brightness = (bri - 1.0) / 100.0;
                        pp.HslColor = new HslColor(pp.HslColor.Hue, pp.HslColor.Saturation, (bri - 1.0) / 100.0);
                        //pp.PointColor = hsb.RgbColor;
                        //SelectedPoint.PointColor = hsb.RgbColor;
                    }
                }
                UpdateModel();
            }
        }

        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (workMode == Mode.Gradient)
            {
                workMode = Mode.Range;
                modeBtn.Content = "Range";
                return;
            }
            if (workMode == Mode.Range)
            {
                workMode = Mode.Lightness;
                modeBtn.Content = "Lightness";
                return;
            }
            if (workMode == Mode.Lightness)
            {
                workMode = Mode.Gradient;
                modeBtn.Content = "Point";
                return;
            }
        }

        private void OnSliderClick(object sender, MouseButtonEventArgs e)
        {
            PatternPoint pp1 = null;
            if (e.ClickCount == 2)      // DoubleClick
            {
                System.Windows.Point pt = e.GetPosition((UIElement)sender);
                //Console.WriteLine("X: {0} Y: {1}", pt.X, pt.Y);
                //Console.WriteLine("ActualWidth: {0}", sliderArea.ActualWidth);
                int ledPos = (int)(pt.X * Maximum / sliderArea.ActualWidth);
                if (ledPos < 1)
                    ledPos = 1;
                //Console.WriteLine("LedPos: {0}", ledPos);
                //PatternPoint leftPoint = null;
                //PatternPoint rightPoint = null;

                //foreach (PatternPoint pp in Pattern)
                //{
                //    if (ledPos > pp.LedPos)
                //        leftPoint = pp;
                //}

                //for (int i = Pattern.Count - 1; i >= 0; i--)
                //{
                //    if (ledPos < Pattern[i].LedPos)
                //        rightPoint = Pattern[i];
                //}

                // если первая точка в Pattern
                if (ledPos < sliders[0].Value)
                {
                    switch (workMode)
                    {
                        case Mode.Range:
                            if ((sliders[0].Value - ledPos) >= 2)
                            {
                                pp1 = new PatternPoint(Pattern[0].PointColor, ledPos) { LedCount = 2, Variant = PointVariant.Range };
                            }
                            else
                                return;
                            break;
                        case Mode.Lightness:
                            return;
                        default:
                            pp1 = new PatternPoint(Pattern[0].PointColor, ledPos) { LedCount = 1 };
                            break;
                    }
                    
                    Pattern.Insert(0, pp1);
                    if (selectedSliderItem != null)
                        selectedSliderItem = null;
                    InsertSliders();
                    UpdateModel();
                    SelectedPoint = pp1;
                    return;
                }

                // если последняя точка в Pattern
                if (ledPos > sliders[sliders.Count - 1].Value)
                {
                    switch (workMode)
                    {
                        case Mode.Range:
                            if ((Maximum - ledPos) >= 2)
                            {
                                pp1 = new PatternPoint(Pattern[Pattern.Count - 1].PointColor, ledPos) { LedCount = 2, Variant = PointVariant.Range };
                            }
                            else
                                return;
                            break;
                        case Mode.Lightness:
                            return;
                        default:
                            pp1 = new PatternPoint(Pattern[Pattern.Count - 1].PointColor, ledPos) { LedCount = 1 };
                            break;
                    }
                    Pattern.Add(pp1);
                    if (selectedSliderItem != null)
                        selectedSliderItem = null;
                    InsertSliders();
                    UpdateModel();
                    SelectedPoint = pp1;
                    return;
                }

                // между PatternPoint
                PatternPoint before;
                PatternPoint after;
                for ( int i = 0; i < sliders.Count - 1; i++)
                {
                    if (ledPos > sliders[i].Value && ledPos < sliders[i + 1].Value)
                    {
                        if ((sliders[i].Variant == SliderVariant.RangeLeftLimit) &&
                            (sliders[i + 1].Variant == SliderVariant.RangeRightLimit))
                            // внутри Range не может быть точек
                            return;

                        before = Pattern[sliders[i].PatternIx];
                        after = Pattern[sliders[i + 1].PatternIx];
                        switch (workMode)
                        {
                            case Mode.Range:
                                if ((sliders[i + 1].Value - ledPos) > 2)
                                {
                                    //pp1 = new PatternPoint(before.PointColor, ledPos) { LedCount = 2, Variant = PointVariant.Range };
                                    pp1 = new PatternPoint(StripModel[ledPos - 1].PointColor, ledPos) { LedCount = 2, Variant = PointVariant.Range };
                                }
                                else
                                    return;
                                break;
                            case Mode.Lightness:
                                pp1 = new PatternPoint(StripModel[ledPos - 1].PointColor, ledPos) { LedCount = 1, Variant = PointVariant.Lightness };
                                break;
                            default:
                                pp1 = new PatternPoint(StripModel[ledPos - 1].PointColor, ledPos) { LedCount = 1 };
                                break;
                        }
                        Pattern.Insert(Pattern.IndexOf(after), pp1);
                        if (selectedSliderItem != null)
                            selectedSliderItem = null;
                        InsertSliders();
                        UpdateModel();
                        SelectedPoint = pp1;
                        return;
                    }
                }
                

                // между точками
                //Color c = (display.Items[ledPos] as PatternPoint).PointColor;
                //int ix = Pattern.IndexOf(rightPoint);
                //pp1 = new PatternPoint(c, ledPos) { LedCount = 1 };
                //Pattern.Insert(ix, pp1);

                //InsertSliders();
                //SelectedPoint = pp1;
            }
        }


        #endregion

        /************************************************************************/

        #region Main Methods

        private void UpdateModel()
        {
            PatternPoint previousPoint = null;
            bool haveLightnessCurve = false;
            //PatternPoint tmpPnt = null;
            //PatternPoint to = null;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < StripModel.Count; i++)
                StripModel[i].PointColor = Color.Black;

            foreach (PatternPoint pp in Pattern)
            {
                if (previousPoint != null)
                {
                    //if (pp.Variant != PointVariant.Range)
                    //{
                    if (pp.Variant == PointVariant.Lightness)
                        haveLightnessCurve = true;
                    else
                    {
                        if (haveLightnessCurve)
                            UpdateGradient(previousPoint, pp, GetLightnessCurve(previousPoint, pp));
                        else
                            MakeGradient(previousPoint, pp);
                        haveLightnessCurve = false;
                        previousPoint = pp;
                    }
                    //}
                    //else
                    //{
                    //    // диапазон
                    //    MakeGradient(previousPoint, pp);
                    //    for (int i = 0; i < pp.LedCount; i++)
                    //        StripModel[pp.LedPos + i].PointColor = pp.PointColor;
                    //    previousPoint = new PatternPoint(pp.PointColor, pp.LedPos + pp.LedCount - 1);
                    //}
                }
                else
                    previousPoint = pp;
            }
            //foreach (SliderItem si in sliders)
            //{
            //    if (previousPoint != null)
            //    {

            //    }
            //    else
            //    {
            //        if (si.Variant == SliderVariant.RangeLeftLimit)
            //        {
            //            tmpPnt = Pattern[si.PatternIx];
            //            previousPoint = new PatternPoint(tmpPnt.PointColor, tmpPnt.LedPos + tmpPnt.LedCount - 1);
            //        }
            //    }
            //}

            watch.Stop();
        }

        private void MakeGradient(PatternPoint from, PatternPoint to)
        {
            HslColor hsl;
            int stepCount = 0;
            PatternPoint tmpFrom = null;

            if (to.Variant == PointVariant.Range)
                for (int i = 0; i < to.LedCount; i++)
                    StripModel[to.LedPos + i - 1].PointColor = to.PointColor;

            if (from.Variant == PointVariant.Range)
            {
                for (int i = 0; i < from.LedCount; i++)
                    StripModel[from.LedPos + i - 1].PointColor = from.PointColor;
                tmpFrom = new PatternPoint(from.PointColor, from.LedPos + from.LedCount - 1);
            }
            else
                tmpFrom = from;

            stepCount = to.LedPos - tmpFrom.LedPos;
            double deltaHue = (to.HslColor.Hue - tmpFrom.HslColor.Hue) / stepCount;
            double deltaSat = (to.HslColor.Saturation - tmpFrom.HslColor.Saturation) / stepCount;
            double deltaBri = (to.HslColor.Lightness - tmpFrom.HslColor.Lightness) / stepCount;

            StripModel[tmpFrom.LedPos - 1].PointColor = tmpFrom.PointColor;
            //Console.WriteLine("Pos: {0}  Hue: {1},  Lightness: {2}", from.LedPos, from.HslColor.Hue, from.HslColor.Lightness);
            StripModel[to.LedPos - 1].PointColor = to.PointColor;

            for (int i = 1; i < stepCount; i++)
            {
                hsl = new HslColor(from.HslColor.Hue + i * deltaHue,
                                   from.HslColor.Saturation + i * deltaSat,
                                   from.HslColor.Lightness + i * deltaBri);
                StripModel[tmpFrom.LedPos + i - 1].PointColor = hsl.ToRGB();
                //Console.WriteLine("Pos: {0}  Hue: {1},  Lightness: {2}", from.LedPos + i, hsl.Hue, hsl.Lightness);
            }
            //Console.WriteLine("Pos: {0}  Hue: {1},  Lightness: {2}", to.LedPos, to.HslColor.Hue, to.HslColor.Lightness);
        }

        private List<PatternPoint> GetLightnessCurve(PatternPoint from, PatternPoint to)
        {
            List<PatternPoint> lightnessCurve = null;
            int fromIx = Pattern.IndexOf(from);
            int toIx = Pattern.IndexOf(to);
            int stepCount = 0;
            int baseLedPos;
            HslColor hsl;

            double deltaHue = 0.0;
            double deltaSat = 0.0;

            if ((toIx - fromIx) != 0)       // имеются яркостные точки
            {
                lightnessCurve = new List<PatternPoint>();
                for (int i = fromIx + 1; i < toIx; i++)
                    lightnessCurve.Add(Pattern[i]);


                if (from.Variant == PointVariant.Range)
                    baseLedPos = (from.LedPos + from.LedCount - 1);
                else
                    baseLedPos = from.LedPos;

                stepCount = to.LedPos - baseLedPos;

                deltaHue = (to.HslColor.Hue - from.HslColor.Hue) / stepCount;
                deltaSat = (to.HslColor.Saturation - from.HslColor.Saturation) / stepCount;

                blockOnPointColorChanged = true;
                foreach (PatternPoint pp in lightnessCurve)
                {
                    hsl = new HslColor(from.HslColor.Hue + (pp.LedPos - baseLedPos) * deltaHue,
                                       from.HslColor.Saturation + (pp.LedPos - baseLedPos) * deltaSat,
                                       pp.HslColor.Lightness);
                    pp.HslColor = hsl;
                }
                blockOnPointColorChanged = false;
            }
            return lightnessCurve;
        }

        private void UpdateGradient(PatternPoint from, PatternPoint to, List<PatternPoint> lightnessCurve)
        {
            PatternPoint tmpFrom;
            tmpFrom = from;
            foreach (PatternPoint pp in lightnessCurve)
            {
                MakeGradient(tmpFrom, pp);
                tmpFrom = pp;
            }
            MakeGradient(tmpFrom, to);
        }

        //private void RecalcLightnessPoint()
        //{
        //    PatternPoint leftGradientPoint = null;
        //    PatternPoint rightGradientPoint = null;
        //    //PatternPoint pp;
        //    List<PatternPoint> leftLightCurve = new List<PatternPoint>();
        //    List<PatternPoint> rightLightCurve = new List<PatternPoint>();

        //    double deltaHue = 0.0;
        //    double deltaSaturation = 0.0;
        //    double deltaLightness = 0.0;
        //    int stepCount = 0;
        //    int baseLedPos = 0;

        //    HslColor hsl;

        //    if (selectedPointIx != 0)
        //    {
        //        // определяем GradientPoint или RangePoint слева
        //        // попутно формируем List<PatternPoint> для возможных LightnessPoint
        //        for (int i = selectedPointIx - 1; i >= 0; i--)
        //        {
        //            if (Pattern[i].Variant == PointVariant.Gradient || Pattern[i].Variant == PointVariant.Range)
        //            {
        //                leftGradientPoint = Pattern[i];
        //                break;
        //            }
        //            else
        //                leftLightCurve.Add(Pattern[i]);
        //        }


        //        // теперь справа
        //        for (int i = selectedPointIx + 1; i <= Pattern.Count; i++)
        //        {
        //            if (Pattern[i].Variant == PointVariant.Gradient || Pattern[i].Variant == PointVariant.Range)
        //            {
        //                rightGradientPoint = Pattern[i];
        //                break;
        //            }
        //            else
        //                rightLightCurve.Add(Pattern[i]);
        //        }
        //        // корректируем левую часть
        //        if (leftGradientPoint != null)
        //        {
        //            if (leftGradientPoint.Variant == PointVariant.Range)
        //            {
        //                baseLedPos = leftGradientPoint.LedPos + leftGradientPoint.LedCount - 1;
        //                stepCount = SelectedPoint.LedPos - baseLedPos;
        //            }
        //            else
        //            {
        //                baseLedPos = leftGradientPoint.LedPos;
        //                stepCount = SelectedPoint.LedPos - baseLedPos;
        //            }

        //            deltaHue = (SelectedPoint.HslColor.Hue - leftGradientPoint.HslColor.Hue) / stepCount;
        //            deltaSaturation = (SelectedPoint.HslColor.Saturation - leftGradientPoint.HslColor.Saturation) / stepCount;

        //            foreach (PatternPoint pp in leftLightCurve)
        //            {
        //                hsl = new HslColor(leftGradientPoint.HslColor.Hue + (pp.LedPos - baseLedPos) * deltaHue,
        //                                   leftGradientPoint.HslColor.Saturation + (pp.LedPos - baseLedPos) * deltaSaturation,
        //                                   pp.HslColor.Lightness);
        //                pp.HslColor = hsl;
        //            }
        //        }
        //    }
        //}

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

        public event EventHandler SendPacket;

    }
}
