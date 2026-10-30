using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TodoListApp.Desktop.Views.Controls;
/// <summary>
/// Interaction logic for TodoListMonthPillsNavigation.xaml
/// </summary>
public partial class TodoListMonthPillsNavigation : UserControl
{
    private const double ScrollStep = 200;

    public TodoListMonthPillsNavigation()
    {
        InitializeComponent();
    }

    private void OnScrollLeftClick(object sender, RoutedEventArgs e) =>
        PillsScrollViewer.ScrollToHorizontalOffset(Math.Max(0, PillsScrollViewer.HorizontalOffset - ScrollStep));

    private void OnScrollRightClick(object sender, RoutedEventArgs e) =>
        PillsScrollViewer.ScrollToHorizontalOffset(Math.Min(PillsScrollViewer.ScrollableWidth, PillsScrollViewer.HorizontalOffset + ScrollStep));

    // Колесо мыши над панелью прокручивает плитки по горизонтали
    private void OnPillsPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (PillsScrollViewer.ScrollableWidth <= 0) { return; }

        PillsScrollViewer.ScrollToHorizontalOffset(PillsScrollViewer.HorizontalOffset - e.Delta);
        e.Handled = true;
    }

    private void OnPillsScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        var hasOverflow = PillsScrollViewer.ScrollableWidth > 0;
        var visibility = hasOverflow ? Visibility.Visible : Visibility.Collapsed;

        ScrollLeftButton.Visibility = visibility;
        ScrollRightButton.Visibility = visibility;

        ScrollLeftButton.IsEnabled = PillsScrollViewer.HorizontalOffset > 0;
        ScrollRightButton.IsEnabled = PillsScrollViewer.HorizontalOffset < PillsScrollViewer.ScrollableWidth;
    }
}
