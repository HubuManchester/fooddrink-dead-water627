using FoodDrinkApp.Services;
using FoodDrinkApp.ViewModels;
using Microsoft.Maui.Controls.Maps;

namespace FoodDrinkApp.Views;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MapPage() : this(MauiProgram.Services.GetRequiredService<MapViewModel>())
    {
    }

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        await _viewModel.LoadMapCommand.ExecuteAsync(null);
        PlotPins();
    }

    /// <summary>
    /// Places a pin on the map for each restaurant location
    /// computed by the ViewModel.  The map auto-centres based on
    /// the pin set on Android/iOS; Windows uses a separate map tile
    /// service and pins render automatically.
    /// </summary>
    private void PlotPins()
    {
        RestaurantMap.Pins.Clear();

        foreach (var loc in _viewModel.MapPins)
        {
            RestaurantMap.Pins.Add(new Pin
            {
                Label = "🍽️ Restaurant",
                Location = loc,
                Type = PinType.Place
            });
        }
    }
}
