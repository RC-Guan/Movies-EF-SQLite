using MovieDatabase.Models;

namespace MovieDatabase.Application;

public static class Command
{
    public static void CreateMovie(WebApplication webApplication)
    {
        webApplication.MapPost("/movie", async (IMovieService movieService, Movie movie) =>
        {
            var createdMovie = await movieService.CreateMovieAsync(movie);
            return Results.Created($"/movie/{createdMovie.Id}", createdMovie);
        });
    }

    public static void UpdateMovie(WebApplication webApplication)
    {
        webApplication.MapPut("/movie/{id}",
            async (IMovieService movieService, Movie updateMovie, int id) =>
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
        webApplication.MapDelete("/movie/{id}", async ( IMovieService movieService, int id) =>
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