using System.Text.Json.Serialization;

namespace Simple.Bot.FoodClient;

internal sealed class RecipeInformationResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("imageType")]
    public required string ImageType { get; set; }

    [JsonPropertyName("servings")]
    public int Servings { get; set; }

    [JsonPropertyName("readyInMinutes")]
    public int ReadyInMinutes { get; set; }

    [JsonPropertyName("cookingMinutes")]
    public int CookingMinutes { get; set; }

    [JsonPropertyName("preparationMinutes")]
    public int PreparationMinutes { get; set; }

    [JsonPropertyName("sourceUrl")]
    public required string SourceUrl { get; set; }

    [JsonPropertyName("summary")]
    public required string Summary { get; set; }

    [JsonPropertyName("instructions")]
    public required string Instructions { get; set; }

    [JsonPropertyName("analyzedInstructions")]
    public required RecipeInstructionStep[] AnalyzedInstructions { get; set; }
}
