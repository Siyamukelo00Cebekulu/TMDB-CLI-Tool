using System.Net.Http;
using TMDB_CLI_Tool.Presentation;
using TMDB_CLI_Tool.Repositories;
using TMDB_CLI_Tool.Services;

const string ApiKeyEnvironmentVariable = "TMDB_API_KEY";

var apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Error.WriteLine($"Error: TMDB API key is missing. Set the {ApiKeyEnvironmentVariable} environment variable.");
    Console.Error.WriteLine("Read README.md for setup instructions.");
    return;
}

using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
var repository = new TmdbMovieRepository(httpClient, apiKey);
var movieService = new MovieService(repository);
var app = new MovieConsoleApp(movieService);

var exitCode = await app.RunAsync(args);
Environment.Exit(exitCode);

