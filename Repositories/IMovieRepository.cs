using TMDB_CLI_Tool.Models;

namespace TMDB_CLI_Tool.Repositories;

public interface IMovieRepository
{
    Task<Movie[]> GetMoviesAsync(MovieType movieType, CancellationToken cancellationToken = default);
}
