
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Mzansi_Builds.Services;

namespace Mzansi_Builds.Converters
{
    // show value when count > 0 (used for "No posts yet" visibility)
    public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i && i > 0) return Visibility.Collapsed;
            return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }

    public class EmailToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var email = value as string;
            if (string.IsNullOrWhiteSpace(email)) return string.Empty;
            var user = DataService.Instance.GetUserByEmail(email);
            return user?.Name ?? email;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }

    public class EmailToBioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var email = value as string;
            if (string.IsNullOrWhiteSpace(email)) return string.Empty;
            var user = DataService.Instance.GetUserByEmail(email);
            return user?.Bio ?? string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }

    public class EmailToCityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var email = value as string;
            if (string.IsNullOrWhiteSpace(email)) return string.Empty;
            var user = DataService.Instance.GetUserByEmail(email);
            return user?.City ?? string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
