using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<FoodModel> _foodMemories = [];

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasStatusMessage;

    private CancellationTokenSource? _searchCts;

    public MainPageViewModel()
    {
        Title = "Foodie Log";
    }

    /// <summary>
    /// Load memories from the cloud (mockapi.io) on first launch.
    /// Falls back to local data if the network is unavailable.
    /// All heavy work (JSON + HTTP) is offloaded to the thread pool by FoodLogService.
    /// </summary>
    [RelayCommand]
    private async Task LoadMemoriesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearStatus();

            var items = await FoodLogService.SearchAsync(SearchQuery);
            FoodMemories = new ObservableCollection<FoodModel>(items);
            IsEmpty = FoodMemories.Count == 0;

            if (FoodLogService.LastLoadUsedMockApi)
            {
                ShowStatus("☁️ Loaded from cloud", autoClearSeconds: 2);
            }
            else if (FoodLogService.LastError is not null)
            {
                ShowStatus($"⚠️ {FoodLogService.LastError}", autoClearSeconds: 5);
            }
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    /// <summary>
    /// Pull-to-refresh — forces a fresh fetch from mockapi.io.
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        ClearStatus();

        try
        {
            await FoodLogService.RefreshAsync();

            var items = await FoodLogService.SearchAsync(SearchQuery);
            FoodMemories = new ObservableCollection<FoodModel>(items);
            IsEmpty = FoodMemories.Count == 0;

            ShowStatus("🔄 Refreshed from cloud", autoClearSeconds: 2);
        }
        catch (Exception ex)
        {
            ShowStatus($"⚠️ Refresh failed — {ex.Message}", autoClearSeconds: 5);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    /// <summary>
    /// Debounced search — cancels the previous in-flight search and waits 300 ms
    /// before executing, so rapid keystrokes don't flood the API or block the UI.
    /// </summary>
    [RelayCommand]
    private async Task SearchAsync()
    {
        // Cancel any pending search
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        try
        {
            // Wait 300 ms; if another keystroke arrives, this gets cancelled
            await Task.Delay(300, token);
            await LoadMemoriesAsync();
        }
        catch (TaskCanceledException)
        {
            // Debounced — a newer search replaced this one
        }
    }

    [RelayCommand]
    private async Task GoToAddEntryAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.AddEntryPage));
    }

    [RelayCommand]
    private async Task GoToDetailAsync(FoodModel? item)
    {
        if (item is null) return;
        await Shell.Current.DisplayAlert(item.Name, item.Review, "OK");
    }

    // ──────────────────────────────────────────────
    //  Status helpers
    // ──────────────────────────────────────────────

    private void ShowStatus(string message, int autoClearSeconds = 0)
    {
        StatusMessage = message;
        HasStatusMessage = true;

        if (autoClearSeconds > 0)
        {
            _ = ClearStatusAfterDelay(autoClearSeconds);
        }
    }

    private void ClearStatus()
    {
        StatusMessage = string.Empty;
        HasStatusMessage = false;
    }

    private async Task ClearStatusAfterDelay(int seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        ClearStatus();
    }
}
