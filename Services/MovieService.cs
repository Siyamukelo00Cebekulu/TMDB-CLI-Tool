using TMDB_CLI_Tool.Models;
using TMDB_CLI_Tool.Repositories;

namespace TMDB_CLI_Tool.Services;

public sealed class MovieService
{
    private readonly IMovieRepository _repository;

    public MovieService(IMovieRepository repository)
    {
        _repository = repository;
    }

    public Task<Movie[]> GetMoviesAsync(MovieType movieType, CancellationToken cancellationToken = default)
        => _repository.GetMoviesAsync(movieType, cancellationToken);
}
