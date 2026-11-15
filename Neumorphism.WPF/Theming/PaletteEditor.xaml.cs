using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialColorUtilities;
using Neumorphism.WPF.Theming;

namespace Neumorphism.WPF;

public partial class PaletteEditor : UserControl
{
    public Color Seed { get; private set; } = Color.FromRgb(0, 102, 102);
    public Variant SchemeVariant => Variants.SelectedValue is Variant variant ? variant : Variant.TonalSpot;

    public event EventHandler? ThemeChanged;

    public PaletteEditor()
    {
        InitializeComponent();
        Presets.ItemsSource = GalleryTheme.Presets;
        Variants.SelectedValuePath = nameof(VariantChoice.Value);
        Variants.ItemsSource = new VariantChoice[]
        {
            new("Tonal Spot", Variant.TonalSpot),
            new("Neutral", Variant.Neutral),
            new("Vibrant", Variant.Vibrant),
            new("Expressive", Variant.Expressive),
            new("Fidelity", Variant.Fidelity),
            new("Monochrome", Variant.Monochrome),
            new("Content", Variant.Content),
            new("Rainbow", Variant.Rainbow),
            new("Fruit Salad", Variant.FruitSalad)
        };
        Variants.SelectedValue = Variant.TonalSpot;
        SeedInput.Text = GalleryTheme.Hex(Seed);
    }

    public void SetSwatches(IReadOnlyList<PaletteSwatch> swatches) => Swatches.ItemsSource = swatches;

    private void Preset_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).DataContext is SeedPreset preset)
            ApplySeed(preset.Hex);
    }

    private void ApplySeed_Click(object sender, RoutedEventArgs e) => ApplySeed(SeedInput.Text);

    private void SeedInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        ApplySeed(SeedInput.Text);
        e.Handled = true;
    }

    private void SeedInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (SeedError is not null)
            SeedError.Visibility = Visibility.Collapsed;
    }

    private void Variants_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        ThemeChanged?.Invoke(this, EventArgs.Empty);

    private void ApplySeed(string value)
    {
        ReadOnlySpan<char> hex = value.AsSpan().Trim();
        if (hex.StartsWith("#", StringComparison.Ordinal))
            hex = hex[1..];

        if (hex.Length != 6 || !uint.TryParse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out uint rgb))
        {
            SeedError.Visibility = Visibility.Visible;
            var peer = UIElementAutomationPeer.FromElement(SeedError) ?? UIElementAutomationPeer.CreatePeerForElement(SeedError);
            peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
            SeedInput.Focus();
            return;
        }

        Seed = ColorUtils.ColorFromArgb(unchecked((int)(rgb | 0xff000000)));
        SeedInput.Text = GalleryTheme.Hex(Seed);
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public sealed record VariantChoice(string Name, Variant Value);
}
