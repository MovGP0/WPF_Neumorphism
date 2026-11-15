using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MaterialColorUtilities;

namespace Neumorphism.WPF.Theming;

public sealed record PaletteSwatch(string Role, Brush Fill, Brush Text, string Hex);
public sealed record SeedPreset(string Name, string Hex);

internal sealed class GalleryTheme(ResourceDictionary applicationResources)
{
    private const string BrushPrefix = "MaterialDesign.Brush.";
    private readonly MaterialDynamicColors _colors = new();
    private ResourceDictionary? _activeResources;

    public static SeedPreset[] Presets { get; } =
    [
        new("Teal", "#006666"),
        new("Clay", "#B46A46"),
        new("Iris", "#65558F"),
        new("Blue", "#36618E"),
        new("Moss", "#4A672C")
    ];

    public IReadOnlyList<PaletteSwatch> Apply(Color seed, Variant variant, bool dark, bool highContrast)
    {
        DynamicScheme scheme = DynamicSchemeFactory.Create(seed, variant, dark, 0, Platform.Phone, SpecVersion.Spec2025,
            primary: null, secondary: null, tertiary: null, neutral: null, neutralVariant: null, error: null);

        ResourceDictionary resources = new();
        (string Name, DynamicColor Color)[] roles =
        [
            (nameof(_colors.Surface), _colors.Surface),
            (nameof(_colors.SurfaceContainerLow), _colors.SurfaceContainerLow),
            (nameof(_colors.SurfaceContainer), _colors.SurfaceContainer),
            (nameof(_colors.SurfaceContainerHigh), _colors.SurfaceContainerHigh),
            (nameof(_colors.SurfaceContainerHighest), _colors.SurfaceContainerHighest),
            (nameof(_colors.OnSurface), _colors.OnSurface),
            (nameof(_colors.OnSurfaceVariant), _colors.OnSurfaceVariant),
            (nameof(_colors.Outline), _colors.Outline),
            (nameof(_colors.OutlineVariant), _colors.OutlineVariant),
            (nameof(_colors.Primary), _colors.Primary),
            (nameof(_colors.OnPrimary), _colors.OnPrimary),
            (nameof(_colors.PrimaryContainer), _colors.PrimaryContainer),
            (nameof(_colors.OnPrimaryContainer), _colors.OnPrimaryContainer),
            (nameof(_colors.Secondary), _colors.Secondary),
            (nameof(_colors.OnSecondary), _colors.OnSecondary),
            (nameof(_colors.SecondaryContainer), _colors.SecondaryContainer),
            (nameof(_colors.OnSecondaryContainer), _colors.OnSecondaryContainer),
            (nameof(_colors.Tertiary), _colors.Tertiary),
            (nameof(_colors.OnTertiary), _colors.OnTertiary),
            (nameof(_colors.TertiaryContainer), _colors.TertiaryContainer),
            (nameof(_colors.OnTertiaryContainer), _colors.OnTertiaryContainer),
            (nameof(_colors.Error), _colors.Error),
            (nameof(_colors.OnError), _colors.OnError),
            (nameof(_colors.ErrorContainer), _colors.ErrorContainer),
            (nameof(_colors.OnErrorContainer), _colors.OnErrorContainer)
        ];

        foreach (var (name, role) in roles)
            resources.Add(BrushPrefix + name, BrushFrom(role.GetColor(scheme)));

        if (highContrast)
            ApplySystemColors(resources);

        AddSurfaceColors(resources, scheme, dark, highContrast);
        DelayExamplePalette.Add(resources, scheme, highContrast);

        var dictionaries = applicationResources.MergedDictionaries;
        if (_activeResources is null)
            dictionaries.Add(resources);
        else
            dictionaries[dictionaries.IndexOf(_activeResources)] = resources;

        _activeResources = resources;
        return
        [
            Swatch(nameof(_colors.Primary), nameof(_colors.OnPrimary)),
            Swatch(nameof(_colors.Secondary), nameof(_colors.OnSecondary)),
            Swatch(nameof(_colors.Tertiary), nameof(_colors.OnTertiary)),
            Swatch(nameof(_colors.Error), nameof(_colors.OnError)),
            Swatch(nameof(_colors.PrimaryContainer), nameof(_colors.OnPrimaryContainer)),
            Swatch(nameof(_colors.SecondaryContainer), nameof(_colors.OnSecondaryContainer)),
            Swatch(nameof(_colors.TertiaryContainer), nameof(_colors.OnTertiaryContainer)),
            Swatch(nameof(_colors.ErrorContainer), nameof(_colors.OnErrorContainer)),
            Swatch(nameof(_colors.Surface), nameof(_colors.OnSurface)),
            Swatch(nameof(_colors.SurfaceContainerLow), nameof(_colors.OnSurface)),
            Swatch(nameof(_colors.SurfaceContainer), nameof(_colors.OnSurface)),
            Swatch(nameof(_colors.SurfaceContainerHigh), nameof(_colors.OnSurface))
        ];

        PaletteSwatch Swatch(string role, string foreground)
        {
            var fill = (SolidColorBrush)resources[BrushPrefix + role];
            return new(role, fill, (Brush)resources[BrushPrefix + foreground], Hex(fill.Color));
        }
    }

