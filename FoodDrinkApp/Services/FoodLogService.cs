using System.Net.Http.Json;
using System.Text.Json;
using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

/// <summary>
/// Central data service for food memory entries.
/// Tries the mockapi.io REST endpoint first; falls back to local in-memory data
/// so the app remains usable during demos even without network access.
/// </summary>
public static class FoodLogService
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(12)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
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

    private static List<FoodModel> cachedItems = new(LocalFallbackItems);

    public static bool LastLoadUsedMockApi { get; private set; }

    public static async Task<IReadOnlyList<FoodModel>> SearchAsync(string? query)
    {
        var items = await GetAllAsync();

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

    public static async Task<FoodModel?> GetByIdAsync(string id)
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var item = await HttpClient.GetFromJsonAsync<FoodModel>(
                    $"{MockApiConfig.EndpointUrl.TrimEnd('/')}/{Uri.EscapeDataString(id)}",
                    JsonOptions);

                if (item is not null)
                {
                    return item;
                }
            }
            catch
            {
                // Fall back to the last loaded cache below.
            }
        }

        return cachedItems.FirstOrDefault(item => item.Id == id);
    }

    public static async Task<FoodModel> AddAsync(FoodModel item)
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync(MockApiConfig.EndpointUrl, item, JsonOptions);
                response.EnsureSuccessStatusCode();

                var created = await response.Content.ReadFromJsonAsync<FoodModel>(JsonOptions);
                if (created is not null)
                {
                    cachedItems.Add(created);
                    return created;
                }
            }
            catch
            {
                // Fall back to local cache on network failure.
            }
        }

        cachedItems.Add(item);
        return item;
    }

    private static async Task<IReadOnlyList<FoodModel>> GetAllAsync()
    {
        if (!MockApiConfig.IsConfigured)
        {
            LastLoadUsedMockApi = false;
            return cachedItems;
        }

        try
        {
            var items = await HttpClient.GetFromJsonAsync<List<FoodModel>>(MockApiConfig.EndpointUrl, JsonOptions);
            if (items is { Count: > 0 })
            {
                cachedItems = items;
                LastLoadUsedMockApi = true;
                return cachedItems;
            }
        }
        catch
        {
            // Keep the app usable during demos even if the network is unavailable.
        }

        LastLoadUsedMockApi = false;
        return cachedItems;
    }
}
