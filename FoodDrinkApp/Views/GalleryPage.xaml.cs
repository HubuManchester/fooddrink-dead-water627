using FoodDrinkApp.Services;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class GalleryPage : ContentPage
{
    private readonly GalleryViewModel _viewModel;

    public GalleryPage() : this(MauiProgram.Services.GetRequiredService<GalleryViewModel>())
    {
    }

    public GalleryPage(GalleryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        await _viewModel.LoadGalleryCommand.ExecuteAsync(null);
    }
}
