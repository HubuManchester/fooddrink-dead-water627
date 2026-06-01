using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Services;
using FoodDrinkApp.Models;
using FoodDrinkApp.Views;

namespace FoodDrinkApp.ViewModels;

public partial class StatisticsViewModel : BaseViewModel
{
    // ── Donut chart data ────────────────────────────

    [ObservableProperty]
    private DonutChartDrawable _macroDonut = new();

    [ObservableProperty]
    private string _macroSummaryText = string.Empty;

    // ── Bar chart data ──────────────────────────────

    [ObservableProperty]
    private BarChartDrawable _categoryBarChart = new();

    [ObservableProperty]
    private string _categorySummaryText = string.Empty;

    // ── Totals ──────────────────────────────────────

    [ObservableProperty]
    private int _totalProtein;

    [ObservableProperty]
    private int _totalCarbs;

    [ObservableProperty]
    private int _totalFat;

    [ObservableProperty]
    private int _totalCalories;

    [ObservableProperty]
    private int _entryCount;

    public StatisticsViewModel()
    {
        Title = "Statistics";
    }

    [RelayCommand]
    private async Task LoadStatisticsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var items = await FoodLogService.SearchAsync(null);

            EntryCount = items.Count;
            TotalProtein = items.Sum(i => i.Protein);
            TotalCarbs   = items.Sum(i => i.Carbs);
            TotalFat     = items.Sum(i => i.Fat);
            TotalCalories = items.Sum(i => i.Calories);

            // ── Donut: macronutrient grams ───────────
            MacroDonut = new DonutChartDrawable
            {
                Protein = TotalProtein,
                Carbs   = TotalCarbs,
                Fat     = TotalFat
            };

            MacroSummaryText = TotalProtein + TotalCarbs + TotalFat > 0
                ? $"{TotalProtein}g protein  ·  {TotalCarbs}g carbs  ·  {TotalFat}g fat"
                : "Add nutrition data (calories, protein, carbs, fat) to your food memories to see the macro breakdown here.";

            // ── Bar: average calories by rating ──────
            var bars = new List<(string, float, Color)>();
            var warmColors = new[]
            {
                Color.FromArgb("#FFCDD2"), // 1⭐ light red
                Color.FromArgb("#FFCC80"), // 2⭐ light orange
                Color.FromArgb("#FFF9C4"), // 3⭐ light yellow
                Color.FromArgb("#C8E6C9"), // 4⭐ light green
                Color.FromArgb("#BBDEFB"), // 5⭐ light blue
            };

            for (int r = 1; r <= 5; r++)
            {
                var group = items.Where(i => i.Rating == r).ToList();
                float avg = group.Count > 0 ? (float)group.Average(i => i.Calories) : 0;
                bars.Add(($"{r}⭐  ({group.Count})", avg, warmColors[r - 1]));
            }

            CategoryBarChart = new BarChartDrawable { Bars = bars };

            CategorySummaryText = TotalCalories > 0
                ? $"Average calories shown across {EntryCount} entries ({TotalCalories} kcal total)"
                : "Log calories in your food memories to see average intake by star rating.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
