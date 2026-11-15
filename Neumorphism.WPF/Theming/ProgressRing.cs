using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Neumorphism.WPF.Theming;

public sealed class ProgressRing : ProgressBar
{
    public ProgressRing()
    {
        OverridesDefaultStyle = true;
        SetResourceReference(ForegroundProperty, "MaterialDesign.Brush.Primary");
        SetResourceReference(BackgroundProperty, "MaterialDesign.Brush.SurfaceContainerHighest");
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == ValueProperty || e.Property == MinimumProperty || e.Property == MaximumProperty ||
            e.Property == ForegroundProperty || e.Property == BackgroundProperty || e.Property == IsIndeterminateProperty)
        {
            InvalidateVisual();
        }
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        const double thickness = 7;
        Point center = new(ActualWidth / 2, ActualHeight / 2);
        double radius = Math.Min(center.X, center.Y) - thickness / 2 - 1;
        if (radius <= 0)
            return;

        drawingContext.DrawEllipse(null, new Pen(Background, thickness), center, radius, radius);
        double fraction = IsIndeterminate ? 0.25 : Maximum > Minimum ? (Value - Minimum) / (Maximum - Minimum) : 1;
        if (fraction >= 1)
        {
            drawingContext.DrawEllipse(null, new Pen(Foreground, thickness), center, radius, radius);
        }
        else if (fraction > 0)
        {
            double angle = fraction * Math.Tau;
            Point start = new(center.X, center.Y - radius);
            Point end = new(center.X + radius * Math.Sin(angle), center.Y - radius * Math.Cos(angle));
            StreamGeometry arc = new();
            using (var path = arc.Open())
            {
                path.BeginFigure(start, false, false);
                path.ArcTo(end, new Size(radius, radius), 0, fraction > 0.5, SweepDirection.Clockwise, true, false);
            }
            drawingContext.DrawGeometry(null, new Pen(Foreground, thickness)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            }, arc);
        }
    }
}
