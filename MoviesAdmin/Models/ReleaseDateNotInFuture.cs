using System;
using System.ComponentModel.DataAnnotations;

public class ReleaseDateNotInFuture : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            if (date > DateTime.Today)
            {
                return new ValidationResult("Release date cannot be in the future.");
            }
        }

        return ValidationResult.Success;
    }
}