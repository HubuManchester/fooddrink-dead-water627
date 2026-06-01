namespace FoodDrinkApp.Services;

/// <summary>
/// Central configuration point for the mockapi.io REST API endpoint.
///
/// <para>
/// <see cref="EndpointUrl"/> is read by <see cref="FoodLogService"/> to
/// determine whether cloud-storage features are active.  When the URL
/// is non-empty, FoodLogService routes all CRUD operations through
/// HttpClient; when empty it uses only the local in‑memory cache.
/// </para>
///
/// <para>To switch endpoints, simply change the <see cref="EndpointUrl"/>
/// constant and rebuild.</para>
/// </summary>
public static class MockApiConfig
{
    /// <summary>
    /// Full URL of the mockapi.io Resource.
    /// <br/>Example: <c>https://682xxxx.mockapi.io/api/v1/foods</c>
    /// </summary>
    public const string EndpointUrl = "https://6a1d2f23bcc4f20d5ca416e8.mockapi.io/foods";

    /// <summary>
    /// Returns <see langword="true"/> when <see cref="EndpointUrl"/>
    /// has been populated with a valid URL, enabling cloud-mode in
    /// <see cref="FoodLogService"/>.
    /// </summary>
    public static bool IsConfigured => !string.IsNullOrWhiteSpace(EndpointUrl);
}
