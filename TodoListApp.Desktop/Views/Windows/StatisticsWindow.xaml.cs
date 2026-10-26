using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using TodoListApp.Desktop.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace TodoListApp.Desktop.Views;
/// <summary>
/// Interaction logic for StatisticsWindow.xaml
/// </summary>
public partial class StatisticsWindow : FluentWindow
{
    private bool redrawQueued;
    private StatisticsViewModel? subscribedViewModel;

    public StatisticsWindow()
    {
        InitializeComponent();

        // Графики статичные: отключаем зум/перетаскивание, чтобы колесо мыши прокручивало окно
        DynamicsPlot.UserInputProcessor.Disable();
        ComparisonPlot.UserInputProcessor.Disable();

        DataContextChanged += (_, _) => Subscribe();
        ApplicationThemeManager.Changed += OnThemeChanged;
        Closed += (_, _) =>
        {
            ApplicationThemeManager.Changed -= OnThemeChanged;
            Unsubscribe();
        };
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, System.Windows.Media.Color systemAccent) => QueueRedraw();

    private void Subscribe()
    {
        Unsubscribe();

        if (DataContext is StatisticsViewModel vm)
        {
            subscribedViewModel = vm;
            vm.Dynamics.CollectionChanged += OnDataChanged;
            vm.Comparison.CollectionChanged += OnDataChanged;
            QueueRedraw();
        }
    }

    private void Unsubscribe()
    {
        if (subscribedViewModel != null)
        {
            subscribedViewModel.Dynamics.CollectionChanged -= OnDataChanged;
            subscribedViewModel.Comparison.CollectionChanged -= OnDataChanged;
            subscribedViewModel = null;
        }
    }

    private void OnDataChanged(object? sender, NotifyCollectionChangedEventArgs e) => QueueRedraw();

    // Clear + Add порождают много событий подряд - перерисовываем один раз после них
    private void QueueRedraw()
    {
        if (redrawQueued) { return; }
        redrawQueued = true;
        Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(Redraw));
    }

    private void Redraw()
    {
        redrawQueued = false;

        if (DataContext is not StatisticsViewModel vm) { return; }

        var dark = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark;

        // Динамика хранится от нового месяца к старому, на графике нужен обратный порядок
        var months = vm.Dynamics.Reverse().ToList();
        DrawBars(DynamicsPlot, months.Select(m => m.ShortMonth).ToList(), months.Select(m => (double)m.Total).ToList(), dark);

        DrawBars(ComparisonPlot, vm.Comparison.Select(c => c.Name).ToList(), vm.Comparison.Select(c => (double)c.Total).ToList(), dark);
    }

    private static void DrawBars(ScottPlot.WPF.WpfPlot control, IReadOnlyList<string> labels, IReadOnlyList<double> values, bool dark)
    {
        var plot = control.Plot;
        plot.Clear();
        plot.SetStyle(dark ? new ScottPlot.PlotStyles.Dark() : new ScottPlot.PlotStyles.Light());

        if (values.Count > 0)
        {
            var bars = plot.Add.Bars(values.ToArray());
            bars.Color = ScottPlot.Color.FromHex("#4169E1");

            for (var i = 0; i < bars.Bars.Count; i++)
            {
                bars.Bars[i].LineWidth = 0;
                bars.Bars[i].Label = values[i] > 0 ? values[i].ToString("N0") : string.Empty;
                bars.Bars[i].LabelOnTop = true;
            }

            plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                labels.Select((label, i) => new ScottPlot.Tick(i, label)).ToArray());
        }

        var max = values.Count > 0 ? values.Max() : 0;
        plot.Axes.SetLimitsY(0, max > 0 ? max * 1.2 : 1);

        control.Refresh();
    }
}
