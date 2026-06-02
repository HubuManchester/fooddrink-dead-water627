using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FoodDrinkApp.Models;

/// <summary>
/// Core data model for a food memory entry in the Foodie Log app.
/// Represents a complete dining experience: dish name, restaurant,
/// personal review, photo, GPS location, date, star rating, optional
/// nutrition facts (calories, protein, carbs, fat), and search tags.
/// </summary>
/// <remarks>
/// <para>
/// This class uses CommunityToolkit.Mvvm <see cref="ObservablePropertyAttribute"/>
/// on private backing fields.  The source generator creates public
/// properties (Name, RestaurantName, etc.) with INotifyPropertyChanged
/// support.  <see cref="JsonPropertyNameAttribute"/> on each field tells
/// System.Text.Json how to map camelCase JSON keys during deserialisation
/// from mockapi.io.
/// </para>
/// <para>
/// <see cref="JsonIgnoreAttribute"/> on computed properties prevents them
/// from being serialised to JSON — they exist for UI display only.
/// </para>
/// </remarks>
public sealed partial class FoodModel : ObservableObject
{
    // ── Identity ────────────────────────────────────

    /// <summary>Unique identifier.  Auto-generated as a 32-char hex GUID if not supplied by the API.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // ── Core fields ──────────────────────────────────

    /// <summary>Name of the dish or drink (e.g. "Berry Yogurt Bowl").</summary>
    [JsonPropertyName("name")]
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>Restaurant or place where the meal was eaten.</summary>
    [JsonPropertyName("restaurantName")]
    [ObservableProperty]
    private string _restaurantName = string.Empty;

    /// <summary>Personal review / description of the dining experience.</summary>
    [JsonPropertyName("review")]
    [ObservableProperty]
    private string _review = string.Empty;

    /// <summary>Local file-system path of the captured food photo.  Stored in <c>FileSystem.CacheDirectory</c>.</summary>
    [JsonPropertyName("imagePath")]
    [ObservableProperty]
    private string _imagePath = string.Empty;

    /// <summary>Human-readable dining location (city, district, country).  May be auto-filled by GPS geocoding.</summary>
    [JsonPropertyName("location")]
    [ObservableProperty]
    private string _location = string.Empty;

    /// <summary>The date of the meal — cannot be set in the future (enforced by <c>AddEntryPageViewModel</c>).</summary>
    [JsonPropertyName("date")]
    [ObservableProperty]
    private DateTime _date = DateTime.Today;

    /// <summary>Star rating from 1 to 5, set via the rating-picker buttons on the AddEntryPage.</summary>
    [JsonPropertyName("rating")]
    [ObservableProperty]
    private int _rating;

    /// <summary>Space-separated tags used for full-text search (e.g. "healthy breakfast yogurt").</summary>
    [JsonPropertyName("tags")]
    [ObservableProperty]
    private string _tags = string.Empty;

    // ── Nutrition (optional — consumed by StatisticsPage charts) ───

    /// <summary>Calories in kilocalories (kcal).  Optional; defaults to 0.</summary>
    [JsonPropertyName("calories")]
    [ObservableProperty]
    private int _calories;

    /// <summary>Protein content in grams.  Optional; defaults to 0.</summary>
    [JsonPropertyName("protein")]
    [ObservableProperty]
    private int _protein;

    /// <summary>Carbohydrate content in grams.  Optional; defaults to 0.</summary>
    [JsonPropertyName("carbs")]
    [ObservableProperty]
    private int _carbs;

    /// <summary>Fat content in grams.  Optional; defaults to 0.</summary>
    [JsonPropertyName("fat")]
    [ObservableProperty]
    private int _fat;

    // ── Computed display properties (never serialised to JSON) ───

    /// <summary>Formatted date string for UI display, e.g. "2025-06-01".</summary>
    [JsonIgnore]
    public string DateLabel => Date.ToString("yyyy-MM-dd");

    /// <summary>Star-rating string for UI display, e.g. "⭐⭐⭐" for rating 3.</summary>
    [JsonIgnore]
    public string RatingLabel => Rating > 0 ? new string('⭐', Rating) : "Not rated";

    /// <summary>
    /// Full-sentence description read by the screen reader (TalkBack).
    /// Format: "{Name}. At {RestaurantName}. {Review}. Location: {Location}..."
    /// </summary>
    [JsonIgnore]
    public string AccessibleSummary =>
        $"{Name}. At {RestaurantName}. {Review}. Location: {Location}. Date: {DateLabel}. Rating: {Rating} stars.";

    /// <summary>Compact inline subtitle for the card list: "Grandma's Kitchen  ·  2025-06-01".</summary>
    [JsonIgnore]
    public string CardSubtitle =>
        string.IsNullOrWhiteSpace(RestaurantName) ? DateLabel : $"{RestaurantName}  ·  {DateLabel}";

    /// <summary>Returns <see langword="true"/> when at least one nutrition field has been populated.</summary>
    [JsonIgnore]
    public bool HasNutrition => Calories > 0 || Protein > 0 || Carbs > 0 || Fat > 0;

    /// <summary>Single-line nutrition summary for detail views, e.g. "520 kcal  ·  P:38g  C:58g  F:14g".</summary>
    [JsonIgnore]
    public string NutritionSummary =>
        HasNutrition
            ? $"{Calories} kcal  ·  P:{Protein}g  C:{Carbs}g  F:{Fat}g"
            : string.Empty;

    // ── Detail-page display helpers ───────────────────

    /// <summary>Formatted calorie label, e.g. "520 kcal".</summary>
    [JsonIgnore]
    public string CaloriesLabel => $"{Calories} kcal";

    /// <summary>True when this entry exceeds the 800 kcal threshold.</summary>
    [JsonIgnore]
    public bool IsHighCalorie => Calories > 800;

    /// <summary>Semantic alert text for the calorie banner.</summary>
    [JsonIgnore]
    public string CalorieAlertText => Calories > 800
        ? "🚨 High-Calorie Intake Alert"
        : "✅ Healthy Portion Size";

    /// <summary>Background colour for the calorie alert banner.</summary>
    [JsonIgnore]
    public Color CalorieAlertBackground => Calories > 800
        ? Color.FromArgb("#FDE8E8")
        : Color.FromArgb("#E6F4EA");

    /// <summary>Text colour for the calorie alert banner.</summary>
    [JsonIgnore]
    public Color CalorieAlertTextColor => Calories > 800
        ? Color.FromArgb("#C62828")
        : Color.FromArgb("#2E7D32");

    /// <summary>Macro assessment label for protein.</summary>
    [JsonIgnore]
    public string ProteinLevel => Protein switch
    {
        >= 30 => "High",
        >= 10 => "Balanced",
        _     => "Low"
    };

    /// <summary>Macro assessment label for carbohydrates.</summary>
    [JsonIgnore]
    public string CarbsLevel => Carbs switch
    {
        >= 50 => "High",
        >= 20 => "Balanced",
        _     => "Low"
    };

    /// <summary>Macro assessment label for fat.</summary>
    [JsonIgnore]
    public string FatLevel => Fat switch
    {
        >= 20 => "High",
        >= 5  => "Balanced",
        _     => "Low"
    };
}
