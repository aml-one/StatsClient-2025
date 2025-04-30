using System.Globalization;
using System.Windows.Data;

namespace StatsClient.MVVM.Converters;

public class Double8ToMultiplyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double val)
        {
            return val * 10;
        }

        return 10;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
