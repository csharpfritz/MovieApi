using System.ComponentModel.DataAnnotations;

namespace MovieApi.Models;

public class Movie
{
	[Key]
	public int Id { get; set; }
	public string Title { get; set; }
	public int ReleaseYear { get; set; }
	public string Director { get; set; }
	public string Description { get; set; }
	public List<Genre> Genre { get; set; }
	public float Rating { get; set; }
	public DateTime ReleaseDate { get; set; }
}

public class Genre 
{

	public int Id { get; set; }
	public string Name { get; set; }

	public List<Movie> Movies { get; set; }
	public string Description { get; set; }

}
