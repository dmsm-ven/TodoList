using System.Windows;

namespace TodoListApp.Desktop.Views;
/// <summary>
/// Interaction logic for ConnectionErrorView.xaml
/// </summary>
public partial class ConnectionErrorWindow : Window
{
    public ConnectionErrorWindow()
    {
        InitializeComponent();
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        // Begin dragging the window
        this.DragMove();
    }
}
