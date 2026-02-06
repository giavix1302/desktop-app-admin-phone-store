using System;
using System.Globalization;
using System.Windows.Data;

namespace AdminPhoneStore.Helpers
{
    public class ProductToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? "Sửa sản phẩm" : "Thêm sản phẩm mới";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
