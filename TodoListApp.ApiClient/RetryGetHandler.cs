using System.Net;

namespace TodoListApp.ApiClient;

/// <summary>
/// Повторяет только GET-запросы при временных сбоях (сетевая ошибка, 502/503/504/408).
/// POST/PUT/DELETE не повторяются, чтобы не продублировать запись.
/// </summary>
public class RetryGetHandler : DelegatingHandler
{
    private static readonly TimeSpan[] Delays = { TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(1) };

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method != HttpMethod.Get)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        for (int attempt = 0; ; attempt++)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                if (!IsTransient(response.StatusCode) || attempt >= Delays.Length)
                {
                    return response;
                }
                response.Dispose();
            }
            catch (Exception ex) when ((ex is HttpRequestException or TaskCanceledException)
                                       && !cancellationToken.IsCancellationRequested
                                       && attempt < Delays.Length)
            {
                // временный сбой - пробуем еще раз
            }

            await Task.Delay(Delays[attempt], cancellationToken);
        }
    }

    private static bool IsTransient(HttpStatusCode code) =>
        code is HttpStatusCode.RequestTimeout
            or HttpStatusCode.BadGateway
            or HttpStatusCode.ServiceUnavailable
            or HttpStatusCode.GatewayTimeout;
}
