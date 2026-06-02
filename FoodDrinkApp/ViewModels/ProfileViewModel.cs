using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    [ObservableProperty]
    private int _totalMemories;

    [ObservableProperty]
    private int _totalPhotos;

    [ObservableProperty]
    private int _totalCalories;

    [ObservableProperty]
    private double _averageRating;

    [ObservableProperty]
    private int _topRestaurantsCount;

    [ObservableProperty]
    private string _mostVisitedRestaurant = "—";

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var items = await FoodLogService.SearchAsync(null);

            // Filter today's entries for daily-calorie tracking
            var today = DateTime.Today;
            var todayItems = items
                .Where(i => i.Date.Date == today)
                .ToList();

            TotalMemories = todayItems.Count;
            TotalPhotos = todayItems.Count(i => !string.IsNullOrWhiteSpace(i.ImagePath));
            TotalCalories = todayItems.Sum(i => i.Calories);
            AverageRating = todayItems.Count > 0
                ? Math.Round(todayItems.Average(i => i.Rating), 1)
                : 0;

            var top = todayItems
                .GroupBy(i => i.RestaurantName)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();
            MostVisitedRestaurant = top?.Key ?? "—";
            TopRestaurantsCount = top?.Count() ?? 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.SettingsPage));
    }

    public ProfileViewModel()
    {
        Title = "Profile";
    }
}
