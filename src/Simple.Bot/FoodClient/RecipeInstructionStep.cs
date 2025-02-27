using System.Text.Json.Serialization;

namespace Simple.Bot.FoodClient;

internal sealed class RecipeInstructionStep
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("step")]
    public required string Step { get; set; }

    [JsonPropertyName("ingredients")]
    public required EntityShortInfo[] Ingredients { get; set; }

    [JsonPropertyName("equipment")]
    public required EntityShortInfo[] Equipment { get; set; }
}
