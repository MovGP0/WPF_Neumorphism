using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Neumorphism.WPF.Theming;

public sealed class RoundedViewport : Decorator
{
    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
        nameof(Radius), typeof(double), typeof(RoundedViewport),
        new PropertyMetadata(0d, (element, _) => ((RoundedViewport)element).UpdateClip()),
        value => value is double radius && double.IsFinite(radius) && radius >= 0);

    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        UpdateClip();
    }

    private void UpdateClip()
    {
        double radius = Math.Min(Radius, Math.Min(ActualWidth, ActualHeight) / 2);
        RectangleGeometry clip = new(new Rect(RenderSize), radius, radius);
        clip.Freeze();
        Clip = clip;
    }
}
