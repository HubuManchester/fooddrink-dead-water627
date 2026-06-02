using System.Text.Json.Serialization;

namespace FoodDrinkApp.Models;

public sealed class FoodItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    [JsonPropertyName("restaurantName")]
    public string RestaurantName { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    [JsonPropertyName("protein")]
    public int Protein { get; set; }

    [JsonPropertyName("carbs")]
    public int Carbs { get; set; }

    [JsonPropertyName("fat")]
    public int Fat { get; set; }

    [JsonPropertyName("allergyNote")]
    public string AllergyNote { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public string Tags { get; set; } = string.Empty;

    [JsonIgnore]
    public string CaloriesLabel => $"{Calories} kcal";

    [JsonIgnore]
    public string RatingLabel => Rating > 0 ? new string('⭐', Rating) : "Not rated";

    [JsonIgnore]
    public bool IsHighCalorie => Calories > 800;

    [JsonIgnore]
    public string CalorieAlertText => Calories > 800
        ? "🚨 High-Calorie Intake Alert"
        : "✅ Healthy Portion Size";

    [JsonIgnore]
    public Color CalorieAlertBackground => Calories > 800
        ? Color.FromArgb("#FDE8E8")
        : Color.FromArgb("#E6F4EA");

    [JsonIgnore]
    public Color CalorieAlertTextColor => Calories > 800
        ? Color.FromArgb("#C62828")
        : Color.FromArgb("#2E7D32");

    /// <summary>Macro label with mock analysis text for UI badges.</summary>
    [JsonIgnore]
    public string ProteinLevel => Protein switch
    {
        >= 30 => "High",
        >= 10 => "Balanced",
        _     => "Low"
    };

    /// <summary>Macro label with mock analysis text for UI badges.</summary>
    [JsonIgnore]
    public string CarbsLevel => Carbs switch
    {
        >= 50 => "High",
        >= 20 => "Balanced",
        _     => "Low"
    };

    /// <summary>Macro label with mock analysis text for UI badges.</summary>
    [JsonIgnore]
    public string FatLevel => Fat switch
    {
        >= 20 => "High",
        >= 5  => "Balanced",
        _     => "Low"
    };

    [JsonIgnore]
    public string MacroSummary => $"Protein {Protein}g, carbs {Carbs}g, fat {Fat}g";

    [JsonIgnore]
    public string AccessibleSummary => $"{Name}. {Category}. {Calories} kcal. {MacroSummary}. {AllergyNote}";
}
