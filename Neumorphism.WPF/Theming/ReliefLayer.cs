using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Neumorphism.WPF.Theming;

public enum ReliefLayerKind
{
    Highlight,
    Shade,
    Inset
}

internal sealed record ReliefPaint(DropShadowEffect Highlight, DropShadowEffect Shade, Brush InsetEdge, Brush InsetFill);

public sealed class ReliefLayer : Border
{
    internal const string PaletteKey = "Neumorphism.Relief.Palette";

    public static readonly DependencyProperty ElevationProperty = DependencyProperty.Register(
        nameof(Elevation), typeof(SurfaceElevation), typeof(ReliefLayer),
        new PropertyMetadata(SurfaceElevation.Level0, Refresh), value => value is SurfaceElevation);

    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind), typeof(ReliefLayerKind), typeof(ReliefLayer),
        new PropertyMetadata(ReliefLayerKind.Highlight, Refresh));

    private static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
        "Palette", typeof(ReliefPaint[]), typeof(ReliefLayer), new PropertyMetadata(null, Refresh));

    public ReliefLayer()
    {
        IsHitTestVisible = false;
        SetResourceReference(PaletteProperty, PaletteKey);
    }

    public SurfaceElevation Elevation
    {
        get => (SurfaceElevation)GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    public ReliefLayerKind Kind
    {
        get => (ReliefLayerKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    private static void Refresh(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var layer = (ReliefLayer)sender;
        var palette = (ReliefPaint[]?)layer.GetValue(PaletteProperty);
        var paint = palette?[layer.Elevation.Level];

        if (e.Property == KindProperty && e.OldValue is ReliefLayerKind.Inset)
        {
            layer.InvalidateProperty(BackgroundProperty);
            layer.InvalidateProperty(BorderBrushProperty);
        }

        layer.Effect = layer.Kind switch
        {
            ReliefLayerKind.Highlight => paint?.Highlight,
            ReliefLayerKind.Shade => paint?.Shade,
            _ => null
        };

        if (layer.Kind == ReliefLayerKind.Inset)
        {
            layer.SetCurrentValue(BackgroundProperty, paint?.InsetFill);
            layer.SetCurrentValue(BorderBrushProperty, paint?.InsetEdge);
        }
    }
}
