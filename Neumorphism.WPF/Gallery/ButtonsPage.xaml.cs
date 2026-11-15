using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Neumorphism.WPF.Gallery;

public partial class ButtonsPage : UserControl
{
    public ButtonsPage()
    {
        InitializeComponent();
    }

    private void DemoButton_Click(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not Button button)
            return;

        if (button.ContextMenu is { } menu)
        {
            menu.PlacementTarget = button == SaveMenuButton ? SaveSplit : button;
            menu.Placement = PlacementMode.Bottom;
            menu.IsOpen = true;
            return;
        }

        string name = AutomationProperties.GetName(button);
        ShowFeedback($"Activated {(!string.IsNullOrEmpty(name) ? name : button.Content)}.");
    }

    private void DemoMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (((MenuItem)sender).Header is string header)
            ShowFeedback($"Selected {header.Replace("_", "")}.");
    }

    private void DemoToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (IsLoaded)
            ShowFeedback(((ToggleButton)sender).IsChecked == true ? "Toggle is on." : "Toggle is off.");
    }

    private void ShowFeedback(string text)
    {
        Feedback.Text = text;
        var peer = UIElementAutomationPeer.FromElement(Feedback) ?? UIElementAutomationPeer.CreatePeerForElement(Feedback);
        peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
    }
}
