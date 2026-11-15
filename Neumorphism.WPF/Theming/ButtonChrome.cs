using System.Windows;

namespace Neumorphism.WPF.Theming;

public enum ButtonRelief
{
    Flat,
    Raised,
    Recessed
}

public static class ButtonChrome
{
    public static readonly DependencyProperty ReliefProperty = DependencyProperty.RegisterAttached(
        "Relief", typeof(ButtonRelief), typeof(ButtonChrome), new PropertyMetadata(ButtonRelief.Raised));

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
        "CornerRadius", typeof(CornerRadius), typeof(ButtonChrome), new PropertyMetadata(new CornerRadius(14)));

    public static ButtonRelief GetRelief(DependencyObject element) => (ButtonRelief)element.GetValue(ReliefProperty);
    public static void SetRelief(DependencyObject element, ButtonRelief value) => element.SetValue(ReliefProperty, value);

    public static CornerRadius GetCornerRadius(DependencyObject element) => (CornerRadius)element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(DependencyObject element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);
}
