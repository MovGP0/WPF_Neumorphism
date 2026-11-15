using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

internal static class GalleryCatalog
{
    public const string Buttons = "Buttons";
    public const string Cards = "Cards";
    public const string Elevation = "Elevation";

    public static IReadOnlyList<string> Groups { get; } =
    [
        Buttons,
        Cards,
        "Chips",
        "ComboBoxes",
        "DataGrids",
        Elevation,
        "Expander",
        "Fields",
        "Group Boxes",
        "Lists",
        "Menus & Tool Bars",
        "Navigation Bars",
        "Navigation Rail",
        "Pickers",
        "Progress Indicators",
        "Rating Bar",
        "Sliders",
        "Snackbar",
        "Toggles",
        "ToolTips",
        "Trees"
    ];

    public static UserControl? CreatePage(string group) => group switch
    {
        "Chips" => new ChipsPage(),
        "ComboBoxes" => new ComboBoxesPage(),
        "DataGrids" => new DataGridsPage(),
        "Expander" => new ExpanderPage(),
        "Fields" => new FieldsPage(),
        "Group Boxes" => new GroupBoxesPage(),
        "Lists" => new ListsPage(),
        "Menus & Tool Bars" => new MenusAndToolBarsPage(),
        "Navigation Bars" => new NavigationBarsPage(),
        _ => null
    };
}
