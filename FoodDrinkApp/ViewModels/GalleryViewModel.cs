using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

public partial class GalleryViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<FoodModel> _photos = [];

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isRefreshing;

    [RelayCommand]
    private async Task LoadGalleryAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var items = await FoodLogService.SearchAsync(null);
            var withPhotos = items
                .Where(i => !string.IsNullOrWhiteSpace(i.ImagePath))
                .OrderByDescending(i => i.Date)
                .ToList();

            Photos = new ObservableCollection<FoodModel>(withPhotos);
            IsEmpty = Photos.Count == 0;
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshGalleryAsync()
    {
        IsRefreshing = true;
        await FoodLogService.RefreshAsync();
        await LoadGalleryAsync();
    }

    public GalleryViewModel()
    {
        Title = "Gallery";
    }
}
