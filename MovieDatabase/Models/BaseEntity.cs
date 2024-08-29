using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDatabase.Models;

public abstract class BaseEntity
{    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }

    [Required] [StringLength(20)] public string Name { get; set; }

    [StringLength(500)] public string Description { get; set; } = string.Empty;
}