using FoodDrinkApp.ViewModels;
using FoodDrinkApp.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

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
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── ViewModels ──────────────────────────────
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<AddEntryPageViewModel>();
        builder.Services.AddTransient<StatisticsViewModel>();
        builder.Services.AddTransient<GalleryViewModel>();
        builder.Services.AddTransient<MapViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // ── Pages ────────────────────────────────────
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AddEntryPage>();
        builder.Services.AddTransient<StatisticsPage>();
        builder.Services.AddTransient<GalleryPage>();
        builder.Services.AddTransient<MapPage>();
        builder.Services.AddTransient<ProfilePage>();

        // ── Settings ────────────────────────────────
        builder.Services.AddTransient<SettingsPage>();

        // ── Android: dampen scroll sensitivity ─────
        ConfigureSmoothScrolling();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        Services = app.Services;
        return app;
    }

    /// <summary>
    /// Applies Android platform-specific scroll handler mappings
    /// to prevent erratic, over-sensitive scrolling in the emulator.
    /// ScrollViews and CollectionViews receive damped fling behaviour
    /// and disabled overscroll glow to match a natural feel.
    /// </summary>
    private static void ConfigureSmoothScrolling()
    {
#if ANDROID
        ScrollViewHandler.Mapper.AppendToMapping("DampedScroll", (handler, view) =>
        {
            handler.PlatformView.OverScrollMode =
                Android.Views.OverScrollMode.Never;
            handler.PlatformView.SmoothScrollingEnabled = true;
            handler.PlatformView.VerticalScrollBarEnabled = true;
        });
#endif
    }
}
