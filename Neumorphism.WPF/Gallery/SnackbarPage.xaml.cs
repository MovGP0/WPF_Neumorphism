using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class SnackbarPage : UserControl
{
    public SnackbarPage()
    {
        InitializeComponent();
        _timeout.Tick += (_, _) =>
        {
            if (!Notice.IsKeyboardFocusWithin && !Notice.IsMouseOver)
                CloseNotice();
        };
        Unloaded += (_, _) =>
        {
            if (_timeout.IsEnabled)
                CloseNotice();
        };
    }

    private readonly DispatcherTimer _timeout = new() { Interval = TimeSpan.FromSeconds(6) };

    private void ShowStatus_Click(object sender, RoutedEventArgs e) => OpenNotice("Settings saved.", false);
    private void ShowUndo_Click(object sender, RoutedEventArgs e) => OpenNotice("Draft archived.", true);

    private void OpenNotice(string text, bool hasUndo)
    {
        _timeout.Stop();
        Feedback.Text = string.Empty;
        Undo.Visibility = hasUndo ? Visibility.Visible : Visibility.Collapsed;
        Notice.Visibility = Visibility.Visible;
        Message.Text = text;
        Announce(Message);
        if (!hasUndo)
            _timeout.Start();
    }

    private void Dismiss_Click(object sender, RoutedEventArgs e) => CloseNotice();

    private void Undo_Click(object sender, RoutedEventArgs e)
    {
        CloseNotice();
        Feedback.Text = "Draft restored.";
        Announce(Feedback);
    }

    private void CloseNotice()
    {
        _timeout.Stop();
        if (Notice.IsKeyboardFocusWithin)
        {
            var trigger = Undo.IsVisible ? ShowUndo : ShowStatus;
            trigger.Focus();
        }

        Notice.Visibility = Visibility.Collapsed;
    }

    private static void Announce(TextBlock text) =>
        UIElementAutomationPeer.CreatePeerForElement(text)?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
}
