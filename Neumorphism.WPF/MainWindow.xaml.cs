using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Neumorphism.WPF.Gallery;
using Neumorphism.WPF.Theming;

namespace Neumorphism.WPF;

public partial class MainWindow : Window
{
    private readonly GalleryTheme _theme;
    private readonly Dictionary<string, UserControl> _componentPages = [];

    public MainWindow()
    {
        InitializeComponent();
        _theme = new(Application.Current.Resources);
        PaletteToggle.Checked += PaletteToggle_Changed;
        PaletteToggle.Unchecked += PaletteToggle_Changed;
        DarkModeToggle.Checked += DarkModeToggle_Changed;
        DarkModeToggle.Unchecked += DarkModeToggle_Changed;
        RefreshTheme();
        FilterCatalog();
        SystemParameters.StaticPropertyChanged += SystemParameters_Changed;
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        PaletteToggle.IsChecked = false;
        FilterCatalog();
    }

    private void FilterCatalog()
    {
        string query = SearchBox.Text.Trim();
        var previousGroup = Groups.SelectedItem as string;
        IReadOnlyList<string> matches = query.Length == 0
            ? GalleryCatalog.Groups
            : GalleryCatalog.Groups.Where(group => group.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();

        Groups.ItemsSource = matches;
        Groups.SelectedItem = previousGroup is not null && matches.Contains(previousGroup)
            ? previousGroup
            : matches.FirstOrDefault();
        ShowSelectedGroup();
    }

    private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PaletteToggle.IsChecked = false;
        ShowSelectedGroup();
    }

    private void ShowSelectedGroup()
    {
        var group = Groups.SelectedItem as string;
        GroupHeading.Text = group ?? "Search results";
        NoResults.Visibility = group is null ? Visibility.Visible : Visibility.Collapsed;
        ButtonsGallery.Visibility = group == GalleryCatalog.Buttons ? Visibility.Visible : Visibility.Collapsed;
        CardsGallery.Visibility = group == GalleryCatalog.Cards ? Visibility.Visible : Visibility.Collapsed;
        ElevationGallery.Visibility = group == GalleryCatalog.Elevation ? Visibility.Visible : Visibility.Collapsed;
        ComponentGallery.Content = FindComponentPage(group);
        if (group is not null)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (Equals(Groups.SelectedItem, group) &&
                    Groups.ItemContainerGenerator.ContainerFromItem(group) is ListBoxItem item)
                {
                    item.BringIntoView();
                }
            }, DispatcherPriority.Loaded);
        }
    }

    private UserControl? FindComponentPage(string? group)
    {
        if (group is null)
            return null;

        if (_componentPages.TryGetValue(group, out UserControl? page))
            return page;

        page = GalleryCatalog.CreatePage(group);
        if (page is not null)
            _componentPages.Add(group, page);

        return page;
    }

    private void Groups_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is DependencyObject source && ItemsControl.ContainerFromElement(Groups, source) is ListBoxItem)
            PaletteToggle.IsChecked = false;
    }

    private void Groups_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Enter or Key.Space && Groups.SelectedItem is not null)
            PaletteToggle.IsChecked = false;
    }

    private void DarkModeToggle_Changed(object sender, RoutedEventArgs e) => RefreshTheme();
    private void Palette_ThemeChanged(object? sender, EventArgs e) => RefreshTheme();

    private void PaletteToggle_Changed(object sender, RoutedEventArgs e)
    {
        bool show = PaletteToggle.IsChecked == true;
        CatalogView.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
        Palette.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
    }

    private void RefreshTheme() =>
        Palette.SetSwatches(_theme.Apply(Palette.Seed, Palette.SchemeVariant,
            DarkModeToggle.IsChecked == true, SystemParameters.HighContrast));

    private void SystemParameters_Changed(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SystemParameters.HighContrast))
            RefreshTheme();
    }

    protected override void OnClosed(EventArgs e)
    {
        SystemParameters.StaticPropertyChanged -= SystemParameters_Changed;
        base.OnClosed(e);
    }
}
