using FoodDrinkApp.Services;
using FoodDrinkApp.ViewModels;
using Microsoft.Maui.Controls.Maps;

namespace FoodDrinkApp.Views;

/// <summary>
/// Interactive restaurant map — places a <see cref="Pin"/> on an
/// embedded <see cref="Map"/> control for each restaurant location
/// recorded in the user's food memories.  Location strings are
/// resolved to coordinates by <see cref="MapViewModel.HashLocation"/>.
/// </summary>
/// <remarks>
/// Android requires a valid Google Maps API key in
/// <c>AndroidManifest.xml</c>:
/// <c>&lt;meta-data android:name="com.google.android.geo.API_KEY" ... /&gt;</c>.
/// Without a real key the map renders as an empty grid — pins still
/// appear and the app does not crash.
/// </remarks>
public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    /// <summary>Parameterless constructor for Shell DataTemplate instantiation.</summary>
    public MapPage() : this(MauiProgram.Services.GetRequiredService<MapViewModel>())
    {
    }

    /// <summary>DI constructor — receives the ViewModel from the container.</summary>
    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    /// <summary>
    /// Applies accessibility scaling, loads pin coordinates from the
    /// service, and plots them on the map control.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        await _viewModel.LoadMapCommand.ExecuteAsync(null);
        PlotPins();
    }

    /// <summary>
    /// Clears the map and adds one <see cref="Pin"/> per restaurant
    /// location returned by the ViewModel.
    /// </summary>
    private void PlotPins()
    {
        RestaurantMap.Pins.Clear();

        foreach (var loc in _viewModel.MapPins)
        {
            RestaurantMap.Pins.Add(new Pin
            {
                Label    = "🍽️ Restaurant",
                Location = loc,
                Type     = PinType.Place
            });
        }
    }
}
