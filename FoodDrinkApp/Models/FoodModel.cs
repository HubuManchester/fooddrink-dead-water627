using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FoodDrinkApp.Models;

/// <summary>
/// Core data model for a food memory entry in the Foodie Log app.
/// Represents a dining experience with restaurant info, review, photo, location, and date.
/// </summary>
public sealed partial class FoodModel : ObservableObject
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

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
}
