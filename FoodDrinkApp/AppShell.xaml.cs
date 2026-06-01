namespace FoodDrinkApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Foodie Log routes — only our own pages are registered.
        Routing.RegisterRoute(nameof(Views.AddEntryPage), typeof(Views.AddEntryPage));
    }
}
