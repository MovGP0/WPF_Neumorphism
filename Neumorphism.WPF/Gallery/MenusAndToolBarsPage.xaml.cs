using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class MenusAndToolBarsPage : UserControl
{
    public MenusAndToolBarsPage()
    {
        InitializeComponent();
    }

    private void Menu_Click(object sender, RoutedEventArgs e) =>
        Announce(((string)((MenuItem)sender).Header).Replace("_", ""));

    private void Tool_Click(object sender, RoutedEventArgs e)
    {
        Tools.IsOverflowOpen = false;
        Announce((string)((Button)sender).Content);
    }

    private void Announce(string command)
    {
        Feedback.Text = $"Ran {command}.";
        UIElementAutomationPeer.CreatePeerForElement(Feedback)?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
    }
}
