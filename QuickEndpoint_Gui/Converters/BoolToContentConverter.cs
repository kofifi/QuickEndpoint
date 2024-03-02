using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace QuickEndpoint_Gui.Converters
{
    public class BoolToContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Sprawdzamy, czy panel jest otwarty
            bool isPaneOpen = (bool)value;
            // Jeżeli panel jest otwarty, zwracamy "Pełna Nazwa", w przeciwnym razie zwracamy pusty ciąg
            return isPaneOpen ? "Pełna Nazwa" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
