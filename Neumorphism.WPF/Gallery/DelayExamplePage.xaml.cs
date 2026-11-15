using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace Neumorphism.WPF.Gallery;

public partial class DelayExamplePage : UserControl
{
    private static readonly string[] Modes = ["Air", "Magnetic", "Circuit", "Aether", "Reverse"];
    private static readonly (string Label, double Seconds)[] Divisions =
        [("1/16", 0.125), ("1/8", 0.25), ("1/4", 0.5), ("1/2", 1), ("1/1", 2)];
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(40) };
    private readonly Stopwatch _clock = new();
    private int _mode;
    private int _stepCount = 5;
    private bool _postDucker = true;

    public static readonly DependencyProperty AccentProperty = DependencyProperty.Register(
        nameof(Accent), typeof(Brush), typeof(DelayExamplePage));
    public static readonly DependencyProperty CoreProperty = DependencyProperty.Register(
        nameof(Core), typeof(Brush), typeof(DelayExamplePage));
    public static readonly DependencyProperty SpillProperty = DependencyProperty.Register(
        nameof(Spill), typeof(Brush), typeof(DelayExamplePage));

    public Brush Accent => (Brush)GetValue(AccentProperty);
    public Brush Core => (Brush)GetValue(CoreProperty);
    public Brush Spill => (Brush)GetValue(SpillProperty);

    public DelayExamplePage()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            ApplyMode();
            UpdatePattern();
            UpdateReadouts();
            UpdateFilterCurve();
            UpdateTransport();
        };
        Unloaded += (_, _) => StopPreview();
        IsVisibleChanged += (_, _) => UpdateTransport();
        _timer.Tick += (_, _) => Tick();
    }

    private void Dial_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!IsLoaded)
            return;

        if (ReferenceEquals(sender, DelayTime) || ReferenceEquals(sender, Pan))
            UpdateReadouts();
        if (ReferenceEquals(sender, LowCut) || ReferenceEquals(sender, HighCut))
            UpdateFilterCurve();
        DrawContour();
    }

    private void PreviousMode_Click(object sender, RoutedEventArgs e) => ChangeMode(-1);
    private void NextMode_Click(object sender, RoutedEventArgs e) => ChangeMode(1);

    private void ChangeMode(int delta)
    {
        _mode = (_mode + delta + Modes.Length) % Modes.Length;
        ApplyMode();
        _clock.Reset();
        UpdateTransport();
        DrawContour();
    }

    private void Bypass_Changed(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded)
            return;

        ApplyMode();
        UpdateTransport();
    }

    private void ApplyMode()
    {
        string mode = Modes[_mode];
        ModeName.Text = mode.ToUpperInvariant();
        if (Bypass.IsChecked == true)
        {
            SetResourceReference(AccentProperty, "DelayExample.Brush.Muted");
            SetResourceReference(CoreProperty, "DelayExample.Brush.Text");
            SetValue(SpillProperty, Brushes.Transparent);
        }
        else
        {
            SetResourceReference(AccentProperty, $"DelayExample.Brush.{mode}Accent");
            SetResourceReference(CoreProperty, $"DelayExample.Brush.{mode}Core");
            SetResourceReference(SpillProperty, $"DelayExample.Brush.{mode}Spill");
        }
        DrawContour();
    }

    private void Pattern_Changed(object sender, RoutedEventArgs e)
    {
        if (IsLoaded)
            UpdatePattern();
    }

    private void StepCount_Click(object sender, RoutedEventArgs e)
    {
        _stepCount = _stepCount % Steps.Children.Count + 1;
        UpdatePattern();
    }

    private void UpdatePattern()
    {
        bool sequence = SequencePattern.IsChecked == true;
        StepCount.IsEnabled = sequence;
        StepCount.Content = $"{_stepCount} ↕";
        for (int step = 0; step < Steps.Children.Count; step++)
            ((ToggleButton)Steps.Children[step]).IsEnabled = sequence && step < _stepCount;

        DrawContour();
    }

    private void Sync_Changed(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded)
            return;

        UpdateReadouts();
        DrawContour();
    }

    private void UpdateReadouts()
    {
        bool synced = Sync.IsChecked == true;
        DelayTime.ReadoutOverride = synced ? Divisions[DivisionIndex].Label : null;
        DelayTime.Unit = synced ? string.Empty : "ms";
        DelayTime.SmallChange = synced ? 160 : 10;
        DelayTime.LargeChange = synced ? 160 : 60;
        Pan.ReadoutOverride = Pan.Value == 0 ? "C" : $"{Math.Abs(Pan.Value):0}{(Pan.Value < 0 ? "L" : "R")}";
    }

    private void Filter_Changed(object sender, RoutedEventArgs e)
    {
        if (IsLoaded)
            DrawContour();
    }

    private void Target_Click(object sender, RoutedEventArgs e)
    {
        _postDucker = !_postDucker;
        Target.Content = _postDucker ? "POST  ⌄" : "PRE  ⌄";
        DrawContour();
    }

    private void Transport_Changed(object sender, RoutedEventArgs e) => UpdateTransport();

    private void UpdateTransport()
    {
        if (!IsLoaded)
            return;

        bool playing = IsVisible && SystemParameters.ClientAreaAnimation &&
            Bypass.IsChecked != true && Freeze.IsChecked != true;
        if (playing)
        {
            if (_clock.Elapsed.TotalSeconds >= CycleDuration && Loop.IsChecked != true)
                _clock.Reset();

            _clock.Start();
            _timer.Start();
        }
        else
        {
            StopPreview();
        }
    }

    private void StopPreview()
    {
        _timer.Stop();
        _clock.Stop();
    }

    private int DivisionIndex => Math.Clamp((int)Math.Round((DelayTime.Value - 80) / 160), 0, Divisions.Length - 1);

    private double EchoPeriod => (Sync.IsChecked == true ? Divisions[DivisionIndex].Seconds : DelayTime.Value / 1000)
        + Offset.Value / 1000;

    private double CycleDuration => Math.Max(1, EchoPeriod * _stepCount);

    private void Tick()
    {
        if (!SystemParameters.ClientAreaAnimation ||
            (Loop.IsChecked != true && _clock.Elapsed.TotalSeconds >= CycleDuration))
        {
            StopPreview();
            return;
        }

        DrawContour();
    }

    private void DrawContour()
    {
        if (!IsLoaded)
            return;

        const int samples = 112;
        double period = EchoPeriod;
        double time = _clock.Elapsed.TotalSeconds;
        // Sample the visible pattern to give each active echo a matching pulse.
        int step = (int)(time / period) % _stepCount;
        bool sequence = SequencePattern.IsChecked == true;
        bool stepEnabled = !sequence || ((ToggleButton)Steps.Children[step]).IsChecked == true;
        double swing = (Swing.Value - 50) / 180;
        double age = Math.Clamp(time / period % 1 - (step % 2 == 0 ? 0 : swing), 0, 1);
        double pulse = stepEnabled ? Math.Exp(-age * (4.8 - Feedback.Value / 28)) : 0;
        double sidechain = Sidechain.IsChecked == true
            ? DuckerAmount.Value / 130 * Math.Exp(-age * (1 + DuckerRelease.Value / 25))
            : 0;
        pulse *= 1 - sidechain * (_postDucker ? 1 : 0.55);

        double rotation = time * ModRate.Value * (0.3 + Modulation.Value / 110);
        double spread = Diffusion.Value / 100;
        double damping = (LowDamping.Value * LowDampFrequency.Value / 700 +
            HighDamping.Value * HighDampFrequency.Value / 20) / 200;
        double filter = (1 - LowCut.Value / 3500) * (0.65 + HighCut.Value / 57);
        double radius = 53 + 5 * pulse + Mix.Value / 35;
        double width = 0.86 + WidthDial.Value / 650;
        double centerX = 88 + Pan.Value / 12;
        bool bypassed = Bypass.IsChecked == true;
        int echoes = bypassed ? 1 : 5;

        var geometry = new StreamGeometry();
        using (StreamGeometryContext contour = geometry.Open())
        {
            for (int echo = 0; echo < echoes; echo++)
            {
                double phase = rotation + echo * (0.54 + spread * 0.34);
                double attenuation = 1 - echo * (0.016 + damping * (PreFilter.IsChecked == true ? 0.012 : 0.03));
                for (int i = 0; i < samples; i++)
                {
                    double angle = i * Math.Tau / samples;
                    double shape = _mode switch
                    {
                        1 => 0.12 * Math.Sin(2 * angle + phase) + 0.055 * Math.Cos(6 * angle - phase),
                        2 => 0.12 * Math.Cos(4 * angle + phase) + 0.028 * Math.Cos(8 * angle + phase),
                        3 => 0.17 * Math.Cos(3 * angle + phase) + 0.075 * Math.Sin(5 * angle - phase),
                        4 => 0.18 * Math.Sin(2 * angle - phase) + 0.09 * Math.Cos(3 * angle + phase),
                        _ => 0.075 * Math.Cos(2 * angle + 1.25 * phase) + 0.104 * Math.Sin(3 * angle - 0.9 * phase)
                    };
                    double displacement = radius * attenuation * (1 + (bypassed ? 0 : shape * filter));
                    double offset = (echo - 2) * (4.6 + spread * 2);
                    var point = new Point(centerX + Math.Sin(phase) * 2.5 + displacement * Math.Cos(angle) * width,
                        81 + displacement * Math.Sin(angle) * 1.12 + offset);
                    if (i == 0)
                        contour.BeginFigure(point, false, true);
                    else
                        contour.LineTo(point, true, false);
                }
            }
        }
        geometry.Freeze();
        ContourGlow.Data = geometry;
    }

    private void UpdateFilterCurve()
    {
        double low = Math.Log10(LowCut.Value / 20) / 3 * 102 + 11;
        double high = Math.Log10(HighCut.Value * 1000 / 20) / 3 * 100 + 28;
        double knee = Math.Min(low + 13, high - 5);
        var curve = new StreamGeometry();
        using (StreamGeometryContext path = curve.Open())
        {
            path.BeginFigure(new Point(Math.Max(2, low - 16), 41), false, false);
            path.BezierTo(new Point(low - 9, 36), new Point(low - 2, 17), new Point(knee, 17), true, false);
            path.LineTo(new Point(high, 17), true, false);
            path.QuadraticBezierTo(new Point(Math.Min(131, high + 8), 17),
                new Point(Math.Min(131, high + 18), high < 113 ? 41 : 26), true, false);
        }
        curve.Freeze();
        FilterCurve.Data = curve;
    }
}
