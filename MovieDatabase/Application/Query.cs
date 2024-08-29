using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Application;


public static class Query
{
    public static void GetMovies(WebApplication webApplication)
    {
        webApplication.MapGet("/movies", async ([FromServices] IMovieService movieService) => await movieService.GetMoviesAsync());
    }

    public static void GetMovieById(WebApplication webApplication)
    {
        webApplication.MapGet("/movies/{id}", async ([FromServices] IMovieService movieService, int id) => await movieService.GetMovieByIdAsync(id));
    }
}