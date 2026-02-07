using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AdminPhoneStore.Helpers
{
    /// <summary>
    /// Converter để chuyển boolean thành màu cho nút Primary Image
    /// </summary>
    public class BoolToPrimaryColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return new SolidColorBrush(Color.FromRgb(104, 90, 255)); // #685AFF Primary color
            }
            return new SolidColorBrush(Color.FromRgb(148, 163, 184)); // #94A3B8 Gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
