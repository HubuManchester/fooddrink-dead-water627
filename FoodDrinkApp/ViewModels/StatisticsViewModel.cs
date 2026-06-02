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

    // ── Calorie goal synergy (linked from Settings) ─

    [ObservableProperty]
    private int _dailyCalorieGoal = 2000;

    [ObservableProperty]
    private string _goalProgressText = string.Empty;

    [ObservableProperty]
    private double _goalProgressPercent;

    [ObservableProperty]
    private string _goalStatusEmoji = "🎯";

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

            // Read the user's calorie goal from Settings preferences
            DailyCalorieGoal = Preferences.Get("pref_calorie_goal", 2000);

            var items = await FoodLogService.SearchAsync(null);

            // ── Filter items to today's date for daily tracking ──
            var today = DateTime.Today;
            var todayItems = items
                .Where(i => i.Date.Date == today)
                .ToList();

            // Overall totals (used by charts and summary card)
            EntryCount    = todayItems.Count;
            TotalProtein  = todayItems.Sum(i => i.Protein);
            TotalCarbs    = todayItems.Sum(i => i.Carbs);
            TotalFat      = todayItems.Sum(i => i.Fat);
            TotalCalories = todayItems.Sum(i => i.Calories);

            // ── Goal progress calculation (today-only) ─
            GoalProgressPercent = DailyCalorieGoal > 0
                ? Math.Min(100, (double)TotalCalories / DailyCalorieGoal * 100)
                : 0;

            GoalStatusEmoji = GoalProgressPercent switch
            {
                >= 100 => "✅",
                >= 75  => "🔥",
                >= 50  => "🍽️",
                >= 25  => "🌱",
                _      => "🎯"
            };

            GoalProgressText = DailyCalorieGoal > 0
                ? $"{GoalStatusEmoji}  {TotalCalories} / {DailyCalorieGoal} kcal  ({GoalProgressPercent:F0}%)"
                : $"{TotalCalories} kcal today";

            // ── Donut: today's macronutrient grams ────
            MacroDonut = new DonutChartDrawable
            {
                Protein               = TotalProtein,
                Carbs                 = TotalCarbs,
                Fat                   = TotalFat,
                GoalCalories          = DailyCalorieGoal,
                TotalCaloriesConsumed = TotalCalories
            };

            MacroSummaryText = TotalProtein + TotalCarbs + TotalFat > 0
                ? $"{TotalProtein}g protein  ·  {TotalCarbs}g carbs  ·  {TotalFat}g fat"
                : "Add nutrition data (calories, protein, carbs, fat) to your food memories to see the macro breakdown here.";

            // ── Bar: today's average calories by rating ─
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
                var group = todayItems.Where(i => i.Rating == r).ToList();
                float avg = group.Count > 0 ? (float)group.Average(i => i.Calories) : 0;
                bars.Add(($"{r}⭐  ({group.Count})", avg, warmColors[r - 1]));
            }

            CategoryBarChart = new BarChartDrawable
            {
                Bars         = bars,
                GoalCalories = DailyCalorieGoal
            };

            CategorySummaryText = TotalCalories > 0
                ? $"Today's average calories shown across {EntryCount} entries ({TotalCalories} kcal total)"
                : "Log calories in your food memories to see today's intake by star rating.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
