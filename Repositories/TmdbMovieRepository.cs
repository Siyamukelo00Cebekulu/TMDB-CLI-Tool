using System.Net.Http;
using System.Text.Json;
using TMDB_CLI_Tool.Models;

namespace TMDB_CLI_Tool.Repositories;

public sealed class TmdbMovieRepository : IMovieRepository
{
    private const string BaseUrl = "https://api.themoviedb.org/3/movie/";
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public TmdbMovieRepository(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<Movie[]> GetMoviesAsync(MovieType movieType, CancellationToken cancellationToken = default)
    {
        var endpoint = movieType.ToEndpoint();
        var requestUrl = $"{BaseUrl}{endpoint}?api_key={_apiKey}&language=en-US&page=1";
        using var response = await _httpClient.GetAsync(requestUrl, cancellationToken);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var page = JsonSerializer.Deserialize<MoviePage>(content, options);
        return page?.Results ?? Array.Empty<Movie>();
    }
}
