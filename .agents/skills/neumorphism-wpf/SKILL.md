---
name: neumorphism-wpf
description: Design and implement Neumorphism styles, templates, and control-gallery pages in this Windows WPF repository. Use when adding or reviewing XAML controls, tactile surfaces, keyboard states, or Material Design 3 seed-based light and dark palettes.
---

# Neumorphism for WPF

Adapt the OpenDesign Neumorphism kit's soft, extruded surfaces to native WPF. Use the Material Design 3 demo as the control inventory and navigation reference. Build WPF styles and templates in this repository.

Build and run the gallery on Windows with the .NET 10 SDK or newer.

## Color roles

- Resolve colors through the gallery's seed-derived Material Design 3 scheme. Keep the seed, scheme variant, and light or dark mode as independent inputs. Regenerate the complete set of role brushes together when an input changes.
- Reference brushes with `DynamicResource` and use the `MaterialDesign.Brush.<Role>` naming convention. Use `Surface` or a `SurfaceContainer*` role for broad neutral surfaces, `OnSurface` for text, `OnSurfaceVariant` for secondary text, and `Outline` for functional boundaries.
- Pair accent fills with their matching foreground roles, such as `Primary` with `OnPrimary` and `PrimaryContainer` with `OnPrimaryContainer`. Pair validation fills and text with the corresponding `Error` roles. Use `Primary` for focus and selected accents.
- Derive the paired highlight and shade from the neutral tonal palette, starting from the surrounding surface tone. Use a consistent upper-left light source. Tune each mode separately so both edges remain visible, especially the highlight on dark surfaces.

## Surfaces and control templates

- Use a 4-DIP spacing rhythm with 8/12/16 DIP inside controls, 24/32 DIP for section spacing, and 14/22 DIP corner radii for control and panel surfaces. Leave room around raised surfaces for their shadows. Use a readable desktop sans-serif such as Segoe UI, with stronger size and weight for headings.
- Use shallow outer relief for raised controls and clipped inner edges for wells such as fields or tracks. Use depth selectively. Keep navigation, table rows, and dense lists flat, with a clear current-item marker. See the [WPF surface recipes and source review](references/surface-recipes.md) when building or evaluating a control template.
- For hardware-inspired examples and recreations, use the [faceplate, dial, and indicator recipes](references/surface-recipes.md#hardware-inspired-panels) and compare the rendered WPF control with the reference at the same scale.
- Preserve native control behavior, automation peers, and required template parts. Treat momentary press, persistent selection, and keyboard focus as independent states. Pair depth with a visible thumb, checkmark, outline, fill, or label where it communicates a state.
- Use keyboard navigation to check focus visibility and reading order. Measure text and functional boundaries against the actual rendered adjacent colors, including the accent fill. Use 4.5:1 for normal text and 3:1 for large text, meaningful non-text indicators, and focus outlines. Respect Windows high-contrast system brushes and reduced animation when custom effects would interfere.

## Workflow

1. Find the component family in the gallery catalog and inspect the existing theme resources before adding a template. Add controls or specimens when the requested work calls for them.
2. Add reusable styles to the theme dictionaries and place requested examples in the selected gallery group. Use WPF controls and automation labels for navigation and theme inputs.
3. Build `Neumorphism.slnx`. Exercise the changed controls with the keyboard and verify light and dark modes with at least two seeds. Check for clipped shadows, unreadable state changes, and contrast loss at increased display scaling.

The visual reference is the [OpenDesign Neumorphism kit](https://github.com/MovGP0/open-design/tree/59e4f2ae8/design-systems/neumorphism). Its `DESIGN.md`, `tokens.css`, and preview kits describe the visual intent.
