using System.Windows;

namespace Neumorphism.WPF.Theming;

public enum SurfaceRelief
{
    Flat,
    Raised,
    Recessed
}

public static class SurfaceChrome
{
    public static readonly DependencyProperty ReliefProperty = DependencyProperty.RegisterAttached(
        "Relief", typeof(SurfaceRelief), typeof(SurfaceChrome), new PropertyMetadata(SurfaceRelief.Flat));

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
        "CornerRadius", typeof(CornerRadius), typeof(SurfaceChrome), new PropertyMetadata(default(CornerRadius)));

    public static SurfaceRelief GetRelief(DependencyObject element) => (SurfaceRelief)element.GetValue(ReliefProperty);
    public static void SetRelief(DependencyObject element, SurfaceRelief value) => element.SetValue(ReliefProperty, value);

    public static CornerRadius GetCornerRadius(DependencyObject element) => (CornerRadius)element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(DependencyObject element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);
}
