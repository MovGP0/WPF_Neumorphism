using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MaterialColorUtilities;

namespace Neumorphism.WPF.Theming;

internal static class DelayExamplePalette
{
    private const string Prefix = "DelayExample.Brush.";

    public static void Add(ResourceDictionary resources, DynamicScheme scheme, bool highContrast)
    {
        var metal = TonalPalette.FromHueAndChroma(scheme.NeutralPalette.GetHue(), 2);
        var backdrop = TonalPalette.FromHueAndChroma(239, 7);

        Put("Backdrop", Vertical(backdrop, (0, 79), (0.58, 78), (1, 68)), SystemColors.WindowBrush);
        Put("Face", Vertical(metal, (0, 6), (0.53, 2), (1, 4)), SystemColors.WindowBrush);
        Put("Deck", Vertical(metal, (0, 25), (0.06, 21), (0.16, 27), (0.62, 17), (0.94, 11), (1, 5)),
            SystemColors.WindowBrush);
        Put("DeckLight", new RadialGradientBrush(new GradientStopCollection
        {
            new(ColorUtils.ColorFromArgb(metal.Tone(46)) with { A = 138 }, 0),
            new(ColorUtils.ColorFromArgb(metal.Tone(31)) with { A = 75 }, 0.51),
            new(Colors.Transparent, 1)
        }), Brushes.Transparent);
        Put("LowerLip", Vertical(metal, (0, 16), (0.24, 12), (0.63, 7), (1, 3)), Brushes.Transparent);
        Put("Bay", Vertical(metal, (0, 13), (0.4, 9), (1, 6)), SystemColors.WindowBrush);
        Put("DialBevel", Vertical(metal, (0, 37), (0.22, 30), (0.7, 14), (1, 8)), SystemColors.WindowBrush);
        Put("DialFace", Vertical(metal, (0, 24), (0.65, 20), (1, 14)), SystemColors.WindowBrush);
        Put("LampSocket", Vertical(metal, (0, 4), (0.48, 12), (1, 39)), SystemColors.WindowTextBrush);
        Put("LampCap", Vertical(metal, (0, 37), (0.38, 23), (1, 11)), SystemColors.WindowBrush);
        Put("Selected", Vertical(metal, (0, 66), (1, 48)), SystemColors.HighlightBrush);
        Put("OnSelected", Tone(metal, 2), SystemColors.HighlightTextBrush);
        Put("Well", Tone(metal, 2), SystemColors.WindowBrush);
        Put("Edge", Tone(metal, 33), SystemColors.WindowTextBrush);
        Put("Rule", Tone(metal, 20), SystemColors.WindowTextBrush);
        Put("Text", Tone(metal, 88), SystemColors.WindowTextBrush);
        Put("Muted", Tone(metal, 69), SystemColors.WindowTextBrush);

        AddShadow("DialHighlight", 52, 135, 1.7, 7, 0.44);
        AddShadow("DialShade", 0, 315, 2.6, 6, 0.9);
        AddShadow("LampHighlight", 56, 135, 1.4, 6, 0.48);
        AddShadow("LampShade", 0, 315, 1.7, 4, 0.95);

        AddMode("Air", 257);
        AddMode("Magnetic", scheme.PrimaryPalette.GetHue() + 85);
        AddMode("Circuit", scheme.SecondaryPalette.GetHue() + 135);
        AddMode("Aether", scheme.TertiaryPalette.GetHue() + 45);
        AddMode("Reverse", scheme.PrimaryPalette.GetHue());

        void AddMode(string name, double hue)
        {
            var accent = TonalPalette.FromHueAndChroma(hue, 68);
            Put(name + "Accent", Tone(accent, 70), SystemColors.HighlightBrush);
            Put(name + "Core", Tone(accent, 91), SystemColors.WindowTextBrush);
            Put(name + "Spill", new RadialGradientBrush(new GradientStopCollection
            {
                new(ColorUtils.ColorFromArgb(accent.Tone(54)) with { A = 150 }, 0),
                new(ColorUtils.ColorFromArgb(accent.Tone(34)) with { A = 46 }, 0.48),
                new(Colors.Transparent, 1)
            }), Brushes.Transparent);
        }

        void Put(string name, Brush brush, Brush systemBrush)
        {
            if (highContrast)
            {
                resources[Prefix + name] = systemBrush;
                return;
            }

            brush.Freeze();
            resources[Prefix + name] = brush;
        }

        void AddShadow(string name, int tone, double direction, double depth, double blur, double opacity)
        {
            var shadow = new DropShadowEffect
            {
                Color = ColorUtils.ColorFromArgb(metal.Tone(tone)),
                Direction = direction,
                ShadowDepth = depth,
                BlurRadius = blur,
                Opacity = highContrast ? 0 : opacity
            };
            shadow.Freeze();
            resources["DelayExample.Effect." + name] = shadow;
        }
    }

    private static SolidColorBrush Tone(TonalPalette palette, int tone) =>
        new(ColorUtils.ColorFromArgb(palette.Tone(tone)));

    private static LinearGradientBrush Vertical(TonalPalette palette, params (double Offset, int Tone)[] stops) =>
        new(new GradientStopCollection(stops.Select(stop =>
            new GradientStop(ColorUtils.ColorFromArgb(palette.Tone(stop.Tone)), stop.Offset))),
            new Point(0, 0), new Point(0, 1));
}
