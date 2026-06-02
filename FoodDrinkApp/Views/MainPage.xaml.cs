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
///
/// Also includes a Random Meal Picker: tap the banner button or shake
/// the device to get a random food suggestion from the diary.
/// </summary>
public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel;

    // ── Accelerometer shake detection ──────────────────
    private const double ShakeThresholdG = 1.2;
    private DateTime _lastShakeUtc = DateTime.MinValue;
    private static readonly TimeSpan ShakeCooldown = TimeSpan.FromSeconds(2);
    private double _prevX, _prevY, _prevZ;
    private bool _sensorsStarted;

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
    /// event handler, triggers the initial data load from the cloud,
    /// starts listening for accelerometer shake gestures, and monitors
    /// network connectivity to show an offline banner when needed.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        SearchBar.TextChanged += OnSearchTextChanged;
        await _viewModel.LoadMemoriesCommand.ExecuteAsync(null);
        StartSensors();
        StartConnectivityMonitor();
    }

    /// <summary>Unsubscribes the search handler, stops sensors, and
    /// tears down connectivity monitoring to avoid leaks.</summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SearchBar.TextChanged -= OnSearchTextChanged;
        StopSensors();
        StopConnectivityMonitor();
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

    // ──────────────────────────────────────────────
    //  Random Meal Picker
    // ──────────────────────────────────────────────

    /// <summary>
    /// Triggered by the "🎲 Tap or Shake" button on screen.
    /// Provides haptic confirmation, then picks a random food memory.
    /// </summary>
    private async void OnRandomPickClicked(object? sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        await PickRandomMealAsync();
    }

    /// <summary>
    /// Selects a random item from the current <see cref="MainPageViewModel.FoodMemories"/>
    /// collection and shows a DisplayAlert with the dish name and restaurant.
    /// Shows a friendly prompt if the diary is still empty.
    /// </summary>
    private async Task PickRandomMealAsync()
    {
        var items = _viewModel.FoodMemories;
        if (items is null || items.Count == 0)
        {
            await DisplayAlert(
                "No Meals Yet",
                "Add some food memories first, then try a random pick! 🍜",
                "OK");
            return;
        }

        var picked = items[Random.Shared.Next(items.Count)];

        await DisplayAlert(
            "🎲 Random Pick!",
            $"🍽️  {picked.Name}\n📍  {picked.RestaurantName}",
            "Yum!");
    }

    // ──────────────────────────────────────────────
    //  Accelerometer shake detection
    // ──────────────────────────────────────────────

    /// <summary>Starts the accelerometer and subscribes to readings.</summary>
    private void StartSensors()
    {
        if (_sensorsStarted) return;

        try
        {
            Accelerometer.Default.ReadingChanged += OnAccelerometerReadingChanged;
            Accelerometer.Default.Start(SensorSpeed.UI);
            _sensorsStarted = true;
        }
        catch (FeatureNotSupportedException)
        {
            // Accelerometer not available — shake won't work, button still works.
        }
        catch (Exception)
        {
            // Sensor already started or permission denied — non-fatal.
        }
    }

    /// <summary>Stops the accelerometer and unsubscribes to avoid battery drain.</summary>
    private void StopSensors()
    {
        if (!_sensorsStarted) return;

        try
        {
            Accelerometer.Default.Stop();
            Accelerometer.Default.ReadingChanged -= OnAccelerometerReadingChanged;
            _sensorsStarted = false;
        }
        catch (Exception)
        {
            // Best-effort cleanup; ignore if already stopped.
        }
    }

    /// <summary>
    /// Fires on every accelerometer reading.  Computes the delta magnitude
    /// from the previous sample; when it exceeds <see cref="ShakeThresholdG"/>
    /// and the cooldown period has passed, triggers a random meal pick on the
    /// main thread with haptic feedback.
    /// </summary>
    private void OnAccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        var a = e.Reading.Acceleration;

        double dx = a.X - _prevX;
        double dy = a.Y - _prevY;
        double dz = a.Z - _prevZ;

        _prevX = a.X;
        _prevY = a.Y;
        _prevZ = a.Z;

        double magnitude = Math.Sqrt(dx * dx + dy * dy + dz * dz);
        if (magnitude < ShakeThresholdG)
            return;

        var now = DateTime.UtcNow;
        if (now - _lastShakeUtc < ShakeCooldown)
            return;
        _lastShakeUtc = now;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await PickRandomMealAsync();
        });
    }

    // ──────────────────────────────────────────────
    //  Connectivity monitoring (offline banner)
    // ──────────────────────────────────────────────

    /// <summary>
    /// Subscribes to <see cref="Connectivity.ConnectivityChanged"/> and
    /// sets the initial banner visibility based on the current network state.
    /// When the device goes offline a warning banner appears at the top of
    /// the page; it hides automatically when connectivity resumes.
    /// </summary>
    private void StartConnectivityMonitor()
    {
        UpdateOfflineBanner();
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    /// <summary>Unsubscribes from connectivity events to prevent leaks.</summary>
    private void StopConnectivityMonitor()
    {
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
    }

    /// <summary>
    /// Event handler fired whenever the network access level changes.
    /// Runs on the main thread so the UI update is safe.
    /// </summary>
    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateOfflineBanner);
    }

    /// <summary>
    /// Shows or hides the offline banner based on
    /// <see cref="Connectivity.Current.NetworkAccess"/>.
    /// </summary>
    private void UpdateOfflineBanner()
    {
        var isOffline = Connectivity.Current.NetworkAccess != NetworkAccess.Internet;
        OfflineBanner.IsVisible = isOffline;
    }
}
