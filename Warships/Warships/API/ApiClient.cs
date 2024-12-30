using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Warships.API;
public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(string baseAddress/*, string token*/)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
    }

    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        var response = await _httpClient.PostAsync(endpoint, new StringContent(JsonSerializer.Serialize(data, JsonSerializerOptions.Web), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
    }
}

