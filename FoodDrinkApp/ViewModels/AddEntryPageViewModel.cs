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

    // ── Hardware: Camera ─────────────────────────────

    /// <summary>
    /// Local file path of the captured photo.
    /// Bound to the Image.Source in the XAML preview.
    /// </summary>
    [ObservableProperty]
    private string _imagePath = string.Empty;

    [ObservableProperty]
    private bool _isTakingPhoto;

    /// <summary>
    /// True when a photo has been captured and is ready to preview.
    /// </summary>
    public bool HasPhoto => !string.IsNullOrWhiteSpace(ImagePath);

    // ── Hardware: Location ───────────────────────────

    [ObservableProperty]
    private bool _isGettingLocation;

    // ── Nutrition (optional) ──────────────────────────

    [ObservableProperty]
    private int _calories;

    [ObservableProperty]
    private int _protein;

    [ObservableProperty]
    private int _carbs;

    [ObservableProperty]
    private int _fat;

    // ── Computed ─────────────────────────────────────

    public string RatingLabel => Rating > 0 ? new string('⭐', Rating) : "Tap a star to rate";

    public AddEntryPageViewModel()
    {
        Title = "Add Food Memory";
    }

    // ──────────────────────────────────────────────
    //  Notifications
    // ──────────────────────────────────────────────

    partial void OnImagePathChanged(string value)
    {
        OnPropertyChanged(nameof(HasPhoto));
    }

    // ──────────────────────────────────────────────
    //  Rating
    // ──────────────────────────────────────────────

    [RelayCommand]
    private void SetRating(string ratingStr)
    {
        if (int.TryParse(ratingStr, out var value) && value is >= 1 and <= 5)
        {
            Rating = value;
            OnPropertyChanged(nameof(RatingLabel));
        }
    }

    // ──────────────────────────────────────────────
    //  Camera — Take Photo
    // ──────────────────────────────────────────────

    /// <summary>
    /// Requests camera permission, launches the system camera,
    /// saves the photo to the app cache directory, and sets
    /// ImagePath so the preview updates.
    /// </summary>
    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        if (IsTakingPhoto) return;

        try
        {
            IsTakingPhoto = true;

            // ── Runtime permission check ──────────────
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    ShowValidation("Camera permission is required to take a photo.");
                    return;
                }
            }

            if (!MediaPicker.Default.IsCaptureSupported)
            {
                ShowValidation("Camera is not available on this device.");
                return;
            }

            // ── Launch camera ─────────────────────────
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null) return; // user cancelled

            // ── Save to local cache ───────────────────
            var fileName = $"foodie_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);

            using var sourceStream = await photo.OpenReadAsync();
            using var fileStream = File.OpenWrite(localPath);
            await sourceStream.CopyToAsync(fileStream);

            ImagePath = localPath;
            SemanticScreenReader.Announce("Photo captured and saved.");

            // ── AI Computer Vision classification ─────
            // Open a stream from the saved file and send it to
            // ComputerVisionService for food recognition.
            try
            {
                SemanticScreenReader.Announce("Analysing photo with AI...");

                using var classifyStream = File.OpenRead(localPath);
                string aiLabel = await ComputerVisionService.ClassifyFoodImageAsync(classifyStream);

                if (!string.IsNullOrWhiteSpace(aiLabel))
                {
                    // Auto-fill the dish name field with the AI result
                    FoodName = aiLabel;

                    // Haptic confirmation so the user feels the AI result arrive
                    HapticFeedback.Default.Perform(HapticFeedbackType.Click);

                    SemanticScreenReader.Announce(
                        $"AI recognised: {aiLabel}. Dish name auto-filled.");
                }
            }
            catch (Exception aiEx)
            {
                // CV failure is non-fatal — the photo is still saved,
                // the user can type the dish name manually.
                SemanticScreenReader.Announce(
                    "AI classification unavailable. You can enter the dish name manually.");
                System.Diagnostics.Debug.WriteLine(
                    $"ComputerVisionService: {aiEx.Message}");
            }
        }
        catch (PermissionException)
        {
            ShowValidation("Camera permission was denied.");
        }
        catch (Exception ex)
        {
            ShowValidation($"Could not take photo: {ex.Message}");
        }
        finally
        {
            IsTakingPhoto = false;
        }
    }

    // ──────────────────────────────────────────────
    //  Location — Get GPS
    // ──────────────────────────────────────────────

    /// <summary>
    /// Requests location permission, reads the GPS position,
    /// reverse-geocodes it to a human-readable address,
    /// and auto-fills the Location field.
    /// </summary>
    [RelayCommand]
    private async Task GetLocationAsync()
    {
        if (IsGettingLocation) return;

        try
        {
            IsGettingLocation = true;

            // ── Runtime permission check ──────────────
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    ShowValidation("Location permission is required to get GPS position.");
                    return;
                }
            }

            // ── Read GPS ──────────────────────────────
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location is null)
            {
                ShowValidation("Could not determine location. Ensure GPS is enabled.");
                return;
            }

            // ── Reverse geocode to address ────────────
            string addressText;
            try
            {
                var placemarks = await Geocoding.Default.GetPlacemarksAsync(
                    location.Latitude, location.Longitude);

                var placemark = placemarks?.FirstOrDefault();
                if (placemark is not null)
                {
                    var parts = new List<string>();

                    if (!string.IsNullOrWhiteSpace(placemark.Locality))
                        parts.Add(placemark.Locality);
                    else if (!string.IsNullOrWhiteSpace(placemark.SubAdminArea))
                        parts.Add(placemark.SubAdminArea);

                    if (!string.IsNullOrWhiteSpace(placemark.AdminArea))
                        parts.Add(placemark.AdminArea);

                    if (!string.IsNullOrWhiteSpace(placemark.CountryName))
                        parts.Add(placemark.CountryName);

                    addressText = parts.Count > 0
                        ? string.Join(", ", parts)
                        : $"{location.Latitude:F5}, {location.Longitude:F5}";
                }
                else
                {
                    // Geocoding returned no results — use coordinates
                    addressText = $"{location.Latitude:F5}, {location.Longitude:F5}";
                }
            }
            catch
            {
                // Geocoding failed — fall back to raw coordinates
                addressText = $"{location.Latitude:F5}, {location.Longitude:F5}";
            }

            Location = addressText;
            SemanticScreenReader.Announce($"Location set to {addressText}");
        }
        catch (PermissionException)
        {
            ShowValidation("Location permission was denied.");
        }
        catch (FeatureNotSupportedException)
        {
            ShowValidation("GPS is not supported on this device.");
        }
        catch (FeatureNotEnabledException)
        {
            ShowValidation("GPS is disabled. Please enable location services.");
        }
        catch (Exception ex)
        {
            ShowValidation($"Could not get location: {ex.Message}");
        }
        finally
        {
            IsGettingLocation = false;
        }
    }

    // ──────────────────────────────────────────────
    //  Save
    // ──────────────────────────────────────────────

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
                ImagePath = ImagePath,
                Location = Location.Trim(),
                Date = Date,
                Rating = Rating,
                Calories = Calories,
                Protein = Protein,
                Carbs = Carbs,
                Fat = Fat,
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
