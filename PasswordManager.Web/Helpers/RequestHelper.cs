using Microsoft.JSInterop;
using System.Text.Json;
using System.Text;

namespace PasswordManager.Web.Helpers;

public class RequestHelper
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public RequestHelper(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    private async Task<string> GetAuthToken()
    {
        var authToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        return authToken?.Trim('"', ' ');
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        var authToken = await GetAuthToken();
        if (string.IsNullOrEmpty(authToken))
            throw new UnauthorizedAccessException("Authorization token is missing.");

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

        return await _httpClient.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T contentData)
    {
        var authToken = await GetAuthToken();
        if (string.IsNullOrEmpty(authToken))
            throw new UnauthorizedAccessException("Authorization token is missing.");

        var content = new StringContent(JsonSerializer.Serialize(contentData), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };

        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

        return await _httpClient.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T contentData)
    {
        var authToken = await GetAuthToken();
        if (string.IsNullOrEmpty(authToken))
            throw new UnauthorizedAccessException("Authorization token is missing.");

        var content = new StringContent(JsonSerializer.Serialize(contentData), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = content
        };

        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

        return await _httpClient.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        var authToken = await GetAuthToken();
        if (string.IsNullOrEmpty(authToken))
            throw new UnauthorizedAccessException("Authorization token is missing.");

        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, url);
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

        return await _httpClient.SendAsync(requestMessage);
    }

    // Classes

    //public class RequestErrorContent
    //{
    //    public List<string>Error
    //}

}
