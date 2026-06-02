using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp;

/// <summary>
/// Detail dashboard for a single food memory entry loaded from the
/// cloud data service.  Uses compiled XAML bindings to
/// <see cref="FoodModel"/> — no manual label assignment needed.
/// Reached via Shell navigation from the MainPage card list.
/// </summary>
/// <remarks>
/// Text-to-speech and hardware vibration are intentionally excluded
/// from this page.  The target test device lacks stable system-level
/// TTS and haptic support, so the page only renders pure data-driven
/// content that is safe across all platforms.
/// </remarks>
[QueryProperty(nameof(ItemId), "id")]
public partial class FoodDetailPage : ContentPage
{
    private FoodModel? _currentItem;

    /// <summary>
    /// Parameterless constructor.  BindingContext is set once the item
    /// loads via the <c>ItemId</c> query property.
    /// </summary>
    public FoodDetailPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Applies large-text accessibility scaling each time the page
    /// appears.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
    }

    /// <summary>
    /// Navigation query-property setter.  Triggers an async load of the
    /// food memory identified by <paramref name="id"/>.
    /// </summary>
    public string ItemId
    {
        set => _ = LoadItemAsync(value);
    }

    /// <summary>
    /// Fetches the <see cref="FoodModel"/> from the data service and
    /// assigns it as the page's BindingContext, populating all compiled
    /// bindings automatically.
    /// </summary>
    private async Task LoadItemAsync(string id)
    {
        _currentItem = await FoodLogService.GetByIdAsync(id);

        if (_currentItem is null)
        {
            _currentItem = new FoodModel
            {
                Name = "Record not found",
                Review = "The selected food memory could not be loaded.",
                RestaurantName = "—",
                Location = string.Empty
            };
        }

        BindingContext = _currentItem;
    }
}
