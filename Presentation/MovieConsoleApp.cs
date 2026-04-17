using System.Text.Json;
using TMDB_CLI_Tool.Models;
using TMDB_CLI_Tool.Services;

namespace TMDB_CLI_Tool.Presentation;

public sealed class MovieConsoleApp
{
    private readonly MovieService _movieService;

    public MovieConsoleApp(MovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default)
    {
        if (!TryParseMovieType(args, out var movieType))
        {
            PrintUsage();
            return 1;
        }

        Console.WriteLine($"Fetching {movieType.ToDisplayName()} from TMDB...");

        try
        {
            var movies = await _movieService.GetMoviesAsync(movieType, cancellationToken);
            if (movies.Length == 0)
            {
                Console.WriteLine("No movies found.");
                return 0;
            }

            PrintMovies(movies);
            return 0;
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"Network error: {ex.Message}");
            return 2;
        }
        catch (TaskCanceledException)
        {
            Console.Error.WriteLine("Request timed out. Check your network connection and try again.");
            return 2;
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Failed to parse API response: {ex.Message}");
            return 2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected error: {ex.Message}");
            return 2;
        }
    }

    private static bool TryParseMovieType(string[] args, out MovieType movieType)
    {
        movieType = default;
        if (args.Length == 0)
        {
            return false;
        }

        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            if (arg.Equals("--type", StringComparison.OrdinalIgnoreCase) || arg.Equals("-t", StringComparison.OrdinalIgnoreCase))
            {
                if (index + 1 < args.Length)
                {
                    return MovieTypeExtensions.TryParse(args[index + 1], out movieType);
                }

                return false;
            }

            if (arg.StartsWith("--type=", StringComparison.OrdinalIgnoreCase))
            {
                return MovieTypeExtensions.TryParse(arg[("--type=").Length..], out movieType);
            }
        }

        return false;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("TMDB CLI Tool");
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run -- --type popular");
        Console.WriteLine("  dotnet run -- --type top");
        Console.WriteLine("  dotnet run -- --type upcoming");
        Console.WriteLine("  dotnet run -- --type playing");
        Console.WriteLine();
        Console.WriteLine("Supported values for --type: " + MovieTypeExtensions.Usage());
    }

    private static void PrintMovies(Movie[] movies)
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
}
