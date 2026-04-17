using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

const string ApiKeyEnvironmentVariable = "TMDB_API_KEY";
const string ApiBaseUrl = "https://api.themoviedb.org/3/movie/";

var movieType = ParseMovieType(args);
if (movieType is null)
{
    PrintUsage();
    return;
}

var apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Error.WriteLine($"Error: TMDB API key is missing. Set the {ApiKeyEnvironmentVariable} environment variable.");
    Console.Error.WriteLine("Read README.md for setup instructions.");
    return;
}

var endpoint = MapMovieType(movieType);
if (endpoint is null)
{
    Console.Error.WriteLine($"Error: Unsupported movie type '{movieType}'.");
    PrintUsage();
    return;
}

var requestUrl = $"{ApiBaseUrl}{endpoint}?api_key={apiKey}&language=en-US&page=1";

try
{
    using var httpClient = new HttpClient();
    httpClient.Timeout = TimeSpan.FromSeconds(15);

    Console.WriteLine($"Fetching {GetDisplayName(movieType)} from TMDB...");

    var response = await httpClient.GetAsync(requestUrl);
    if (!response.IsSuccessStatusCode)
    {
        Console.Error.WriteLine($"API request failed with status {(int)response.StatusCode} {response.ReasonPhrase}.");
        return;
    }

    var json = await response.Content.ReadAsStringAsync();
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    var moviePage = JsonSerializer.Deserialize<MoviePage>(json, options);
    if (moviePage?.Results is not { Length: > 0 })
    {
        Console.WriteLine("No movies found.");
        return;
    }

    PrintMovies(moviePage.Results);
}
catch (HttpRequestException ex)
{
    Console.Error.WriteLine($"Network error: {ex.Message}");
}
catch (TaskCanceledException)
{
    Console.Error.WriteLine("Request timed out. Check your network connection and try again.");
}
catch (JsonException ex)
{
    Console.Error.WriteLine($"Failed to parse API response: {ex.Message}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected error: {ex.Message}");
}

static string? ParseMovieType(string[] args)
{
    if (args.Length == 0)
    {
        return null;
    }

    for (var index = 0; index < args.Length; index++)
    {
        var arg = args[index];
        if (arg.Equals("--type", StringComparison.OrdinalIgnoreCase) || arg.Equals("-t", StringComparison.OrdinalIgnoreCase))
        {
            if (index + 1 < args.Length)
            {
                return args[index + 1].Trim().ToLowerInvariant();
            }

            return null;
        }

        if (arg.StartsWith("--type=", StringComparison.OrdinalIgnoreCase))
        {
            return arg[("--type=").Length..].Trim().ToLowerInvariant();
        }
    }

    return null;
}

static void PrintUsage()
{
    Console.WriteLine("TMDB CLI Tool");
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run -- --type popular");
    Console.WriteLine("  dotnet run -- --type top");
    Console.WriteLine("  dotnet run -- --type upcoming");
    Console.WriteLine("  dotnet run -- --type playing");
    Console.WriteLine();
    Console.WriteLine("Supported values for --type: popular, top, upcoming, playing");
}

static string? MapMovieType(string movieType) => movieType switch
{
    "popular" => "popular",
    "top" => "top_rated",
    "upcoming" => "upcoming",
    "playing" => "now_playing",
    _ => null,
};

static string GetDisplayName(string movieType) => movieType switch
{
    "popular" => "Popular Movies",
    "top" => "Top Rated Movies",
    "upcoming" => "Upcoming Movies",
    "playing" => "Now Playing Movies",
    _ => movieType,
};

static void PrintMovies(Movie[] movies)
{
    Console.WriteLine();
    Console.WriteLine("Top results:");
    Console.WriteLine(new string('-', 60));

    for (var index = 0; index < Math.Min(movies.Length, 10); index++)
    {
        var movie = movies[index];
        Console.WriteLine($"{index + 1}. {movie.Title} ({movie.ReleaseDate}) - Rating {movie.VoteAverage:N1}");
        if (!string.IsNullOrWhiteSpace(movie.Overview))
        {
            Console.WriteLine($"   {movie.Overview}");
        }
        Console.WriteLine();
    }
}

internal sealed record MoviePage(
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("results")] Movie[] Results,
    [property: JsonPropertyName("total_pages")] int TotalPages,
    [property: JsonPropertyName("total_results")] int TotalResults);

internal sealed record Movie(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("overview")] string Overview,
    [property: JsonPropertyName("release_date")] string ReleaseDate,
    [property: JsonPropertyName("vote_average")] double VoteAverage);
