using System.Windows;
using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class RatingBarPage : UserControl
{
    public RatingBarPage()
    {
        InitializeComponent();
        Rating.ItemsSource = Enumerable.Range(1, 5).ToArray();
        Rating.SelectedIndex = 2;
        Review.ItemsSource = Enumerable.Range(1, 5).ToArray();
        Review.SelectedIndex = 3;
    }

    private void Clear_Click(object sender, RoutedEventArgs e) => Rating.SelectedIndex = -1;
}
