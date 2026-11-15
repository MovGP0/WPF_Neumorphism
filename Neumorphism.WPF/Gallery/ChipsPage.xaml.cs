using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class ChipsPage : UserControl
{
    public ChipsPage()
    {
        InitializeComponent();
    }

    private void Action_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var name = AutomationProperties.GetName(button);
        Announce($"Activated {(string.IsNullOrEmpty(name) ? button.Content : name)}.");
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        InputChip.Visibility = Visibility.Collapsed;
        RestoreChip.Visibility = Visibility.Visible;
        RestoreChip.Focus();
        Announce("Design removed.");
    }

    private void Restore_Click(object sender, RoutedEventArgs e)
    {
        RestoreChip.Visibility = Visibility.Collapsed;
        InputChip.Visibility = Visibility.Visible;
        InputChip.Focus();
        Announce("Design restored.");
    }

    private void Announce(string text)
    {
        Feedback.Text = text;
        UIElementAutomationPeer.CreatePeerForElement(Feedback)?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
    }
}
