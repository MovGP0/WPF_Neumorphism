using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class ToolTipsPage : UserControl
{
    public ToolTipsPage()
    {
        InitializeComponent();
    }

    private void Refresh_Click(object sender, RoutedEventArgs e) => Announce("Preview refreshed.");
    private void Hint_Click(object sender, RoutedEventArgs e) => Announce("Keyboard hint acknowledged.");

    private void Announce(string message)
    {
        Feedback.Text = message;
        UIElementAutomationPeer.CreatePeerForElement(Feedback)?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
    }
}
