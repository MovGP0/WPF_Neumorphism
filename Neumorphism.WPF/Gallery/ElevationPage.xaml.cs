using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using Neumorphism.WPF.Theming;

namespace Neumorphism.WPF.Gallery;

public partial class ElevationPage : UserControl
{
    public ElevationPage()
    {
        InitializeComponent();
    }

    private void Example_Click(object sender, RoutedEventArgs e)
    {
        var elevation = SurfaceChrome.GetElevation((Button)sender);
        Feedback.Text = $"Activated the level {elevation.Level} button.";
        var peer = UIElementAutomationPeer.FromElement(Feedback) ?? UIElementAutomationPeer.CreatePeerForElement(Feedback);
        peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
    }
}
