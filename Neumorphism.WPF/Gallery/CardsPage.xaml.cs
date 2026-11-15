using System.Windows;
using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class CardsPage : UserControl
{
    public CardsPage()
    {
        InitializeComponent();
    }

    private void ClearNote_Click(object sender, RoutedEventArgs e)
    {
        Note.Clear();
        Note.Focus();
    }
}
