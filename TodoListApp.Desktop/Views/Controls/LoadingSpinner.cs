using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TodoListApp.Desktop.Views.Controls;

/// <summary>
/// Простой индикатор загрузки: вращающаяся дуга на полупрозрачной окружности.
/// Анимация работает только пока <see cref="IsLoading"/> = true и элемент виден.
/// </summary>
public class LoadingSpinner : FrameworkElement
{
    private const double DefaultSize = 40;
    private const double SweepAngle = 270;

    private readonly RotateTransform rotateTransform = new();

    public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register(
            nameof(IsLoading),
            typeof(bool),
            typeof(LoadingSpinner),
            new PropertyMetadata(true, (d, _) => ((LoadingSpinner)d).UpdateAnimation()));

    public static readonly DependencyProperty ThicknessProperty =
        DependencyProperty.Register(
            nameof(Thickness),
            typeof(double),
            typeof(LoadingSpinner),
            new FrameworkPropertyMetadata(4.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(
            nameof(Color),
            typeof(Color),
            typeof(LoadingSpinner),
            new FrameworkPropertyMetadata(Colors.RoyalBlue, FrameworkPropertyMetadataOptions.AffectsRender));

    static LoadingSpinner()
    {
        // Размер по умолчанию, который можно переопределить через Width/Height
        WidthProperty.OverrideMetadata(typeof(LoadingSpinner), new FrameworkPropertyMetadata(DefaultSize));
        HeightProperty.OverrideMetadata(typeof(LoadingSpinner), new FrameworkPropertyMetadata(DefaultSize));
        IsHitTestVisibleProperty.OverrideMetadata(typeof(LoadingSpinner), new FrameworkPropertyMetadata(false));
    }

    public LoadingSpinner()
    {
        RenderTransform = rotateTransform;
        RenderTransformOrigin = new Point(0.5, 0.5);

        IsVisibleChanged += (_, _) => UpdateAnimation();
        Loaded += (_, _) => UpdateAnimation();
        Unloaded += (_, _) => StopAnimation();
    }

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>Толщина дуги.</summary>
    public double Thickness
    {
        get => (double)GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }

    /// <summary>Цвет дуги.</summary>
    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    private void UpdateAnimation()
    {
        if (IsLoading && IsVisible)
        {
            var animation = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(1))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }
        else
        {
            StopAnimation();
        }
    }

    private void StopAnimation() => rotateTransform.BeginAnimation(RotateTransform.AngleProperty, null);

    protected override void OnRender(DrawingContext drawingContext)
    {
        var thickness = Math.Max(Thickness, 0.5);
        var diameter = Math.Min(ActualWidth, ActualHeight) - thickness;
        if (diameter <= 0) { return; }

        var radius = diameter / 2;
        var center = new Point(ActualWidth / 2, ActualHeight / 2);

        var trackBrush = new SolidColorBrush(Color) { Opacity = 0.2 };
        trackBrush.Freeze();
        var arcBrush = new SolidColorBrush(Color);
        arcBrush.Freeze();

        var trackPen = new Pen(trackBrush, thickness);
        trackPen.Freeze();
        var arcPen = new Pen(arcBrush, thickness)
        {
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round
        };
        arcPen.Freeze();

        // Фоновая окружность
        drawingContext.DrawEllipse(null, trackPen, center, radius, radius);

        // Вращающаяся дуга: от 0° (справа) по часовой стрелке на 270°
        var start = new Point(center.X + radius, center.Y);
        var endRadians = SweepAngle * Math.PI / 180;
        var end = new Point(center.X + radius * Math.Cos(endRadians), center.Y + radius * Math.Sin(endRadians));

        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            context.BeginFigure(start, false, false);
            context.ArcTo(end, new Size(radius, radius), 0, true, SweepDirection.Clockwise, true, false);
        }
        geometry.Freeze();

        drawingContext.DrawGeometry(null, arcPen, geometry);
    }
}
