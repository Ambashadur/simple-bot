using System.Text;

namespace Simple.Bot.FoodClient;

public class ComplexSearchQuery
{
    public string? Query { get; set; }

    public string? Cuisine { get; set; }

    public string? TitleMatch { get; set; }

    public override string ToString() {
        var sb = new StringBuilder();

        if (Query is not null) {
            sb.Append("&query=");
            sb.Append(Query);
        }

        if (Cuisine is not null) {
            sb.Append("&cuisine=");
            sb.Append(Cuisine);
        }

        if (TitleMatch is not null) {
            sb.Append("&titleMatch=");
            sb.Append(TitleMatch);
        }

        return sb.ToString();
    }
}
