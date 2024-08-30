using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDatabase.Models;

public abstract class BaseEntity
{    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Name cannot be longer than 20 characters")]
    public string Name { get; set; }

    [StringLength(500)] public string Description { get; set; } = string.Empty;
}