using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MovieApi.Models;
using Xunit;

namespace MovieTests;

public class MovieControllerTests
{
	private readonly MovieController _controller;
	private readonly Mock<MovieContext> _mockDbContext;

	public MovieControllerTests()
	{
		_mockDbContext = new Mock<MovieContext>();

		_controller = new MovieController(_mockDbContext.Object);
	}

	[Fact]
	public async Task AddMovie_ShouldAddMovie_WhenValidInputIsSupplied()
	{
		// Arrange
		var movie = new Movie { Title = "The Godfather", Description = "A mafia boss tries to legitimize his business.", Rating = 9.2f, ReleaseDate = new System.DateTime(1972, 3, 24) };
		var genre1 = new Genre { Name = "Crime" };
		var genre2 = new Genre { Name = "Drama" };

		movie.Genre = new List<Genre> { genre1, genre2 };

		_ = _mockDbContext.Setup(m => m.Genres.FirstOrDefaultAsync(g => g.Name == genre1.Name))
				.ReturnsAsync(genre1);

		_mockDbContext.Setup(m => m.Genres.FirstOrDefaultAsync(g => g.Name == genre2.Name))
				.ReturnsAsync(genre2);

		// Act
		var result = await _controller.AddMovie(movie);

		// Assert
		var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
		var createdMovie = Assert.IsAssignableFrom<Movie>(createdResult.Value);

		Assert.Equal(movie.Title, createdMovie.Title);
		Assert.Equal(movie.Description, createdMovie.Description);
		Assert.Equal(movie.Rating, createdMovie.Rating);
		Assert.Equal(movie.ReleaseDate, createdMovie.ReleaseDate);

		Assert.Contains(createdMovie.Genre, g => g.Name == "Crime");
		Assert.Contains(createdMovie.Genre, g => g.Name == "Drama");

		_mockDbContext.Verify(m => m.Movies.Add(It.IsAny<Movie>()), Times.Once());
		_mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Once());
	}

	[Fact]
	public async Task GetMovie_ShouldReturnMovieFromDb_WhenMovieExistsInDatabase()
	{
		// Arrange
		var movie = new Movie { Id = 1, Title = "The Godfather", Rating = 9.2f, ReleaseDate = new System.DateTime(1972, 3, 24) };

		_mockDbContext.Setup(m => m.Movies.FindAsync(1)).ReturnsAsync(movie);

		// Act
		var result = await _controller.GetMovie(1);

		// Assert
		var movieResult = Assert.IsAssignableFrom<Movie>(result.Value);

		Assert.Equal(movie.Id, movieResult.Id);
		Assert.Equal(movie.Title, movieResult.Title);
		Assert.Equal(movie.Rating, movieResult.Rating);
		Assert.Equal(movie.ReleaseDate, movieResult.ReleaseDate);
	}

	[Fact]
	public async Task GetMovie_ShouldReturnNotFound_WhenMovieDoesNotExistInDatabase()
	{
		// Arrange
		_mockDbContext.Setup(m => m.Movies.FindAsync(1)).ReturnsAsync((Movie)null);

		// Act
		var result = await _controller.GetMovie(1);

		// Assert
		Assert.IsType<NotFoundResult>(result.Result);
	}
}
