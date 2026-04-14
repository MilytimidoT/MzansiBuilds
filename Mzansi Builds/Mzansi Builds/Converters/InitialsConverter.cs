using System;
using System.Globalization;
using System.Windows.Data;

namespace Mzansi_Builds.Converters
{
    public class InitialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = value as string;
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpperInvariant();

            var first = parts[0].Substring(0, 1).ToUpperInvariant();
            var last = parts[parts.Length - 1].Substring(0, 1).ToUpperInvariant();
            return first + last;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
