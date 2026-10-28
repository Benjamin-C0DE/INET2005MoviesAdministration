
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;
using MoviesAdmin.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

public class MoviesController : Controller
{
    private readonly MoviesAdminContext _context;
    private readonly IAzureBlobService _blobService; // Dependency injection for Azure Blob Service

    public MoviesController(MoviesAdminContext context, IAzureBlobService blobService)
    {
        _context = context;
        _blobService = blobService; // Initializes the Azure Blob Service
    }

    // GET: MOVIES
    public async Task<IActionResult> Index()            // Action that returns the list view of all movies
    {
        var movies = await _context.Movie
            .Include(m => m.Genre)                // Include the Genre navigation property
            .Include(m => m.AudienceRating)        // Include the AudienceRating navigation property
            .OrderByDescending(m => m.ReleaseDate)      // Sort movies newest first by ReleaseDate
            .ToListAsync();

        return View(movies);
    }

    // GET: MOVIES/Details
    public async Task<IActionResult> Details(int? id)   // Action to show details for a single movie
    {
        if (id == null)                                 // If no id provided
        {
            return NotFound();                           // Return 404 Not Found
        }

        var movie = await _context.Movie            // Query for the movie with the given id
            .Include(m => m.Genre)          //Display Genre on delete screen             
            .Include(m => m.AudienceRating)//Display Rating on delete screen
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)                          // If movie not found
        {
            return NotFound();                      // Return 404 Not Found
        }

        return View(movie);                         // Pass the movie to the Details view
    }

    // GET: MOVIES/Create
    public IActionResult Create()               // GET action to render the Create form
    {
        ViewData["GenreId"] = new SelectList(_context.Genre.OrderBy(g => g.Title), "Id", "Title"); // Populate genres for dropdown. Alphabetically
        
        ViewData["AudienceRatingId"] = new SelectList(_context.AudienceRating, "Id", "Title"); // Populate ratings for dropdown
        return View();                          // Render the Create view
    }

    // POST: MOVIES/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie, IFormFile? posterFile) // Poster file upload is handled via IFormFile parameter
    {
        // Handles poster file upload if provided
        if (posterFile != null)
        {
            string filename = Guid.NewGuid() + Path.GetExtension(posterFile.FileName);
            movie.PosterUrl = await _blobService.UploadFileAsync(posterFile, filename);
        }

        if (ModelState.IsValid) // Server-side validation check
        {
            _context.Add(movie);                        // Add new movie entity to the DbContext
            await _context.SaveChangesAsync();          // Persist changes to the database
            return RedirectToAction(nameof(Index));     // Redirect back to the Index list
        }

        ViewData["GenreId"] = new SelectList(_context.Genre.OrderBy(g => g.Title), "Id", "Title"); // Repopulate genres for dropdown if validation fails

        ViewData["AudienceRatingId"] = new SelectList(_context.AudienceRating, "Id", "Title"); // Repopulate ratings for dropdown if validation fails
        
        return View(movie);                            // Re-render the Create view with validation messages
    }

    // GET: MOVIES/Edit
    public async Task<IActionResult> Edit(int? id)          // GET action to render the Edit form for a movie
    {
        if (id == null)                                     // If no id provided
        {
            return NotFound();                              // Return 404 Not Found
        }

        var movie = await _context.Movie.FindAsync(id);     
        if (movie == null)                                  // If not found
        {
            return NotFound();                              // Return 404 Not Found
        }

        ViewData["GenreId"] = new SelectList(_context.Genre.OrderBy(g => g.Title), "Id", "Title", movie.GenreId); // Populate genres for dropdown with the current genre selected. Alphabetically

        ViewData["AudienceRatingId"] = new SelectList(_context.AudienceRating, "Id", "Title", movie.AudienceRatingId); // Populate ratings for dropdown with the current rating selected

        return View(movie);                                 // Render the Edit view with the movie model
    }

    // POST: MOVIES/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, Movie movie, IFormFile? posterFile) // poster file upload is handled via IFormFile parameter
    {
        if (id != movie.Id)                                 // Ensure route id matches model id
        {
            return NotFound();                              // Return 404 if mismatch
        }

        // If a new poster file was uploaded, replace the old one
        if (posterFile != null)
        {
            string filename = Guid.NewGuid() + Path.GetExtension(posterFile.FileName);
            movie.PosterUrl = await _blobService.UploadFileAsync(posterFile, filename);
        }

        if (ModelState.IsValid)                            // Server-side validation
        {
            try
            {
                _context.Update(movie);                  // Mark entity as modified
                await _context.SaveChangesAsync();      // Save changes to database
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(movie.Id))             // If the movie no longer exists
                {
                    return NotFound();                  // Return 404 Not Found
                }
                else
                {
                    throw;                              // Re-throw unexpected concurrency errors
                }
            }
            return RedirectToAction(nameof(Index));     // Redirect to Index on success
        }

        ViewData["GenreId"] = new SelectList(_context.Genre.OrderBy(g => g.Title), "Id", "Title", movie.GenreId); // Repopulate genres for dropdown if validation fails

        ViewData["AudienceRatingId"] = new SelectList(_context.AudienceRating, "Id", "Title", movie.AudienceRatingId); // Repopulate ratings for dropdown if validation fails

        return View(movie);                             // Re-render Edit view with validation messages
    }

    // GET: MOVIES/Delete
    public async Task<IActionResult> Delete(int? id) // GET action to confirm deletion of a movie
    {
        if (id == null)                                     // If no id provided
        {
            return NotFound();                              // Return 404 Not Found
        }

        var movie = await _context.Movie                    // Query the movie to display confirmation details
            .Include(m => m.Genre)          // Display Genre on delete screen
            .Include(m => m.AudienceRating) // Display rating on delete page
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)                              // If no id provided
        {
            return NotFound();                          // If no id provided
        }

        return View(movie);                            // Render Delete confirmation view     
    }

    // POST: MOVIES/Delete                                   
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)       // POST action that performs the deletion
    {
        var movie = await _context.Movie.FindAsync(id);             // Find the movie by id
        if (movie != null)                                           // If found
        {
            _context.Movie.Remove(movie);                           // Remove it from the DbContext
        }

        await _context.SaveChangesAsync();                          // Persist deletion to the database
        return RedirectToAction(nameof(Index));                     // Redirect back to the Index list
    }

    private bool MovieExists(int? id)                   // Helper to check whether a movie exists    
    {
        return _context.Movie.Any(e => e.Id == id);    // Returns true if any movie has the given id
    }
}
