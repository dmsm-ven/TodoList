using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
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
    private static readonly OxyColor BarColor = OxyColor.Parse("#4169E1");

    private bool redrawQueued;
    private StatisticsViewModel? subscribedViewModel;

    public StatisticsWindow()
    {
        InitializeComponent();

        // Графики статичные: отключаем зум/перетаскивание/трекер, чтобы колесо мыши прокручивало окно
        DynamicsPlot.Controller = CreateStaticController();
        ComparisonPlot.Controller = CreateStaticController();

        DataContextChanged += (_, _) => Subscribe();
        ApplicationThemeManager.Changed += OnThemeChanged;
        Closed += (_, _) =>
        {
            ApplicationThemeManager.Changed -= OnThemeChanged;
            Unsubscribe();
        };
    }

    private static PlotController CreateStaticController()
    {
        var controller = new PlotController();
        controller.UnbindAll();
        return controller;
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

    private static void DrawBars(OxyPlot.Wpf.PlotView control, IReadOnlyList<string> labels, IReadOnlyList<double> values, bool dark)
    {
        var textColor = dark ? OxyColors.WhiteSmoke : OxyColors.Black;
        var lineColor = dark ? OxyColor.FromRgb(120, 120, 120) : OxyColor.FromRgb(160, 160, 160);
        var gridColor = dark ? OxyColor.FromArgb(40, 255, 255, 255) : OxyColor.FromArgb(40, 0, 0, 0);

        var model = new PlotModel
        {
            Background = OxyColors.Transparent,
            PlotAreaBackground = OxyColors.Transparent,
            PlotAreaBorderColor = OxyColors.Transparent,
            TextColor = textColor
        };

        var max = values.Count > 0 ? values.Max() : 0;

        // Нижняя ось числовая: подписи категорий подставляются по целым индексам столбцов
        var categoryAxis = new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Minimum = -0.6,
            Maximum = Math.Max(values.Count - 1, 0) + 0.6,
            MajorStep = 1,
            MinorStep = 1,
            MajorTickSize = 0,
            MinorTickSize = 0,
            LabelFormatter = v =>
            {
                var index = (int)Math.Round(v);
                return Math.Abs(v - index) < 0.001 && index >= 0 && index < labels.Count ? labels[index] : string.Empty;
            },
            TextColor = textColor,
            TicklineColor = lineColor,
            AxislineColor = lineColor,
            AxislineStyle = LineStyle.Solid,
            IsZoomEnabled = false,
            IsPanEnabled = false
        };

        var valueAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            Minimum = 0,
            Maximum = max > 0 ? max * 1.2 : 1,
            StringFormat = "N0",
            TextColor = textColor,
            TicklineColor = lineColor,
            AxislineColor = lineColor,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = gridColor,
            IsZoomEnabled = false,
            IsPanEnabled = false
        };

        model.Axes.Add(categoryAxis);
        model.Axes.Add(valueAxis);

        if (values.Count > 0)
        {
            var series = new RectangleBarSeries
            {
                FillColor = BarColor,
                StrokeThickness = 0
            };

            for (var i = 0; i < values.Count; i++)
            {
                series.Items.Add(new RectangleBarItem(i - 0.3, 0, i + 0.3, values[i]));

                // Подпись значения над столбцом (для нулевых не показываем)
                if (values[i] > 0)
                {
                    model.Annotations.Add(new TextAnnotation
                    {
                        Text = values[i].ToString("N0"),
                        TextPosition = new DataPoint(i, values[i]),
                        TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center,
                        TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                        TextColor = textColor,
                        Stroke = OxyColors.Transparent,
                        Background = OxyColors.Transparent,
                        Padding = new OxyThickness(0, 0, 0, 2)
                    });
                }
            }

            model.Series.Add(series);
        }

        control.Model = model;
    }
}
