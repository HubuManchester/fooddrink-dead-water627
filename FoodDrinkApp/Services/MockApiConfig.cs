namespace FoodDrinkApp.Services;

public static class MockApiConfig
{
    // Foodie Log mockapi.io endpoint — GET /foods, POST /foods, GET /foods/:id
    public const string EndpointUrl = "https://6a1d2f23bcc4f20d5ca416e8.mockapi.io/foods";

    public static bool IsConfigured => !string.IsNullOrWhiteSpace(EndpointUrl);
}
