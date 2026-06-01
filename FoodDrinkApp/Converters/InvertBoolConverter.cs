using System.Globalization;

namespace FoodDrinkApp.Converters;

/// <summary>
/// Inverts a boolean value.  Used for visibility bindings where
/// one view is shown when a condition is false (e.g. empty state).
/// </summary>
public sealed class InvertBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;
        return value;
    }
}
