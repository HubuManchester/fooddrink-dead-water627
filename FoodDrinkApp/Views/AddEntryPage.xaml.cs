using FoodDrinkApp.Services;

namespace FoodDrinkApp.Views;

public partial class AddEntryPage : ContentPage
{
    /// <summary>
    /// Parameterless constructor for Shell DataTemplate / route instantiation.
    /// Resolves the ViewModel from the application DI container.
    /// </summary>
    public AddEntryPage() : this(MauiProgram.Services.GetRequiredService<ViewModels.AddEntryPageViewModel>())
    {
    }

    public AddEntryPage(ViewModels.AddEntryPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
    }
}
