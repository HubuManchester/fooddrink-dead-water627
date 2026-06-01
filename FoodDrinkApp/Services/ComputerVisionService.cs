using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FoodDrinkApp.Services;

/// <summary>
/// Computer Vision food-classification service.
///
/// Architecture: the service exposes a single async method,
/// ClassifyFoodImageAsync, that accepts a photo stream and returns
/// the most likely food label.
///
/// Production path (commented) sends the image to a cloud CV
/// endpoint such as Hugging Face Inference API or Azure Custom Vision.
/// The current MVP uses a deterministic local simulation so the
/// demo is 100 % reliable — no external network dependency.
/// </summary>
public static class ComputerVisionService
{
    // ──────────────────────────────────────────────────────
    //  Production HttpClient scaffold (ready to activate)
    // ──────────────────────────────────────────────────────

    // Replace with your actual endpoint and key:
    // private const string CvEndpoint = "https://api-inference.huggingface.co/models/nateraw/food";
    // private const string CvApiKey   = "hf_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

    // private static readonly HttpClient CvHttpClient = new()
    // {
    //     Timeout = TimeSpan.FromSeconds(20)
    // };

    // ── Production path (Azure Custom Vision variant) ──
    //
    // public static async Task<string> ClassifyFoodImageAsync(Stream imageStream)
    // {
    //     using var content = new StreamContent(imageStream);
    //     content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
    //
    //     var request = new HttpRequestMessage(HttpMethod.Post, CvEndpoint)
    //     {
    //         Content = content
    //     };
    //     request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CvApiKey);
    //
    //     var response = await CvHttpClient.SendAsync(request);
    //     response.EnsureSuccessStatusCode();
    //
    //     // Hugging Face food model returns an array of { label, score }
    //     var predictions = await response.Content.ReadFromJsonAsync<JsonElement[]>();
    //     var top = predictions?[0].GetProperty("label").GetString() ?? "Unknown";
    //     return ParseLabel(top);
    // }

    // ──────────────────────────────────────────────────────
    //  MVP simulation — deterministic pseudo-AI classifier
    // ──────────────────────────────────────────────────────

    /// <summary>
    /// Food categories returned by the simulated classifier.
    /// Each entry is a plausible dish name that could appear in
    /// a food photo.
    /// </summary>
    private static readonly string[] FoodLabels =
    [
        "Healthy Avocado Salad",
        "Classic Beef Burger",
        "Fresh Salmon Sushi",
        "Margherita Pizza",
        "Berry Yogurt Bowl",
        "Chicken Brown Rice Box",
        "Spicy Ramen Noodles",
        "Matcha Tiramisu",
        "Braised Pork Belly Rice",
        "Vegetable Stir Fry",
        "Iced Matcha Latte",
        "Tomato Pasta"
    ];

    private static readonly Random Rng = new();

    /// <summary>
    /// Classify a food photo.
    ///
    /// In production this would POST the image to a cloud CV model.
    /// For the MVP demo it simulates ~1.5 s model-inference latency
    /// and returns a deterministic label keyed on the photo stream
    /// length so repeated runs with the same image give the same
    /// output — making the screencast fully predictable.
    /// </summary>
    /// <param name="imageStream">JPEG or PNG photo stream.</param>
    /// <returns>Food label string, e.g. "Classic Beef Burger".</returns>
    public static async Task<string> ClassifyFoodImageAsync(Stream imageStream)
    {
        // ── Simulate cloud-model latency ──────────────
        // AI model inference on a remote GPU takes 1–2 seconds.
        // We simulate this so the screencast shows the UX in action.
        await Task.Delay(1500);

        // ── Deterministic label selection ─────────────
        // Use the stream length as a stable seed so the same image
        // always produces the same result during the demo.
        int index = (int)(imageStream.Length % FoodLabels.Length);
        string label = FoodLabels[index];

        // Simulate occasional double-word return from the API:
        // e.g. raw json → "AI Predicted: Healthy Avocado Salad"
        return label;
    }

    // ──────────────────────────────────────────────────────
    //  Helpers (for production path)
    // ──────────────────────────────────────────────────────

    /// <summary>
    /// Normalise the raw AI label into a clean dish name.
    /// Production Hugging Face / Azure labels often include
    /// underscores or trailing confidence tokens.
    /// </summary>
    private static string ParseLabel(string raw)
    {
        return raw
            .Replace('_', ' ')
            .Replace("food,", "")
            .Replace("cuisine,", "")
            .Trim();
    }
}
