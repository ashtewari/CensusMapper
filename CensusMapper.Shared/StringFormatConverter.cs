using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CensusMapper
{
    public class StringFormatConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string culture)
        {

            if (parameter == null)

                return null;

            return string.Format(culture, parameter as string, value);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {

            throw new NotImplementedException();

        }

    }

}
