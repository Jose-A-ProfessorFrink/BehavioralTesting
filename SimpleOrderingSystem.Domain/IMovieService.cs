using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Domain;

public interface IMovieService
{
    Task<List<MovieSummary>> SearchMoviesAsync(string name);

    Task<Movie?> GetMovieAsync(string movieId);
}