    public static string Hex(Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    private void AddSurfaceColors(ResourceDictionary resources, DynamicScheme scheme, bool dark, bool highContrast)
    {
        double tone = _colors.Surface.GetTone(scheme);
        Color highlight = NeutralAt(tone + (dark ? 31 : 10));
        Color shade = NeutralAt(tone - (dark ? 16 : 40));
        double highlightOpacity = highContrast ? 0.0 : dark ? 0.55 : 0.95;
        double shadeOpacity = highContrast ? 0.0 : dark ? 0.75 : 0.37;

        resources["Neumorphism.Color.Shade"] = shade;
        resources["Neumorphism.Opacity.Shade"] = shadeOpacity;
        resources["Neumorphism.Opacity.FocusGlow"] = highContrast ? 0.0 : 0.42;
        Color ledGlow = PrimaryAt(dark ? 77 : 54);
        var halo = new RadialGradientBrush(new GradientStopCollection
        {
            new(WithOpacity(ledGlow, dark ? 0.78 : 0.62), 0),
            new(WithOpacity(ledGlow, dark ? 0.28 : 0.20), 0.46),
            new(WithOpacity(ledGlow, 0), 1)
        });
        halo.Freeze();
        resources["Neumorphism.Brush.SelectionLedGlow"] = highContrast ? Brushes.Transparent : halo;
        resources["Neumorphism.Brush.SelectionLedLens"] = highContrast ? SystemColors.HighlightBrush
            : MakeGradient(new Point(0, 0), new Point(1, 1), new GradientStopCollection
            {
                new(PrimaryAt(dark ? 96 : 88), 0),
                new(PrimaryAt(dark ? 86 : 72), 0.37),
                new(PrimaryAt(dark ? 69 : 49), 1)
            });
        resources["Neumorphism.Effect.SelectionLedShadow"] = MakeShadow(
            shade, highContrast ? 0 : dark ? 0.90 : 0.55, 315, 1.4, 4.5);
        resources["Neumorphism.Brush.ButtonDisabledText"] = highContrast
            ? SystemColors.GrayTextBrush
            : BrushFrom(WithOpacity(_colors.OnSurfaceVariant.GetColor(scheme), 0.64));
        ReliefPaint[][] palette = Enum.GetValues<SurfaceLightDirection>().Select(CreateDirectionalPalette).ToArray();
        resources[ReliefLayer.PaletteKey] = palette;
        ReliefPaint standard = palette[(int)SurfaceLightDirection.Default][SurfaceElevation.Level2.Level];
        resources["Neumorphism.Brush.RaisedEdge"] = standard.RaisedEdge;
        resources["Neumorphism.Brush.InsetEdge"] = standard.InsetEdge;
        resources["Neumorphism.Brush.InsetFill"] = standard.InsetFill;

        Color NeutralAt(double value) =>
            ColorUtils.ColorFromArgb(scheme.NeutralPalette.Tone((int)Math.Clamp(Math.Round(value), 0, 100)));

        Color PrimaryAt(int value) => ColorUtils.ColorFromArgb(scheme.PrimaryPalette.Tone(value));

        ReliefPaint[] CreateDirectionalPalette(SurfaceLightDirection direction)
        {
            (double angle, Point start, Point end) = direction switch
            {
                SurfaceLightDirection.Side => (180, new Point(0, 0.5), new Point(1, 0.5)),
                SurfaceLightDirection.Top => (90, new Point(0.5, 0), new Point(0.5, 1)),
                _ => (135, new Point(0, 0), new Point(1, 1))
            };
            // A full-width linear ramp looks nearly horizontal on wide buttons.
            bool lightCorners = direction == SurfaceLightDirection.Diagonal;
            Color lightEdge = WithOpacity(highlight, dark ? 0.82 : 0.94);
            Color darkEdge = WithOpacity(shade, dark ? 0.65 : 0.29);
            Brush raisedEdge = highContrast ? SystemColors.WindowTextBrush
                : lightCorners ? MakeCornerLighting(lightEdge, darkEdge)
                : MakeGradient(start, end, new GradientStopCollection { new(lightEdge, 0), new(darkEdge, 1) });
            Brush raisedFill = lightCorners && !highContrast
                ? MakeCornerLighting(WithOpacity(highlight, dark ? 0.22 : 0.72), WithOpacity(shade, dark ? 0.45 : 0.15))
                : Brushes.Transparent;
            return SurfaceElevation.Levels.Select(CreateReliefPaint).ToArray();

            ReliefPaint CreateReliefPaint(SurfaceElevation level)
            {
                var lightEffect = MakeShadow(highlight, highlightOpacity, angle, level.HighlightDepth, level.HighlightBlur);
                var shadeEffect = MakeShadow(shade, shadeOpacity, (angle + 180) % 360, level.ShadeDepth, level.ShadeBlur);
                if (level.Level == 0)
                    return new(lightEffect, shadeEffect, Brushes.Transparent, Brushes.Transparent, Brushes.Transparent, Brushes.Transparent);
                if (highContrast)
                    return new(lightEffect, shadeEffect, raisedEdge, raisedFill, SystemColors.HighlightBrush, Brushes.Transparent);

                double strength = level.InsetStrength;
                Color insetDarkEdge = WithOpacity(shade, Math.Min(1, strength * (dark ? 0.72 : 0.30)));
                Color insetLightEdge = WithOpacity(highlight, Math.Min(1, strength * (dark ? 0.62 : 0.90)));
                Color insetShade = WithOpacity(shade, strength * (dark ? 0.35 : 0.19));
                Color insetLight = WithOpacity(highlight, strength * (dark ? 0.24 : 0.42));
                Brush edge = lightCorners ? MakeCornerLighting(insetDarkEdge, insetLightEdge)
                    : MakeGradient(start, end, new GradientStopCollection
                    {
                        new(insetDarkEdge, 0),
                        new(Colors.Transparent, 0.52),
                        new(insetLightEdge, 1)
                    });
                Brush fill = lightCorners ? MakeCornerLighting(insetShade, insetLight)
                    : MakeGradient(start, end, new GradientStopCollection
                    {
                        new(insetShade, 0),
                        new(WithOpacity(shade, 0), 0.28 * strength),
                        new(WithOpacity(highlight, 0), 1 - 0.21 * strength),
                        new(insetLight, 1)
                    });
                return new(lightEffect, shadeEffect, raisedEdge, raisedFill, edge, fill);
            }
        }
    }

    private static DropShadowEffect MakeShadow(Color color, double opacity, double direction, double depth, double blur)
    {
        DropShadowEffect effect = new()
        {
            Color = color,
            Opacity = opacity,
            Direction = direction,
            ShadowDepth = depth,
            BlurRadius = blur
        };
        effect.Freeze();
        return effect;
    }

    private static Color WithOpacity(Color color, double opacity) =>
        Color.FromArgb((byte)Math.Round(opacity * byte.MaxValue), color.R, color.G, color.B);

    private static LinearGradientBrush MakeGradient(Point startPoint, Point endPoint, GradientStopCollection stops)
    {
        LinearGradientBrush brush = new(stops, startPoint, endPoint);
        brush.Freeze();
        return brush;
    }

    private static DrawingBrush MakeCornerLighting(Color upperLeft, Color lowerRight)
    {
        var bounds = new RectangleGeometry(new Rect(0, 0, 1, 1));
        var drawing = new DrawingGroup();
        drawing.Children.Add(new GeometryDrawing(Corner(new Point(0, 0), upperLeft), null, bounds));
        drawing.Children.Add(new GeometryDrawing(Corner(new Point(1, 1), lowerRight), null, bounds));
        var brush = new DrawingBrush(drawing);
        brush.Freeze();
        return brush;

        static RadialGradientBrush Corner(Point origin, Color color) => new(color, color with { A = 0 })
        {
            Center = origin,
            GradientOrigin = origin,
            RadiusX = 0.62,
            RadiusY = 1
        };
    }

    private static SolidColorBrush BrushFrom(Color color)
    {
        SolidColorBrush brush = new(color);
        brush.Freeze();
        return brush;
    }

    private static void ApplySystemColors(ResourceDictionary resources)
    {
        Use(SystemColors.WindowBrush, "Surface", "SurfaceContainerLow", "SurfaceContainer",
            "SurfaceContainerHigh", "SurfaceContainerHighest");
        Use(SystemColors.WindowTextBrush, "OnSurface", "OnSurfaceVariant", "Outline", "OutlineVariant");
        Use(SystemColors.HighlightBrush, "Primary", "PrimaryContainer", "Secondary", "SecondaryContainer",
            "Tertiary", "TertiaryContainer", "Error", "ErrorContainer");
        Use(SystemColors.HighlightTextBrush, "OnPrimary", "OnPrimaryContainer", "OnSecondary", "OnSecondaryContainer",
            "OnTertiary", "OnTertiaryContainer", "OnError", "OnErrorContainer");

        void Use(Brush brush, params string[] roles)
        {
            foreach (string role in roles)
                resources[BrushPrefix + role] = brush;
        }
    }
}
