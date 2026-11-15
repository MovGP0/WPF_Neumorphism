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
        Color highlight = NeutralAt(tone + (dark ? 26 : 10));
        Color shade = NeutralAt(tone - (dark ? 16 : 40));
        double highlightOpacity = highContrast ? 0.0 : dark ? 0.45 : 0.95;
        double shadeOpacity = highContrast ? 0.0 : dark ? 0.75 : 0.37;

        resources["Neumorphism.Color.Shade"] = shade;
        resources["Neumorphism.Opacity.Shade"] = shadeOpacity;
        resources["Neumorphism.Brush.ButtonDisabledText"] = highContrast
            ? SystemColors.GrayTextBrush
            : BrushFrom(WithOpacity(_colors.OnSurfaceVariant.GetColor(scheme), 0.64));
        resources["Neumorphism.Brush.RaisedEdge"] = highContrast
            ? SystemColors.WindowTextBrush
            : MakeGradient(new Point(1, 1), new GradientStopCollection
            {
                new(WithOpacity(highlight, dark ? 0.70 : 0.94), 0),
                new(WithOpacity(shade, dark ? 0.65 : 0.29), 1)
            });
        ReliefPaint[] palette = SurfaceElevation.Levels.Select(CreateReliefPaint).ToArray();
        resources[ReliefLayer.PaletteKey] = palette;
        resources["Neumorphism.Brush.InsetEdge"] = palette[SurfaceElevation.Level2.Level].InsetEdge;
        resources["Neumorphism.Brush.InsetFill"] = palette[SurfaceElevation.Level2.Level].InsetFill;

        Color NeutralAt(double value) =>
            ColorUtils.ColorFromArgb(scheme.NeutralPalette.Tone((int)Math.Clamp(Math.Round(value), 0, 100)));

        ReliefPaint CreateReliefPaint(SurfaceElevation level)
        {
            var lightEffect = MakeShadow(highlight, highlightOpacity, 135, level.HighlightDepth, level.HighlightBlur);
            var shadeEffect = MakeShadow(shade, shadeOpacity, 315, level.ShadeDepth, level.ShadeBlur);
            if (level.Level == 0)
                return new(lightEffect, shadeEffect, Brushes.Transparent, Brushes.Transparent);
            if (highContrast)
                return new(lightEffect, shadeEffect, SystemColors.HighlightBrush, Brushes.Transparent);

            double strength = level.InsetStrength;
            Brush edge = MakeGradient(new Point(1, 1), new GradientStopCollection
            {
                new(WithOpacity(shade, Math.Min(1, strength * (dark ? 0.72 : 0.30))), 0),
                new(Colors.Transparent, 0.52),
                new(WithOpacity(highlight, Math.Min(1, strength * (dark ? 0.62 : 0.90))), 1)
            });
            Brush fill = MakeGradient(new Point(0, 1), new GradientStopCollection
            {
                new(WithOpacity(shade, strength * (dark ? 0.35 : 0.19)), 0),
                new(WithOpacity(shade, 0), 0.28 * strength),
                new(WithOpacity(highlight, 0), 1 - 0.21 * strength),
                new(WithOpacity(highlight, strength * (dark ? 0.24 : 0.42)), 1)
            });
            return new(lightEffect, shadeEffect, edge, fill);
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

    private static LinearGradientBrush MakeGradient(Point endPoint, GradientStopCollection stops)
    {
        LinearGradientBrush brush = new(stops, new Point(0, 0), endPoint);
        brush.Freeze();
        return brush;
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
