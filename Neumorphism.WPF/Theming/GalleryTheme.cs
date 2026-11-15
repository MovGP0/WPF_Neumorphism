using System.Windows;
using System.Windows.Media;
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
