using System.Windows;
using System.Windows.Media;

namespace Neumorphism.WPF.Theming;

public enum SurfaceRelief
{
    Flat,
    Raised,
    Recessed
}

public enum SurfaceLightDirection
{
    Diagonal,
    Side,
    Top
}

public static class SurfaceChrome
{
    public static readonly DependencyProperty FocusBrushProperty = DependencyProperty.RegisterAttached(
        "FocusBrush", typeof(Brush), typeof(SurfaceChrome));

    public static readonly DependencyProperty ReliefProperty = DependencyProperty.RegisterAttached(
        "Relief", typeof(SurfaceRelief), typeof(SurfaceChrome), new PropertyMetadata(SurfaceRelief.Flat));

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
        "CornerRadius", typeof(CornerRadius), typeof(SurfaceChrome), new PropertyMetadata(default(CornerRadius)));

    public static readonly DependencyProperty ElevationProperty = DependencyProperty.RegisterAttached(
        "Elevation", typeof(SurfaceElevation), typeof(SurfaceChrome), new PropertyMetadata(SurfaceElevation.Level0),
        value => value is SurfaceElevation);

    public static readonly DependencyProperty LightDirectionProperty = DependencyProperty.RegisterAttached(
        "LightDirection", typeof(SurfaceLightDirection), typeof(SurfaceChrome), new PropertyMetadata(SurfaceLightDirection.Diagonal),
        value => Enum.IsDefined((SurfaceLightDirection)value));

    public static SurfaceRelief GetRelief(DependencyObject element) => (SurfaceRelief)element.GetValue(ReliefProperty);
    public static void SetRelief(DependencyObject element, SurfaceRelief value) => element.SetValue(ReliefProperty, value);

    public static CornerRadius GetCornerRadius(DependencyObject element) => (CornerRadius)element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(DependencyObject element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    public static SurfaceElevation GetElevation(DependencyObject element) => (SurfaceElevation)element.GetValue(ElevationProperty);
    public static void SetElevation(DependencyObject element, SurfaceElevation value) => element.SetValue(ElevationProperty, value);

    public static SurfaceLightDirection GetLightDirection(DependencyObject element) =>
        (SurfaceLightDirection)element.GetValue(LightDirectionProperty);
    public static void SetLightDirection(DependencyObject element, SurfaceLightDirection value) =>
        element.SetValue(LightDirectionProperty, value);

    public static Brush? GetFocusBrush(DependencyObject element) => (Brush?)element.GetValue(FocusBrushProperty);
    public static void SetFocusBrush(DependencyObject element, Brush? value) => element.SetValue(FocusBrushProperty, value);
}
