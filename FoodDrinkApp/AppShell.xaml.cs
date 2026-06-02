namespace FoodDrinkApp;

/// <summary>
/// Application Shell — owns the 5-tab bottom navigation bar and
/// registers Shell routes for pages reached via programmatic
/// navigation (e.g. AddEntryPage reached from MainPage).
/// Tab visibility and order are defined in <c>AppShell.xaml</c>.
/// </summary>
public partial class AppShell : Shell
{
    /// <summary>
    /// Initialises the Shell, registers XAML-compiled routes for
    /// non-tab pages.
    /// </summary>
    public AppShell()
    {
        InitializeComponent();

        // Non-tab pages that are navigated to via Shell.Current.GoToAsync
        Routing.RegisterRoute(nameof(Views.AddEntryPage), typeof(Views.AddEntryPage));
        Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
        Routing.RegisterRoute(nameof(FoodDetailPage), typeof(FoodDetailPage));
    }
}
