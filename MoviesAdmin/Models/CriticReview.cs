using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class CriticReview
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Rating: 1–5 stars
        [Range(1, 5)]
        public int Rating { get; set; }

        public bool IsPublished { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public int MovieId { get; set; }

        public Movie? Movie { get; set; }
        public string AuthorName { get; set; } = string.Empty;

        public string? AuthorPicture { get; set; }
    }
}
