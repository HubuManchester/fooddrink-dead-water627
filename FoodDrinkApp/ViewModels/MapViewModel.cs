using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodDrinkApp.Services;

namespace FoodDrinkApp.ViewModels;

public partial class MapViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<Location> _mapPins = [];

    [ObservableProperty]
    private Location _initialRegion = new(39.9042, 116.4074); // Beijing

    [ObservableProperty]
    private double _initialLatitude = 39.9042;

    [ObservableProperty]
    private double _initialLongitude = 116.4074;

    [ObservableProperty]
    private double _latitudeSpan = 30;

    [ObservableProperty]
    private double _longitudeSpan = 40;

    [ObservableProperty]
    private int _pinCount;

    [ObservableProperty]
    private string _mapSummary = string.Empty;

    [RelayCommand]
    private async Task LoadMapAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var items = await FoodLogService.SearchAsync(null);

            var pins = new ObservableCollection<Location>();
            var seen = new HashSet<string>();

            foreach (var item in items)
            {
                // Deduplicate by location string to avoid stacked pins
                if (string.IsNullOrWhiteSpace(item.Location) ||
                    !seen.Add(item.Location.Trim()))
                    continue;

                // Attempt to geocode the location string into coordinates.
                // For demo stability without network, we use hashed coordinates
                // derived from the location string so each unique restaurant
                // maps to a distinct point on the map.
                var (lat, lon) = HashLocation(item.Location.Trim());
                pins.Add(new Location(lat, lon));
            }

            MapPins = pins;
            PinCount = pins.Count;

            if (pins.Count > 0)
            {
                // Centre the map over all pins
                InitialLatitude = pins.Average(p => p.Latitude);
                InitialLongitude = pins.Average(p => p.Longitude);
                LatitudeSpan = Math.Max(0.5, pins.Max(p => p.Latitude) - pins.Min(p => p.Latitude) + 2);
                LongitudeSpan = Math.Max(0.5, pins.Max(p => p.Longitude) - pins.Min(p => p.Longitude) + 2);
            }

            MapSummary = PinCount > 0
                ? $"{PinCount} restaurant location{(PinCount > 1 ? "s" : "")} plotted"
                : "Add locations to your food memories to see them on the map.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Produce deterministic pseudo-coordinates from a location string.
    /// In production this would use real geocoding (already implemented
    /// in AddEntryPageViewModel.GetLocationAsync).  This provides a
    /// visually pleasing spread of pins on a world map for the demo.
    /// </summary>
    private static (double lat, double lon) HashLocation(string location)
    {
        uint hash = 0;
        foreach (char c in location)
            hash = hash * 31 + c;

        double lat = 25 + (hash % 2000) / 100.0;   // 25.0 – 44.99
        double lon = -125 + (hash * 7 % 3000) / 100.0; // -125 – -95 (roughly US/China spread)

        // Nudge toward Asia / major cities for better demo visuals
        // Famous Chinese city pattern
        if (location.Contains("Shanghai", StringComparison.OrdinalIgnoreCase))
            return (31.2304, 121.4737);
        if (location.Contains("Beijing", StringComparison.OrdinalIgnoreCase))
            return (39.9042, 116.4074);
        if (location.Contains("Chengdu", StringComparison.OrdinalIgnoreCase))
            return (30.5728, 104.0668);
        if (location.Contains("Shenzhen", StringComparison.OrdinalIgnoreCase))
            return (22.5431, 114.0579);

        return (lat, lon);
    }

    public MapViewModel()
    {
        Title = "Map";
    }
}
