using System.Globalization;

namespace FoodDrinkApp.Converters;

/// <summary>
/// Inverts a <see cref="bool"/> value for use in XAML visibility bindings.
/// When a view must be shown in the opposite state of a ViewModel property
/// (e.g. show an empty-state panel when <c>IsEmpty</c> is <see langword="true"/>,
/// but hide the populated list), bind <c>IsVisible</c> through this converter.
/// </summary>
/// <example>
/// <code>
/// &lt;ContentPage.Resources&gt;
///     &lt;conv:InvertBoolConverter x:Key="InvertBool" /&gt;
/// &lt;/ContentPage.Resources&gt;
/// ...
/// IsVisible="{Binding IsEmpty, Converter={StaticResource InvertBool}}"
/// </code>
/// </example>
public sealed class InvertBoolConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;
        return value;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;
        return value;
    }
}
