using Microsoft.EntityFrameworkCore;

namespace MoviesAdmin.Models
{
    public class MoviesAdminContext : DbContext
    {
        public MoviesAdminContext(DbContextOptions<MoviesAdminContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; }
    }
}