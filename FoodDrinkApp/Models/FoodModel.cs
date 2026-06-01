using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FoodDrinkApp.Models;

/// <summary>
/// Core data model for a food memory entry in the Foodie Log app.
/// Represents a dining experience with restaurant info, review, photo,
/// location, date, rating, and optional nutrition facts.
/// </summary>
public sealed partial class FoodModel : ObservableObject
{
    // ── Identity ────────────────────────────────────

    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // ── Core fields ──────────────────────────────────

    [JsonPropertyName("name")]
    [ObservableProperty]
    private string _name = string.Empty;

    [JsonPropertyName("restaurantName")]
    [ObservableProperty]
    private string _restaurantName = string.Empty;

    [JsonPropertyName("review")]
    [ObservableProperty]
    private string _review = string.Empty;

    [JsonPropertyName("imagePath")]
    [ObservableProperty]
    private string _imagePath = string.Empty;

    [JsonPropertyName("location")]
    [ObservableProperty]
    private string _location = string.Empty;

    [JsonPropertyName("date")]
    [ObservableProperty]
    private DateTime _date = DateTime.Today;

    [JsonPropertyName("rating")]
    [ObservableProperty]
    private int _rating;

    [JsonPropertyName("tags")]
    [ObservableProperty]
    private string _tags = string.Empty;

    // ── Nutrition (optional — used by Statistics) ───

    [JsonPropertyName("calories")]
    [ObservableProperty]
    private int _calories;

    [JsonPropertyName("protein")]
    [ObservableProperty]
    private int _protein;

    [JsonPropertyName("carbs")]
    [ObservableProperty]
    private int _carbs;

    [JsonPropertyName("fat")]
    [ObservableProperty]
    private int _fat;

    // ── Computed display properties ──────────────────

    [JsonIgnore]
    public string DateLabel => Date.ToString("yyyy-MM-dd");

    [JsonIgnore]
    public string RatingLabel => Rating > 0 ? new string('⭐', Rating) : "Not rated";

    [JsonIgnore]
    public string AccessibleSummary =>
        $"{Name}. At {RestaurantName}. {Review}. Location: {Location}. Date: {DateLabel}. Rating: {Rating} stars.";

    [JsonIgnore]
    public string CardSubtitle =>
        string.IsNullOrWhiteSpace(RestaurantName) ? DateLabel : $"{RestaurantName}  ·  {DateLabel}";

    [JsonIgnore]
    public bool HasNutrition => Calories > 0 || Protein > 0 || Carbs > 0 || Fat > 0;

    [JsonIgnore]
    public string NutritionSummary =>
        HasNutrition
            ? $"{Calories} kcal  ·  P:{Protein}g  C:{Carbs}g  F:{Fat}g"
            : string.Empty;
}
