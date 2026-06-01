using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class StatisticsPage : ContentPage
{
    private readonly StatisticsViewModel _viewModel;

    /// <summary>
    /// Parameterless constructor for Shell DataTemplate.
    /// </summary>
    public StatisticsPage() : this(MauiProgram.Services.GetRequiredService<StatisticsViewModel>())
    {
    }

    public StatisticsPage(StatisticsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadStatisticsCommand.ExecuteAsync(null);
    }
}
