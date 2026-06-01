using FoodDrinkApp.Services;

namespace FoodDrinkApp.Views;

/// <summary>
/// Add-food-memory form page.  Provides text entry for dish name,
/// restaurant, review, and nutrition facts; camera and GPS hardware
/// buttons; an AI computer-vision label suggestion; and a star-rating
/// selector.  On save the entry is POSTed to the cloud via
/// <see cref="Services.FoodLogService"/>.
/// </summary>
public partial class AddEntryPage : ContentPage
{
    /// <summary>
    /// Parameterless constructor for Shell DataTemplate / route instantiation.
    /// Resolves the ViewModel from the application DI container.
    /// </summary>
    public AddEntryPage() : this(MauiProgram.Services.GetRequiredService<ViewModels.AddEntryPageViewModel>())
    {
    }

    /// <summary>DI constructor — receives the ViewModel from the container.</summary>
    public AddEntryPage(ViewModels.AddEntryPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    /// <summary>Applies large-text accessibility scaling on page appearance.</summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
    }
}
