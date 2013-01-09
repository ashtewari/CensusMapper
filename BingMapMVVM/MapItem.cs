namespace BingMapMVVM
{
    using Bing.Maps;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    public class MapItem : ContentControl
    {
        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            set { SetValue(LatitudeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Latitude.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(double), typeof(MapItem), new PropertyMetadata(0d, OnLatitudePropertyChanged));

        private static void OnLatitudePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((MapItem)sender).OnLatitudePropertyChanged((double)eventArgs.OldValue, (double)eventArgs.NewValue);
        }

        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            set { SetValue(LongitudeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Longitude.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.Register("Longitude", typeof(double), typeof(MapItem), new PropertyMetadata(0d, OnLongitudePropertyChanged));

        private static void OnLongitudePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((MapItem)sender).OnLongitudePropertyChanged((double)eventArgs.OldValue, (double)eventArgs.NewValue);
        }

        private void OnLatitudePropertyChanged(double oldValue, double newValue)
        {
            UpdatePosition();
        }

        private void OnLongitudePropertyChanged(double oldValue, double newValue)
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            var location = new Location(Latitude, Longitude);
            MapLayer.SetPosition(this, location);
        }

        public static T FindParent<T>(DependencyObject parent)
            where T : DependencyObject
        {
            T result = default(T);
            do
            {
                DependencyObject p = VisualTreeHelper.GetParent(parent);
                if (p == null)
                    return result;

                result = p as T;
                if (result != null)
                    return result;

                parent = p;

            } while (result == null);
            return null;
        }
        
    }
}
