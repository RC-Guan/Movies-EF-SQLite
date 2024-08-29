using MovieDatabase.Models;

namespace MovieDatabase.Application;

public interface IMovieService
{
    Task<List<Movie>> GetMoviesAsync();
    Task<Movie?> GetMovieByIdAsync(int id);
    Task<Movie> CreateMovieAsync(Movie movie);
    Task UpdateMovieAsync(int id, Movie updateMovie);
    Task DeleteMovieAsync(int id);
}