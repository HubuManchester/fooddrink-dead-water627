using System.Net.Http.Json;
using System.Text.Json;
using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

/// <summary>
/// Central data service for food memory entries.
/// Primary data source: mockapi.io REST API.
/// Falls back to local in-memory data when the network is unavailable
/// so the app remains fully usable during demos.
///
/// ALL HttpClient calls are wrapped in Task.Run with async lambdas
/// to push synchronous JSON serialisation / deserialisation off the
/// main thread, preventing ANR on Android.
/// </summary>
public static class FoodLogService
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(12)
    };

    /// <summary>
    /// JSON serialiser options configured for mockapi.io.
    ///
    /// IncludeFields = true is CRITICAL: FoodModel uses
    /// CommunityToolkit.Mvvm [ObservableProperty] on private fields.
    /// System.Text.Json defaults to property-only serialisation and
    /// would silently skip the source-generated properties unless
    /// it can also reach the backing fields via IncludeFields.
    ///
    /// PropertyNameCaseInsensitive handles mockapi.io's camelCase keys
    /// (e.g. "restaurantName" → RestaurantName).
    ///
    /// PropertyNamingPolicy.CamelCase ensures POST bodies use camelCase
    /// keys expected by mockapi.io.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
    };

    private static readonly List<FoodModel> LocalFallbackItems =
    [
        new()
        {
            Name = "Braised Pork Belly Rice",
            RestaurantName = "Grandma's Kitchen",
            Review = "Incredibly tender pork belly with perfectly seasoned soy broth. The rice soaked up all the flavour.",
            ImagePath = "",
            Location = "Shanghai, Jing'an District",
            Date = DateTime.Today.AddDays(-3),
            Rating = 5,
            Tags = "Chinese braised pork comfort"
        },
        new()
        {
            Name = "Matcha Tiramisu",
            RestaurantName = "Green Leaf Café",
            Review = "Beautiful layers of matcha cream and espresso-soaked sponge. Not too sweet — just right.",
            ImagePath = "",
            Location = "Beijing, Chaoyang District",
            Date = DateTime.Today.AddDays(-1),
            Rating = 4,
            Tags = "dessert matcha café"
        },
        new()
        {
            Name = "Spicy Hotpot Set",
            RestaurantName = "Chengdu Impression",
            Review = "Numbing Sichuan pepper with rich beef tallow broth. Great variety of fresh ingredients.",
            ImagePath = "",
            Location = "Chengdu, Jinjiang District",
            Date = DateTime.Today.AddDays(-7),
            Rating = 5,
            Tags = "hotpot Sichuan spicy dinner"
        },
        new()
        {
            Name = "Avocado Salmon Toast",
            RestaurantName = "Morning Bliss Brunch",
            Review = "Crispy sourdough topped with smashed avocado, smoked salmon, and a poached egg.",
            ImagePath = "",
            Location = "Shenzhen, Nanshan District",
            Date = DateTime.Today,
            Rating = 4,
            Tags = "brunch toast healthy salmon"
        }
    ];

    private static readonly object CacheLock = new();
    private static List<FoodModel> cachedItems = new(LocalFallbackItems);

    /// <summary>
    /// True when the most recent GetAllAsync call successfully reached mockapi.io.
    /// </summary>
    public static bool LastLoadUsedMockApi { get; private set; }

    /// <summary>
    /// Human-readable description of the last error, or null if the last operation succeeded.
    /// </summary>
    public static string? LastError { get; private set; }

    // ──────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────

    /// <summary>
    /// Search food memories. An empty query returns all items ordered by date descending.
    /// Searches across name, restaurant, review, location, and tags.
    /// </summary>
    public static async Task<IReadOnlyList<FoodModel>> SearchAsync(string? query)
    {
        var items = await GetAllAsync().ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(query))
        {
            return items.OrderByDescending(item => item.Date).ToList();
        }

        var normalised = query.Trim();
        return items
            .Where(item =>
                item.Name.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.RestaurantName.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Review.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Location.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Tags.Contains(normalised, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.Date)
            .ToList();
    }

    /// <summary>
    /// Fetch a single food memory by ID. Tries the API first, then the local cache.
    /// </summary>
    public static async Task<FoodModel?> GetByIdAsync(string id)
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                // async lambda — ensures the inner Task<T> is unwrapped before Task.Run wraps
                var item = await Task.Run(async () =>
                    await HttpClient.GetFromJsonAsync<FoodModel>(
                        $"{MockApiConfig.EndpointUrl.TrimEnd('/')}/{Uri.EscapeDataString(id)}",
                        JsonOptions)
                ).ConfigureAwait(false);

                if (item is not null)
                {
                    LastError = null;
                    return item;
                }
            }
            catch (Exception ex)
            {
                LastError = $"Failed to fetch item {id}: {ex.Message}";
            }
        }

        return cachedItems.FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    /// Add a new food memory. POSTs to mockapi.io when configured;
    /// falls back to adding to the local cache on failure.
    /// Returns the created item (which may include a server-assigned ID).
    ///
    /// The entire POST + JSON serialisation is pushed to a thread-pool thread
    /// via Task.Run to avoid blocking the Android main thread.
    /// </summary>
    public static async Task<FoodModel> AddAsync(FoodModel item)
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                // async lambda — already correct in the previous version
                var created = await Task.Run(async () =>
                {
                    var response = await HttpClient.PostAsJsonAsync(
                        MockApiConfig.EndpointUrl, item, JsonOptions);

                    response.EnsureSuccessStatusCode();

                    return await response.Content
                        .ReadFromJsonAsync<FoodModel>(JsonOptions);
                }).ConfigureAwait(false);

                if (created is not null)
                {
                    lock (CacheLock) { cachedItems.Add(created); }
                    LastError = null;
                    return created;
                }
            }
            catch (Exception ex)
            {
                LastError = $"Network save failed — saved locally. {ex.Message}";
                // Fall through to local cache.
            }
        }

        // Local fallback path
        lock (CacheLock) { cachedItems.Add(item); }
        LastError = null;
        return item;
    }

    /// <summary>
    /// Delete a food memory by ID.  Sends DELETE to mockapi.io when configured;
    /// always removes from the local cache regardless of network outcome.
    /// Returns true if the item was found and removed.
    /// </summary>
    public static async Task<bool> DeleteAsync(string id)
    {
        bool removedLocally = false;
        lock (CacheLock)
        {
            removedLocally = cachedItems.RemoveAll(item => item.Id == id) > 0;
        }

        if (MockApiConfig.IsConfigured)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var response = await HttpClient.DeleteAsync(
                        $"{MockApiConfig.EndpointUrl.TrimEnd('/')}/{Uri.EscapeDataString(id)}");
                    response.EnsureSuccessStatusCode();
                }).ConfigureAwait(false);

                LastError = null;
            }
            catch (Exception ex)
            {
                LastError = $"Delete from cloud failed — removed locally. {ex.Message}";
            }
        }

        return removedLocally;
    }

    /// <summary>
    /// Force-refresh the in-memory cache from the mockapi.io endpoint.
    /// Runs the HTTP GET + JSON deserialisation on a background thread.
    /// </summary>
    public static async Task RefreshAsync()
    {
        if (!MockApiConfig.IsConfigured)
        {
            LastLoadUsedMockApi = false;
            return;
        }

        // async lambda — ensures the inner Task<List<T>> is unwrapped inside Task.Run
        var items = await Task.Run(async () =>
            await HttpClient.GetFromJsonAsync<List<FoodModel>>(
                MockApiConfig.EndpointUrl, JsonOptions)
        ).ConfigureAwait(false);

        if (items is { Count: > 0 })
        {
            lock (CacheLock) { cachedItems = items; }
            LastLoadUsedMockApi = true;
            LastError = null;
        }
    }

    // ──────────────────────────────────────────────
    //  Internal helpers
    // ──────────────────────────────────────────────

    /// <summary>
    /// Fetches all items from mockapi.io (or returns the cached list
    /// when the API is not configured).  Offloads the HTTP + JSON
    /// work to a thread-pool thread.
    /// </summary>
    private static async Task<IReadOnlyList<FoodModel>> GetAllAsync()
    {
        if (!MockApiConfig.IsConfigured)
        {
            LastLoadUsedMockApi = false;
            LastError = null;
            return cachedItems;
        }

        try
        {
            // async lambda — the inner await unwraps GetFromJsonAsync's Task<List<FoodModel>>
            // BEFORE the outer Task.Run wraps the result, avoiding the Task<Task<T>> trap.
            var items = await Task.Run(async () =>
                await HttpClient.GetFromJsonAsync<List<FoodModel>>(
                    MockApiConfig.EndpointUrl, JsonOptions)
            ).ConfigureAwait(false);

            if (items is { Count: > 0 })
            {
                lock (CacheLock) { cachedItems = items; }
                LastLoadUsedMockApi = true;
                LastError = null;
                return cachedItems;
            }
        }
        catch (Exception ex)
        {
            LastError = $"Could not reach mockapi.io — showing local data. {ex.Message}";
        }

        LastLoadUsedMockApi = false;
        return cachedItems;
    }
}
