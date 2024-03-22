using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace QuickEndpoint.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Directly return the boolean value, true for visible, false for collapsed.
            if (value is bool b)
            {
                return b;
            }

            return false; // Default to false/collapsed if the value is not a bool.
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
