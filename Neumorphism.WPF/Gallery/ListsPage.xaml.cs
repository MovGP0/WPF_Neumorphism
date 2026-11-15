using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class ListsPage : UserControl
{
    public ListsPage()
    {
        InitializeComponent();
        Assets.ItemsSource = new[]
        {
            new Asset("Type scale", "Ready"),
            new Asset("Icon set", "In review"),
            new Asset("Palette", "Ready"),
            new Asset("Layouts", "Draft")
        };
    }

    public sealed record Asset(string Name, string State);
}
