using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class PickersPage : UserControl
{
    public PickersPage()
    {
        InitializeComponent();
        Date.SelectedDate = DateTime.Today;
        Month.SelectedDate = DateTime.Today;
        Hour.ItemsSource = Enumerable.Range(0, 24).Select(hour => hour.ToString("00")).ToArray();
        Minute.ItemsSource = Enumerable.Range(0, 60).Select(minute => minute.ToString("00")).ToArray();
        Hour.SelectedIndex = 9;
        Minute.SelectedIndex = 30;
    }
}
