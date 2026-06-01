using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

public partial class AddEntryPageViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _foodName = string.Empty;

    [ObservableProperty]
    private string _restaurantName = string.Empty;

    [ObservableProperty]
    private string _review = string.Empty;

    [ObservableProperty]
    private string _location = string.Empty;

    [ObservableProperty]
    private DateTime _date = DateTime.Today;

    [ObservableProperty]
    private int _rating;

    [ObservableProperty]
    private string _validationMessage = string.Empty;

    [ObservableProperty]
    private bool _hasValidationError;

    public string RatingLabel => Rating > 0 ? new string('⭐', Rating) : "Tap a star to rate";

    public AddEntryPageViewModel()
    {
        Title = "Add Food Memory";
    }

    [RelayCommand]
    private void SetRating(string ratingStr)
    {
        if (int.TryParse(ratingStr, out var value) && value is >= 1 and <= 5)
        {
            Rating = value;
            OnPropertyChanged(nameof(RatingLabel));
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            var error = ValidateForm();
            if (error is not null)
            {
                ShowValidation(error);
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
                return;
            }

            var item = new FoodModel
            {
                Name = FoodName.Trim(),
                RestaurantName = RestaurantName.Trim(),
                Review = Review.Trim(),
                Location = Location.Trim(),
                Date = Date,
                Rating = Rating,
                Tags = $"{FoodName} {RestaurantName}"
            };

            await FoodLogService.AddAsync(item);
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            SemanticScreenReader.Announce($"Food memory \"{item.Name}\" saved.");

            await Shell.Current.DisplayAlert("Saved", "Your food memory has been saved.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ShowValidation($"Could not save: {ex.Message}");
        }
    }

    private string? ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(FoodName))
            return "Please enter the food or drink name.";

        if (string.IsNullOrWhiteSpace(RestaurantName))
            return "Please enter the restaurant or place name.";

        if (string.IsNullOrWhiteSpace(Review))
            return "Please write a short review.";

        if (Date > DateTime.Today)
            return "Date cannot be in the future.";

        return null;
    }

    private void ShowValidation(string message)
    {
        ValidationMessage = message;
        HasValidationError = true;
        SemanticScreenReader.Announce(message);
    }
}
