using FoodDrinkApp.Services;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;

    public ProfilePage() : this(MauiProgram.Services.GetRequiredService<ProfileViewModel>())
    {
    }

    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        await _viewModel.LoadProfileCommand.ExecuteAsync(null);
    }
}
