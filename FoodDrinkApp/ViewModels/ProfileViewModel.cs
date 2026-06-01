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

            TotalMemories = items.Count;
            TotalPhotos = items.Count(i => !string.IsNullOrWhiteSpace(i.ImagePath));
            TotalCalories = items.Sum(i => i.Calories);
            AverageRating = items.Count > 0
                ? Math.Round(items.Average(i => i.Rating), 1)
                : 0;

            var top = items
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
        await Shell.Current.DisplayAlert(
            "Settings",
            "App version: 1.0\nTheme: Auto\nAccessibility: Large text, screen reader support, WCAG-aligned.",
            "OK");
    }

    public ProfileViewModel()
    {
        Title = "Profile";
    }
}
