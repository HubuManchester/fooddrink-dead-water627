using FoodDrinkApp.Services;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

/// <summary>
/// Advanced settings page — provides personalised controls for
/// theme, notifications, nutrition goals, unit localisation,
/// accessibility options, and data-management actions.
/// </summary>
public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    /// <summary>Parameterless constructor for Shell DataTemplate instantiation.</summary>
    public SettingsPage() : this(MauiProgram.Services.GetRequiredService<SettingsViewModel>())
    {
    }

    /// <summary>DI constructor — receives the ViewModel from the container.</summary>
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    /// <summary>
    /// Applies large-text accessibility scaling each time the page
    /// appears, ensuring the WCAG preference takes effect.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
    }
}
