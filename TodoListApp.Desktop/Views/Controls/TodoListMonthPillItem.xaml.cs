using System.Windows;
using System.Windows.Controls;

namespace TodoListApp.Desktop.Views.Controls;
/// <summary>
/// Interaction logic for TodoListMonthPillItem.xaml
/// </summary>
public partial class TodoListMonthPillItem : UserControl
{
    public bool IsSelectedPill
    {
        get { return (bool)GetValue(IsSelectedPillProperty); }
        set { SetValue(IsSelectedPillProperty, value); }
    }

    // Using a DependencyProperty as the backing store for IsSelectedPill.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsSelectedPillProperty =
        DependencyProperty.Register("IsSelectedPill", typeof(bool), typeof(TodoListMonthPillItem), new PropertyMetadata(false));

    public TodoListMonthPillItem()
    {
        InitializeComponent();
    }
}
