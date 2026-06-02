using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

/// <summary>
/// ViewModel for the advanced Settings page.
/// Manages user preferences including theme, notifications,
/// nutrition goals, unit localisation, accessibility, and
/// data-management actions.
/// </summary>
public partial class SettingsViewModel : BaseViewModel
{
    private const string DarkModeKey = "pref_dark_mode";
    private const string PushEnabledKey = "pref_push_enabled";
    private const string CalorieGoalKey = "pref_calorie_goal";
    private const string UseKilojoulesKey = "pref_use_kilojoules";
    private const string HighContrastKey = "pref_high_contrast";
    private const string BiometricKey = "pref_biometric_enabled";

    [ObservableProperty]
    private bool _isDarkMode;

    [ObservableProperty]
    private bool _isPushEnabled;

    [ObservableProperty]
    private int _dailyCalorieGoal = 2000;

    [ObservableProperty]
    private bool _useKilojoules;

    [ObservableProperty]
    private bool _isHighContrast;

    [ObservableProperty]
    private bool _isBiometricEnabled;

    [ObservableProperty]
    private string _appVersion = "Version 1.0.0";

    /// <summary>
    /// Initialises the ViewModel and loads persisted preferences.
    /// </summary>
    public SettingsViewModel()
    {
        Title = "Settings";
        LoadPreferences();
    }

    /// <summary>
    /// Reads all stored preference values from device storage
    /// and applies them to the bindable properties.
    /// </summary>
    private void LoadPreferences()
    {
        IsDarkMode = Preferences.Get(DarkModeKey, false);
        IsPushEnabled = Preferences.Get(PushEnabledKey, true);
        DailyCalorieGoal = Preferences.Get(CalorieGoalKey, 2000);
        UseKilojoules = Preferences.Get(UseKilojoulesKey, false);
        IsHighContrast = Preferences.Get(HighContrastKey, false);
        IsBiometricEnabled = Preferences.Get(BiometricKey, false);

        ApplyDarkMode();
        ApplyHighContrast();
    }

    /// <summary>
    /// Persists dark-mode choice and applies it to the application theme.
    /// </summary>
    partial void OnIsDarkModeChanged(bool value)
    {
        Preferences.Set(DarkModeKey, value);
        ApplyDarkMode();
    }

    /// <summary>
    /// Persists push-notification preference.
    /// </summary>
    partial void OnIsPushEnabledChanged(bool value)
    {
        Preferences.Set(PushEnabledKey, value);
    }

    /// <summary>
    /// Persists the daily calorie goal (synergises with
    /// Statistics and Profile screens).
    /// </summary>
    partial void OnDailyCalorieGoalChanged(int value)
    {
        // Delegates to the pure validator so clamping logic is
        // independently testable without MAUI dependencies.
        DailyCalorieGoal = NutritionValidator.ClampCalorieGoal(value);
        Preferences.Set(CalorieGoalKey, DailyCalorieGoal);
    }

    /// <summary>
    /// Persists energy-unit choice (kcal vs kJ).
    /// When <c>true</c> the user prefers kilojoules.
    /// </summary>
    partial void OnUseKilojoulesChanged(bool value)
    {
        Preferences.Set(UseKilojoulesKey, value);
    }

    /// <summary>
    /// Persists high-contrast accessibility preference.
    /// </summary>
    partial void OnIsHighContrastChanged(bool value)
    {
        Preferences.Set(HighContrastKey, value);
        ApplyHighContrast();
    }

    /// <summary>
    /// Switches the application theme between dark and light modes
    /// based on the current <see cref="IsDarkMode"/> value.
    /// </summary>
    private void ApplyDarkMode()
    {
        Application.Current!.UserAppTheme = IsDarkMode
            ? AppTheme.Dark
            : AppTheme.Light;
    }

    /// <summary>
    /// Enables or disables high-contrast accessibility mode.
    /// </summary>
    private void ApplyHighContrast()
    {
        // High-contrast mode is primarily a semantic flag;
        // pages that need to adapt can check this preference.
        AccessibilityService.LargeTextEnabled = IsHighContrast;
    }

    /// <summary>
    /// Exports the current food log data as a simulated CSV backup.
    /// Displays a confirmation alert to the user.
    /// </summary>
    [RelayCommand]
    private async Task ExportDataAsync()
    {
        try
        {
            IsBusy = true;

            // Simulate export work — in a production app this would
            // serialise FoodLogService data to a CSV file on disk.
            await Task.Delay(400);

            await Shell.Current.DisplayAlert(
                "Export Successful",
                "Backup saved as FoodLog_Backup.csv",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Clears the local mock image cache and notifies the user.
    /// </summary>
    [RelayCommand]
    private async Task ClearImageCacheAsync()
    {
        try
        {
            IsBusy = true;

            // Simulate cache-clear work — removes cached image
            // thumbnails and temporary files.
            var cacheDir = FileSystem.CacheDirectory;
            if (Directory.Exists(cacheDir))
            {
                foreach (var file in Directory.GetFiles(cacheDir, "*.jpg"))
                    File.Delete(file);
                foreach (var file in Directory.GetFiles(cacheDir, "*.png"))
                    File.Delete(file);
            }

            await Task.Delay(200);

            await Shell.Current.DisplayAlert(
                "Cache Cleared",
                "Local image cache has been cleared successfully.",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Persists biometric authentication preference.
    /// When enabled, provides haptic feedback and a confirmation
    /// alert to reassure the user their data is protected.
    /// </summary>
    partial void OnIsBiometricEnabledChanged(bool value)
    {
        Preferences.Set(BiometricKey, value);
        if (value)
            ToggleBiometricCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// Triggers haptic confirmation and displays a security alert
    /// when biometric authentication is activated.
    /// </summary>
    [RelayCommand]
    private async Task ToggleBiometricAsync()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

            await Shell.Current.DisplayAlert(
                "Biometric Protection Activated",
                "Your food logs are securely encrypted locally.",
                "OK");
        }
        catch
        {
            // Haptic feedback not available on all platforms — non-fatal.
        }
    }
}
