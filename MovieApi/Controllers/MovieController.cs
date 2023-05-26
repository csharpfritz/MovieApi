using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Models;
using System.ComponentModel.Design;

[ApiController]
[Route("api/[controller]")]
public class MovieController : ControllerBase
{
	private readonly MovieContext _context;

	public MovieController(MovieContext context)
	{
		_context = context;
	}

	[HttpPost]
	public async Task<ActionResult<Movie>> AddMovie(Movie movie)
	{

		// handle null values
		if (movie.Genre == null)
		{
			movie.Genre = new List<Genre>();
		}

		// lookup the genre by name
		foreach (var genre in movie.Genre)
		{
			var genreInDb = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genre.Name);
			if (genreInDb != null)
			{
				movie.Genre.Remove(genre);
				movie.Genre.Add(genreInDb);
			} else {

				// if the genre doesn't exist, add it to the database
				_context.Genres.Add(genre);

			}
		}

		_context.Movies.Add(movie);
		await _context.SaveChangesAsync();

		return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Movie>> GetMovie(int id)
	{
		var movie = await _context.Movies.FindAsync(id);

		if (movie == null)
		{
			return NotFound();
		}

		return movie;
	}

	// get all movies
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
	{
		return await _context.Movies.ToListAsync();
	}
}
