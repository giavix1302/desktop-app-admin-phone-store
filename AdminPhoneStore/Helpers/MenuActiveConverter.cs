using System;
using System.Globalization;
using System.Windows.Data;

namespace AdminPhoneStore.Helpers
{
    /// <summary>
    /// Converter để check xem menu item có active không
    /// </summary>
    public class MenuActiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string activeMenu = value.ToString() ?? string.Empty;
            string menuTag = parameter.ToString() ?? string.Empty;

            return activeMenu.Equals(menuTag, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
