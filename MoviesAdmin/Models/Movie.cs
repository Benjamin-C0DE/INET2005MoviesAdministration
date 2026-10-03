using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)] // Max length 100 characters
        public string Title { get; set; } //Title of the movie

        [Required]
        [StringLength(500)] // Max length 500 characters
        public string Synopsis { get; set; } // Short description or synopsis of the movie

        [Required]
        public string Genre { get; set; } // Genre of the movie (e.g., Action, Comedy, Drama)

        [Required]
        public string Rating { get; set; } // MPAA-style rating (G, PG, PG-13, R, NC-17)

        [Range(1, 600, ErrorMessage = "Runtime must be between 1 and 600 minutes.")] // Runtime in minutes, must be between 1 and 600
        public int RuntimeMinutes { get; set; } // Duration of the movie in minutes

        [DataType(DataType.Date)] // Indicates that this property should be treated as a date
        [ReleaseDateNotInFuture] // Custom validation attribute to ensure the release date is not in the future
        public DateTime ReleaseDate { get; set; } // Release date of the movie
    }
}