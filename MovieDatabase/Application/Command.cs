using MovieDatabase.Models;
using Microsoft.AspNetCore.Mvc;

namespace MovieDatabase.Application;

public static class Command
{
    public static void CreateMovie(WebApplication webApplication)
    {
        webApplication.MapPost("/movie", async ([FromServices] IMovieService movieService, Movie movie) =>
        {
            var createdMovie = await movieService.CreateMovieAsync(movie);
            return Results.Created($"/movie/{createdMovie.Id}", createdMovie);
        });
    }

    public static void UpdateMovie(WebApplication webApplication)
    {
        webApplication.MapPut("/movie/{id}", async ([FromServices] IMovieService movieService, Movie updateMovie, int id) =>
        {
            try
            {
                await movieService.UpdateMovieAsync(id, updateMovie);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });
    }

    public static void DeleteMovie(WebApplication webApplication)
    {
        webApplication.MapDelete("/movie/{id}", async ([FromServices] IMovieService movieService, int id) =>
        {
            try
            {
                await movieService.DeleteMovieAsync(id);
                return Results.Ok();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });
    }
}