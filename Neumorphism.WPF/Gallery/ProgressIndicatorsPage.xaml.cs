using System.Windows;
using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class ProgressIndicatorsPage : UserControl
{
    public ProgressIndicatorsPage()
    {
        InitializeComponent();
    }

    private void Advance_Click(object sender, RoutedEventArgs e) => Meter.Value = Math.Min(Meter.Maximum, Meter.Value + 10);
    private void Reset_Click(object sender, RoutedEventArgs e) => Meter.Value = 0;
}
