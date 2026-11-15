namespace Neumorphism.WPF.Theming;

public sealed class SurfaceElevation
{
    private SurfaceElevation(int level, string description,
        double highlightDepth, double highlightBlur, double shadeDepth, double shadeBlur, double insetStrength)
    {
        Level = level;
        Description = description;
        HighlightDepth = highlightDepth;
        HighlightBlur = highlightBlur;
        ShadeDepth = shadeDepth;
        ShadeBlur = shadeBlur;
        InsetStrength = insetStrength;
    }

    public int Level { get; }
    public string Description { get; }
    public double HighlightDepth { get; }
    public double HighlightBlur { get; }
    public double ShadeDepth { get; }
    public double ShadeBlur { get; }
    public double InsetStrength { get; }

    public static SurfaceElevation Level0 { get; } = new(0, "Flat", 0, 0, 0, 0, 0);
    public static SurfaceElevation Level1 { get; } = new(1, "Subtle", 1, 5, 2, 6, 0.55);
    public static SurfaceElevation Level2 { get; } = new(2, "Low", 3, 9, 4, 10, 1);
    public static SurfaceElevation Level3 { get; } = new(3, "Medium", 4, 13, 5, 14, 1.25);
    public static SurfaceElevation Level4 { get; } = new(4, "High", 5, 17, 7, 19, 1.5);
    public static SurfaceElevation Level5 { get; } = new(5, "Maximum", 7, 22, 9, 24, 1.8);

    public static IReadOnlyList<SurfaceElevation> Levels { get; } =
        Array.AsReadOnly(new[] { Level0, Level1, Level2, Level3, Level4, Level5 });
}
