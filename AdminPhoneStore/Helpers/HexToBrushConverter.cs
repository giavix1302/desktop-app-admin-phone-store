using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AdminPhoneStore.Helpers
{
    public class HexToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hexCode && !string.IsNullOrEmpty(hexCode))
            {
                try
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexCode));
                }
                catch
                {
                    return new SolidColorBrush(Colors.Gray);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
