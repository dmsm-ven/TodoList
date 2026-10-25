using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using TodoListApp.ApiClient;

namespace TodoListApp.Desktop.Infrastructure;

/// <summary>Единое место показа ошибок пользователю (вместо падения приложения)</summary>
public static class UiErrors
{
    public static void Show(Exception exception, string? action = null)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher != null && !dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => Show(exception, action));
            return;
        }

        string details = exception switch
        {
            ApiException api => api.Message,
            HttpRequestException => "Не удалось связаться с сервером. Проверьте подключение к сети и адрес API.",
            TaskCanceledException => "Сервер не ответил за отведенное время.",
            _ => exception.Message
        };

        var text = string.IsNullOrWhiteSpace(action) ? details : $"{action}\r\n\r\n{details}";
        MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
