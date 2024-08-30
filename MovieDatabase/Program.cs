using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MovieDatabase.Application;
using MovieDatabase.Infrastructure;
using MovieDatabase.Models;

namespace MovieDatabase;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure services
        builder.Services.AddEndpointsApiExplorer();

        // Connect to DBContext
        builder.Services.AddDbContext<MoviesDbContext>(opt =>
        {
            opt.UseSqlite(builder.Configuration.GetConnectionString("DbConnectionString"));
        });

        // Add Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MovieDatabase API",
                Description = "Record the movies you love",
                Version = "v1"
            });
        });

        // Register IMovieService
        builder.Services.AddScoped<IMovieService, MovieService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieDatabase API V1"); });
        }

        GetMovies(app);
        GetMovieById(app);
        CreateMovie(app);
        UpdateMovie(app);
        DeleteMovie(app);

        app.Run();
    }

    private static void GetMovies(WebApplication webApplication)
    {
        webApplication.MapGet("/movies", async (IMovieService movieService) => await movieService.GetMoviesAsync());
    }

    private static void GetMovieById(WebApplication webApplication)
    {
        webApplication.MapGet("/movies/{id:int}", async (int id, IMovieService movieService) =>
        {
            var movie = await movieService.GetMovieByIdAsync(id);
            return movie is not null ? Results.Ok(movie) : Results.NotFound();
        });
    }

    private static void CreateMovie(WebApplication webApplication)
    {
        webApplication.MapPost("/movie", async (IMovieService movieService, Movie movie) =>
        {
            try
            {
                var createdMovie = await movieService.CreateMovieAsync(movie);
                return Results.Created($"/movie/{createdMovie.Id}", createdMovie);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest("Incorrect movie data");
            }
        });
    }

    private static void UpdateMovie(WebApplication webApplication)
    {
        webApplication.MapPut("/movie/{id:int}",
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

    private static void DeleteMovie(WebApplication webApplication)
    {
        webApplication.MapDelete("/movie/{id:int}", async (IMovieService movieService, int id) =>
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