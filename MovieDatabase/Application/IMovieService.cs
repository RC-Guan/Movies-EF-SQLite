using MovieDatabase.Models;

namespace MovieDatabase.Application;

public interface IMovieService
{
    Task<List<Movie>> GetMovies();
    Task<Movie?> GetMovieById(int id);
    Task<Movie> CreateMovie(Movie movie);
    Task UpdateMovie(int id, Movie updateMovie);
    Task DeleteMovie(int id);
}