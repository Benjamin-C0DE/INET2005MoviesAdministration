namespace MoviesAdmin.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
