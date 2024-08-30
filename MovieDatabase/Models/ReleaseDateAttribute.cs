using System.ComponentModel.DataAnnotations;

namespace MovieDatabase.Models;

public class ReleaseDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime dateTime) return dateTime.Year > 1888;
        return false;
    }
}
