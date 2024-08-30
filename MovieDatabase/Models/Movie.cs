using System.ComponentModel.DataAnnotations;

namespace MovieDatabase.Models;

public class Movie : BaseEntity
{  
    [Required]
    [ReleaseDate(ErrorMessage = "Release date cannot be earlier than the year 1888.")]
    public DateTime ReleaseDate { get; set; }
    
    [Required(ErrorMessage = "Genre is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Genre cannot be empty")]
    public string Genre { get; set; }
}