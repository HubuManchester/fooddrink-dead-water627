using System.Globalization;

namespace FoodDrinkApp.Converters;

/// <summary>
/// Divides a percentage value (0–100) by 100 to produce a 0–1 fraction
/// suitable for <see cref="ProgressBar.Progress"/>.
/// </summary>
public sealed class PercentToFractionConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
            return d / 100.0;
        if (value is int i)
            return i / 100.0;
        return 0.0;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
            return d * 100.0;
        return 0;
    }
}
