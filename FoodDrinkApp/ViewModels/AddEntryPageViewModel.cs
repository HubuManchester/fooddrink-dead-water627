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

    /// <summary>
    /// Validate the form, POST to mockapi.io, then navigate back.
    ///
    /// FoodLogService.AddAsync runs JSON serialisation + HTTP on a background
    /// thread (Task.Run).  The ViewModel's await captures the UI thread's
    /// SynchronizationContext, so the continuation — IsBusy, DisplayAlert,
    /// GoToAsync — always executes on the main thread.
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        // ── Client-side validation (synchronous, fast) ──
        var error = ValidateForm();
        if (error is not null)
        {
            ShowValidation(error);
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
            return;
        }

        IsBusy = true;
        ClearValidation();

        try
        {
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

            // Offloaded to thread pool inside FoodLogService — won't block UI
            var saved = await FoodLogService.AddAsync(item);

            // We are back on the UI thread here (SynchronizationContext captured by await)
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            SemanticScreenReader.Announce($"Food memory \"{saved.Name}\" saved.");

            string confirmationMessage = MockApiConfig.IsConfigured
                ? "Your food memory has been saved to the cloud. 🍜"
                : "Your food memory has been saved locally.";

            await Shell.Current.DisplayAlert("Saved", confirmationMessage, "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            // FoodLogService.AddAsync falls back to local cache on failure,
            // so the data is safe — but the user should know.
            ShowValidation($"Saved locally — cloud unavailable.\n{ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ──────────────────────────────────────────────
    //  Validation
    // ──────────────────────────────────────────────

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

    private void ClearValidation()
    {
        ValidationMessage = string.Empty;
        HasValidationError = false;
    }
}
