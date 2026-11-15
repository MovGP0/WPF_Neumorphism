using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class NavigationBarsPage : UserControl
{
    public NavigationBarsPage()
    {
        InitializeComponent();
    }
}


public sealed record GalleryDestination(string Name, string Glyph, string Summary)
{
    public static IReadOnlyList<GalleryDestination> All { get; } =
    [
        new("Overview", "M 2,10 L 12,2 22,10 M 5,8 L 5,21 19,21 19,8 M 10,21 V 14 H 14 V 21", "Review recent activity and open tasks."),
        new("Library", "M 3,4 H 9 V 21 H 3 Z M 12,4 H 18 L 22,19 16,21 Z", "Browse shared references and reusable assets."),
        new("Settings", "M 5,2 V 22 M 12,2 V 22 M 19,2 V 22 M 2,7 H 8 M 9,16 H 15 M 16,10 H 22", "Manage workspace preferences.")
    ];
}
