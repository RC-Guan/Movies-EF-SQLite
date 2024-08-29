using System.ComponentModel.DataAnnotations;

namespace MovieDatabase.Models;

public class Movie : BaseEntity
{  
    [Required]
    public DateTime ReleaseDate { get; set; }

    [Required]
    [StringLength(20)]
    public string Genre { get; set; } = string.Empty;
}