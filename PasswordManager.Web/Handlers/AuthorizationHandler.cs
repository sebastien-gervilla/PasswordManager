using System.Net.Http.Headers;
using Microsoft.JSInterop;

public class AuthorizationHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public AuthorizationHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("console.log", $"Retrieved Token: {token}");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}