using FoodDrinkApp.Services;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

/// <summary>
/// Main food-memory list page.  Displays a searchable, swipe-to-delete
/// card list backed by the cloud (mockapi.io).  When the list is empty
/// a friendly onboarding empty state is shown.
///
/// The page uses a Grid overlay pattern so the <c>RefreshView</c> and
/// the empty-state panel share the same row — only one is visible at
/// a time, avoiding the XAML "multiple children" layout trap.
/// </summary>
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

    /// <summary>DI constructor — receives the ViewModel from the container.</summary>
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    /// <summary>
    /// Applies large-text accessibility scaling, wires up the search
    /// event handler, and triggers the initial data load from the cloud.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        SearchBar.TextChanged += OnSearchTextChanged;
        await _viewModel.LoadMemoriesCommand.ExecuteAsync(null);
    }

    /// <summary>Unsubscribes the search handler to avoid leaks.</summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SearchBar.TextChanged -= OnSearchTextChanged;
    }

    /// <summary>
    /// Forwards keystrokes to the ViewModel which performs debounced
    /// (300 ms) search queries against the data service.
    /// </summary>
    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        _viewModel.SearchQuery = e.NewTextValue;
        await _viewModel.SearchCommand.ExecuteAsync(null);
    }
}
