using FoodDrinkApp.Services;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

/// <summary>
/// User profile page — displays aggregate statistics about the user's
/// food diary (total entries, photos, average rating, most-visited
/// restaurant, and total calories logged) and provides a settings
/// entry point.
/// </summary>
public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;

    /// <summary>Parameterless constructor for Shell DataTemplate instantiation.</summary>
    public ProfilePage() : this(MauiProgram.Services.GetRequiredService<ProfileViewModel>())
    {
    }

    /// <summary>DI constructor — receives the ViewModel from the container.</summary>
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    /// <summary>
    /// Applies large-text accessibility scaling and loads the user
    /// profile statistics from the data service.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        await _viewModel.LoadProfileCommand.ExecuteAsync(null);
    }
}
