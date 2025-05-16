using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StatsClient.MVVM.Converters;

public class IncostistencyToColorConverterConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (!string.IsNullOrEmpty(values[0] as string) && !string.IsNullOrEmpty(values[1] as string))
            return (SolidColorBrush)new BrushConverter().ConvertFrom("#6de392")!;

        if (values[0] as string == "")
            return (SolidColorBrush)new BrushConverter().ConvertFrom("#ffaacc")!;
        
        if (values[1] as string == "")
            return (SolidColorBrush)new BrushConverter().ConvertFrom("#e3bc6d")!;
        

        return Brushes.White;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [];
    }
}
