using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BingMapMVVM
{
    public class LocationAwareDataTemplate
    {


        public static string GetLongitudePath(DependencyObject obj)
        {
            return (string)obj.GetValue(LongitudePathProperty);
        }

        public static void SetLongitudePath(DependencyObject obj, string value)
        {
            obj.SetValue(LongitudePathProperty, value);
        }

        // Using a DependencyProperty as the backing store for LongitudePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LongitudePathProperty =
            DependencyProperty.RegisterAttached("LongitudePath", typeof(string), typeof(DataTemplate), new PropertyMetadata(0));




        public static string GetLatitudePath(DependencyObject obj)
        {
            return (string)obj.GetValue(LatitudePathProperty);
        }

        public static void SetLatitudePath(DependencyObject obj, string value)
        {
            obj.SetValue(LatitudePathProperty, value);
        }

        // Using a DependencyProperty as the backing store for LatitudePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LatitudePathProperty =
            DependencyProperty.RegisterAttached("LatitudePath", typeof(string), typeof(DataTemplate), new PropertyMetadata(0));

        
    }
}
