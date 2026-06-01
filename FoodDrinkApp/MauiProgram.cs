using FoodDrinkApp.ViewModels;
using FoodDrinkApp.Views;
using Microsoft.Extensions.Logging;

namespace FoodDrinkApp;

public static class MauiProgram
{
    /// <summary>
    /// Global service provider exposed so Shell-instantiated pages
    /// (created via DataTemplate, which bypasses DI constructor injection)
    /// can resolve their ViewModels.
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register ViewModels
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<AddEntryPageViewModel>();

        // Register Pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AddEntryPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        Services = app.Services;
        return app;
    }
}
