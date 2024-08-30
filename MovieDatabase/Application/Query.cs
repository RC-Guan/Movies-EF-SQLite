namespace MovieDatabase.Application;

public static class Query
{
    public static void GetMovies(WebApplication webApplication)
    {
        webApplication.MapGet("/movies",
            async (IMovieService movieService) => await movieService.GetMoviesAsync());
    }

    public static void GetMovieById(WebApplication webApplication)
    {
        webApplication.MapGet("/movies/{id}",
            async (IMovieService movieService, int id) => await movieService.GetMovieByIdAsync(id));
    }
}