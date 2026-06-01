using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

/// <summary>
/// Nutrition statistics dashboard — renders a donut chart
/// (macronutrient breakdown) and a bar chart (average calories by
/// star rating) using native <c>GraphicsView</c> drawables.  Also
/// displays a numeric totals card (Protein / Carbs / Fat / kcal).
/// </summary>
public partial class StatisticsPage : ContentPage
{
    private readonly StatisticsViewModel _viewModel;

    /// <summary>Parameterless constructor for Shell DataTemplate instantiation.</summary>
    public StatisticsPage() : this(MauiProgram.Services.GetRequiredService<StatisticsViewModel>())
    {
    }

    /// <summary>DI constructor — receives the ViewModel from the container.</summary>
    public StatisticsPage(StatisticsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    /// <summary>
    /// Triggers data loading; the ViewModel computes chart data and
    /// binds it to the <c>GraphicsView.Drawable</c> properties.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadStatisticsCommand.ExecuteAsync(null);
    }
}
