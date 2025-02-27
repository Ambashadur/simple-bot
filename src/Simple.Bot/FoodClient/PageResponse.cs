using System.Text.Json.Serialization;

namespace Simple.Bot.FoodClient;

internal sealed class PageResponse<TEntity> where TEntity : class
{
    [JsonPropertyName("results")]
    public required TEntity[] Results { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }
}
