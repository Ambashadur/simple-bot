using System.Text.Json.Serialization;

namespace Simple.Bot.FoodClient;

internal sealed class RecipeShortInfo
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("imageType")]
    public required string ImageType { get; set;}
}
