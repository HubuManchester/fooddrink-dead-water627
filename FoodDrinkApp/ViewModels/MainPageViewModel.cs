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

    public MainPageViewModel()
    {
        Title = "Foodie Log";
    }

    [RelayCommand]
    private async Task LoadMemoriesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var items = await FoodLogService.SearchAsync(SearchQuery);
            FoodMemories = new ObservableCollection<FoodModel>(items);
            IsEmpty = FoodMemories.Count == 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadMemoriesAsync();
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
        // Detail page will be added in a future phase
        await Shell.Current.DisplayAlert(item.Name, item.Review, "OK");
    }
}
