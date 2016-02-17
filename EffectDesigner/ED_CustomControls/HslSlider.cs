using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ED_CustomControls
{

    public class HslSlider : Slider
    {
        static HslSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HslSlider), new FrameworkPropertyMetadata(typeof(HslSlider)));
        }


        #region Dependency Properties

        public Point ScaleRange
        {
            get { return (Point)GetValue(ScaleRangeProperty); }
            set { SetValue(ScaleRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Range.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleRangeProperty =
            DependencyProperty.Register("ScaleRange", typeof(Point), typeof(HslSlider), new PropertyMetadata(new Point(0,0), OnRangeChanged));

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion


        //#region Private Fields

        Border scale;

        //#endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scale = Template.FindName("PART_Scale", this) as Border;

            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.StartPoint = new Point(0.5, 1.0);
            lgb.EndPoint = new Point(0.5, 0.0);

            lgb.GradientStops.Add(new GradientStop(Colors.Red, 0.0));
            lgb.GradientStops.Add(new GradientStop(Colors.Orange, 1.0));

            scale.Background = lgb;




            // Set Fill property of rectangle 

        }
    }
}
