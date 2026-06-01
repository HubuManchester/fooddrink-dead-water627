using FoodDrinkApp.Services;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel;

    /// <summary>
    /// Parameterless constructor for Shell DataTemplate instantiation.
    /// Resolves the ViewModel from the application DI container.
    /// </summary>
    public MainPage() : this(MauiProgram.Services.GetRequiredService<MainPageViewModel>())
    {
    }

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        SearchBar.TextChanged += OnSearchTextChanged;
        await _viewModel.LoadMemoriesCommand.ExecuteAsync(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SearchBar.TextChanged -= OnSearchTextChanged;
    }

    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        _viewModel.SearchQuery = e.NewTextValue;
        await _viewModel.SearchCommand.ExecuteAsync(null);
    }
}
