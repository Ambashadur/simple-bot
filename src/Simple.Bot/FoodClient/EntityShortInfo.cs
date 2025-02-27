using System.Text.Json.Serialization;

namespace Simple.Bot.FoodClient;

internal sealed class EntityShortInfo
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("localizedName")]
    public required string LocalizedName { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }
}
