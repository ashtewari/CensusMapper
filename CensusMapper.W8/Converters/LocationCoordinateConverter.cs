using System;
using Bing.Maps;
using CensusMapper.ViewModels;
using Windows.UI.Xaml.Data;

namespace CensusMapper.Converters
{
    public class LocationCoordinateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Coordinates) return value;

            var location = value as Location;
            if (location == null) return null;

            return new Coordinates(location.Latitude, location.Longitude);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Location) return value;

            var location = value as Coordinates;
            if (location == null) return null;

            return new Location(location.Latitude, location.Longitude);
        }
    }
}
