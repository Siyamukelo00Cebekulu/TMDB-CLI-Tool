using System.Text.Json.Serialization;

namespace TMDB_CLI_Tool.Models;

public sealed record Movie(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("overview")] string Overview,
    [property: JsonPropertyName("release_date")] string ReleaseDate,
    [property: JsonPropertyName("vote_average")] double VoteAverage);
