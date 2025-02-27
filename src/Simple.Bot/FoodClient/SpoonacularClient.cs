using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Bot.FoodClient;

internal sealed class SpoonacularClient
{
    private static string? _apiKey;

    private static SpoonacularClient? _instance;
    private readonly HttpClient _httpClient;

    public static SpoonacularClient Instance => _instance ??= new SpoonacularClient();
    public static string? ApiKey { set => _apiKey = value; }

    private SpoonacularClient() {
        var socketHandler = new SocketsHttpHandler {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        };

        _httpClient = new HttpClient(socketHandler) {
            BaseAddress = new Uri("https://api.spoonacular.com")
        };
    }

    public Task<PageResponse<RecipeShortInfo>> RecipeComplexSearch(ComplexSearchQuery query, CancellationToken cancellationToken) {
        var request = new HttpRequestMessage(HttpMethod.Get, $"recipes/complexSearch?apiKey={_apiKey}{query}");
        return Send<PageResponse<RecipeShortInfo>>(request, cancellationToken);
    }

    public Task<RecipeInformationResponse> GetRecipeInformation(long id, CancellationToken cancellationToken) {
        var request = new HttpRequestMessage(HttpMethod.Get, $"recipes/{id}/information?apiKey={_apiKey}");
        return Send<RecipeInformationResponse>(request, cancellationToken);
    }

    private async Task<T> Send<T>(HttpRequestMessage request, CancellationToken cancellationToken) {
        HttpResponseMessage response;

        try {
            response = await _httpClient.SendAsync(request, cancellationToken);
        } catch (Exception exception) {
            throw new HttpRequestException("Exception while sending request", exception, null);
        }

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid response status code", null, response.StatusCode);

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);

        T deserializedResponse;

        try {
            deserializedResponse = (await JsonSerializer.DeserializeAsync<T>(responseStream))!;
        } catch (Exception exception) {
            var contentString = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException("Response deserialization failed", exception, response.StatusCode);
        }

        if (deserializedResponse == null!)
            throw new HttpRequestException("Deserialized response is null", null, response.StatusCode);

        return deserializedResponse;
    }
}
