using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace BudgetApp.Converters;

public class DoubleWithSignConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double number)
            return number >= 0 ? $"+{number} €" : $"{number} €";
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
