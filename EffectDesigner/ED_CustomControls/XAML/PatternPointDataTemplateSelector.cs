﻿using System;
using System.Windows;
using System.Windows.Controls;
using Lighting.Library;

namespace ED_CustomControls
{
    public class PatternPointDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement elemnt = container as FrameworkElement;
            PatternPoint pp = item as PatternPoint;
            var myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source = new Uri("/ED_CustomControls;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            if (pp.Variant ==0)
                return myResourceDictionary["gradientPoint"] as DataTemplate;
            if (pp.Variant == 1)
                return myResourceDictionary["rangePoint"] as DataTemplate;
            if (pp.Variant == 2)
                return myResourceDictionary["lightnessPoint"] as DataTemplate;
            return null;
            //if (pp.LedCount == 1)
            //{
            //    return myResourceDictionary["gradientPoint"] as DataTemplate;
            //}
            //else
            //{
            //    return myResourceDictionary["rangePoint"] as DataTemplate;
            //}
        }
    }

}
