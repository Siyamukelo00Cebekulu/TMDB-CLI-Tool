using System.Text.Json.Serialization;

namespace TMDB_CLI_Tool.Models;

public sealed record MoviePage(
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("results")] Movie[] Results,
    [property: JsonPropertyName("total_pages")] int TotalPages,
    [property: JsonPropertyName("total_results")] int TotalResults);
