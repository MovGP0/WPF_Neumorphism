using System.Globalization;
using System.Windows.Data;

namespace Neumorphism.WPF.Theming;

public sealed class RatingFillConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
        values is [int value, int selectedIndex] && value <= selectedIndex + 1;

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
