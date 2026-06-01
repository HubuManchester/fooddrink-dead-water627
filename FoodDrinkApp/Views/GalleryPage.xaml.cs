using FoodDrinkApp.Services;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

/// <summary>
/// Photo gallery page — displays a 2-column masonry grid of every
/// food photo the user has captured.  Supports pull-to-refresh and
/// shows a friendly empty state when no photos exist.
/// </summary>
public partial class GalleryPage : ContentPage
{
    private readonly GalleryViewModel _viewModel;

    /// <summary>Parameterless constructor for Shell DataTemplate instantiation.</summary>
    public GalleryPage() : this(MauiProgram.Services.GetRequiredService<GalleryViewModel>())
    {
    }

    /// <summary>DI constructor — receives the ViewModel from the container.</summary>
    public GalleryPage(GalleryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    /// <summary>
    /// Applies large-text accessibility scaling and loads the photo
    /// collection from the data service.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        await _viewModel.LoadGalleryCommand.ExecuteAsync(null);
    }
}
