using System;
using System.Windows;
using System.Windows.Controls;
using Lighting.Library;

namespace Xam.Wpf.Controls
{
    public class PatternPointDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement elemnt = container as FrameworkElement;
            PatternPoint pp = item as PatternPoint;
            var myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source = new Uri("/PatternControls;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            if (pp.LedCount == 0)
            {
                return myResourceDictionary["gradientPoint"] as DataTemplate;
            }
            else
            {
                return myResourceDictionary["rangePoint"] as DataTemplate;
            }
        }
    }

}
