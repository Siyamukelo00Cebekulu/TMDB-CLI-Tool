using System.Text.Json.Serialization;

namespace TMDB_CLI_Tool.Models;

public enum MovieType
{
    Popular,
    TopRated,
    Upcoming,
    NowPlaying,
}

public static class MovieTypeExtensions
{
    public static string ToEndpoint(this MovieType movieType) => movieType switch
    {
        MovieType.Popular => "popular",
        MovieType.TopRated => "top_rated",
        MovieType.Upcoming => "upcoming",
        MovieType.NowPlaying => "now_playing",
        _ => throw new InvalidOperationException($"Unsupported movie type: {movieType}"),
    };

    public static string ToDisplayName(this MovieType movieType) => movieType switch
    {
        MovieType.Popular => "Popular Movies",
        MovieType.TopRated => "Top Rated Movies",
        MovieType.Upcoming => "Upcoming Movies",
        MovieType.NowPlaying => "Now Playing Movies",
        _ => movieType.ToString(),
    };

    public static bool TryParse(string? value, out MovieType movieType)
    {
        movieType = MovieType.Popular;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant();
        movieType = normalized switch
        {
            "popular" => MovieType.Popular,
            "top" => MovieType.TopRated,
            "top_rated" => MovieType.TopRated,
            "upcoming" => MovieType.Upcoming,
            "playing" => MovieType.NowPlaying,
            "now_playing" => MovieType.NowPlaying,
            _ => movieType,
        };

        return normalized is "popular" or "top" or "top_rated" or "upcoming" or "playing" or "now_playing";
    }

    public static string Usage() => "popular, top, upcoming, playing";
}
