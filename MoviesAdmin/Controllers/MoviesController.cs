
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;

public class MoviesController : Controller
{
    private readonly MoviesAdminContext _context;

    public MoviesController(MoviesAdminContext context)
    {
        _context = context;
    }

    // MPAA
    private List<string> GetRatings()       // Helper method that returns the allowed MPAA rating options
    {
        return new List<string>             // Returns in-memory list of the MPAA rating as strings
        {
            "G",
            "PG",
            "PG-13",
            "R",
            "NC-17"
        };
    }

    // GET: MOVIES
    public async Task<IActionResult> Index()            // Action that returns the list view of all movies
    {
        var movies = await _context.Movie
            .OrderByDescending(m => m.ReleaseDate)      // Sort movies newest first by ReleaseDate
            .ToListAsync();

        return View(movies);
    }

    // GET: MOVIES/Details/5
    public async Task<IActionResult> Details(int? id)   // Action to show details for a single movie
    {
        if (id == null)                                 // If no id provided
        {
            return NotFound();                           // Return 404 Not Found
        }

        var movie = await _context.Movie            // Query for the movie with the given id
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
        ViewBag.Ratings = GetRatings();         // Provide rating options to the view via ViewBag
        return View();                          // Render the Create view
    }

    // POST: MOVIES/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Synopsis,Genre,Rating,RuntimeMinutes,ReleaseDate")] Movie movie)
    {
        if (ModelState.IsValid) // Server-side validation check
        {
            _context.Add(movie);                        // Add new movie entity to the DbContext
            await _context.SaveChangesAsync();          // Persist changes to the database
            return RedirectToAction(nameof(Index));     // Redirect back to the Index list
        }                                              
        ViewBag.Ratings = GetRatings();                // If validation failed, repopulate ratings for the form
        return View(movie);                            // Re-render the Create view with validation messages
    }

    // GET: MOVIES/Edit/5
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
        ViewBag.Ratings = GetRatings();                     // Provide rating options to the view
        return View(movie);                                 // Render the Edit view with the movie model
    }

    // POST: MOVIES/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Synopsis,Genre,Rating,RuntimeMinutes,ReleaseDate")] Movie movie)
    {
        if (id != movie.Id)                                 // Ensure route id matches model id
        {
            return NotFound();                              // Return 404 if mismatch
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
        ViewBag.Ratings = GetRatings();                 // Repopulate ratings if validation failed  
        return View(movie);                             // Re-render Edit view with validation messages
    }

    // GET: MOVIES/Delete/5
    public async Task<IActionResult> Delete(int? id) // GET action to confirm deletion of a movie
    {
        if (id == null)                                     // If no id provided
        {
            return NotFound();                              // Return 404 Not Found
        }

        var movie = await _context.Movie                    // Query the movie to display confirmation details
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)                              // If no id provided
        {
            return NotFound();                          // If no id provided
        }

        return View(movie);                            // Render Delete confirmation view     
    }

    // POST: MOVIES/Delete/5                                    
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
