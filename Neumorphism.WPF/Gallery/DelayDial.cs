using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Neumorphism.WPF.Gallery;

public sealed class DelayDial : Slider
{
    private const double Sweep = 270;
    private const double RimRadius = 31.5;
    private const double ScaleRadius = 36.5;
    private const double LightRadius = 1.55;
    private const int ScaleLightCount = 34;
    private const int LitLightCount = 6;
    private static readonly Point Center = new(32, 32);
    private static readonly DependencyPropertyKey AnglePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(Angle), typeof(double), typeof(DelayDial), new PropertyMetadata(-Sweep / 2));
    private static readonly DependencyPropertyKey ArcPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(Arc), typeof(Geometry), typeof(DelayDial), new PropertyMetadata(Geometry.Empty));
    private static readonly DependencyPropertyKey ScaleLightsPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ScaleLights), typeof(Geometry), typeof(DelayDial), new PropertyMetadata(Geometry.Empty));

    public static readonly DependencyProperty AngleProperty = AnglePropertyKey.DependencyProperty;
    public static readonly DependencyProperty ArcProperty = ArcPropertyKey.DependencyProperty;
    public static readonly DependencyProperty ScaleLightsProperty = ScaleLightsPropertyKey.DependencyProperty;
    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
        nameof(Unit), typeof(string), typeof(DelayDial), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty ReadoutOverrideProperty = DependencyProperty.Register(
        nameof(ReadoutOverride), typeof(string), typeof(DelayDial));
    public static readonly DependencyProperty ShowAccentProperty = DependencyProperty.Register(
        nameof(ShowAccent), typeof(bool), typeof(DelayDial), new PropertyMetadata(true));
    public static readonly DependencyProperty ShowScaleProperty = DependencyProperty.Register(
        nameof(ShowScale), typeof(bool), typeof(DelayDial),
        new PropertyMetadata(false, (dial, _) => ((DelayDial)dial).UpdateDial()));

    public static Geometry Rim { get; } = Freeze(new EllipseGeometry(Center, RimRadius, RimRadius));
    public static Geometry ScaleSockets { get; } = CreateScale(0, ScaleLightCount);

    static DelayDial() => MaximumProperty.OverrideMetadata(typeof(DelayDial), new FrameworkPropertyMetadata(100d));

    private Point _dragStart;
    private double _dragValue;

    public double Angle => (double)GetValue(AngleProperty);
    public Geometry Arc => (Geometry)GetValue(ArcProperty);
    public Geometry ScaleLights => (Geometry)GetValue(ScaleLightsProperty);
    public bool ShowAccent
    {
        get => (bool)GetValue(ShowAccentProperty);
        set => SetValue(ShowAccentProperty, value);
    }
    public bool ShowScale
    {
        get => (bool)GetValue(ShowScaleProperty);
        set => SetValue(ShowScaleProperty, value);
    }
    public string? ReadoutOverride
    {
        get => (string?)GetValue(ReadoutOverrideProperty);
        set => SetValue(ReadoutOverrideProperty, value);
    }
    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
        base.OnMinimumChanged(oldMinimum, newMinimum);
        UpdateDial();
    }

    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
        base.OnMaximumChanged(oldMaximum, newMaximum);
        UpdateDial();
    }

    protected override void OnValueChanged(double oldValue, double newValue)
    {
        base.OnValueChanged(oldValue, newValue);
        UpdateDial();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (!IsEnabled || !CaptureMouse())
            return;

        Focus();
        _dragStart = e.GetPosition(this);
        _dragValue = Value;
        e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!IsMouseCaptured || e.LeftButton != MouseButtonState.Pressed)
            return;

        double distance = _dragStart.Y - e.GetPosition(this).Y;
        double value = _dragValue + distance * (Maximum - Minimum) / 120;
        if (IsSnapToTickEnabled && TickFrequency > 0)
            value = Minimum + Math.Round((value - Minimum) / TickFrequency) * TickFrequency;

        SetCurrentValue(ValueProperty, Math.Clamp(value, Minimum, Maximum));
        e.Handled = true;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        if (!IsMouseCaptured)
            return;

        ReleaseMouseCapture();
        e.Handled = true;
    }

    private void UpdateDial()
    {
        double progress = Maximum > Minimum ? (Value - Minimum) / (Maximum - Minimum) : 0;
        double travelled = progress * Sweep;
        double angle = travelled - Sweep / 2;
        SetValue(AnglePropertyKey, angle);

        int litThrough = (int)Math.Round(progress * (ScaleLightCount - 1)) + 1;
        int litCount = Math.Min(litThrough, LitLightCount);
        SetValue(ScaleLightsPropertyKey, ShowScale && progress > 0
            ? CreateScale(litThrough - litCount, litCount)
            : Geometry.Empty);
        SetValue(ArcPropertyKey, ShowScale || travelled == 0 ? Geometry.Empty : CreateArc(angle, travelled));
    }

    private static Geometry CreateArc(double angle, double travelled)
    {
        var arc = new StreamGeometry();
        using (StreamGeometryContext geometry = arc.Open())
        {
            geometry.BeginFigure(PointAt(-Sweep / 2, RimRadius), false, false);
            geometry.ArcTo(PointAt(angle, RimRadius), new Size(RimRadius, RimRadius), 0, travelled > 180,
                SweepDirection.Clockwise, true, false);
        }
        return Freeze(arc);
    }

    private static Geometry CreateScale(int start, int count)
    {
        var dots = new GeometryGroup();
        for (int index = start; index < start + count; index++)
        {
            double angle = -Sweep / 2 + index * Sweep / (ScaleLightCount - 1);
            dots.Children.Add(new EllipseGeometry(PointAt(angle, ScaleRadius), LightRadius, LightRadius));
        }
        return Freeze(dots);
    }

    private static Point PointAt(double degrees, double radius)
    {
        double radians = degrees * Math.PI / 180;
        return new Point(Center.X + radius * Math.Sin(radians), Center.Y - radius * Math.Cos(radians));
    }

    private static Geometry Freeze(Geometry geometry)
    {
        geometry.Freeze();
        return geometry;
    }
}
