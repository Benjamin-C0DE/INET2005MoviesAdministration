using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "critic,administrator")]
public class CriticReviewsController : Controller
{
    private readonly MoviesAdminContext _context;

    public CriticReviewsController(MoviesAdminContext context)
    {
        _context = context;
    }

    // GET: CRITICREVIEWS
    public async Task<IActionResult> Index()
    {
        List<CriticReview> reviews;
        string userId = User.GetUserId();         // Get the current user's ID from the claims
        
        reviews = await _context.CriticReview // Access the CriticReview DbSet
            .Include(r => r.Movie)
            .Where(r => r.CreatedBy == userId)
            .ToListAsync();                       // Fetch all critic reviews created by the current user from the database

        return View(reviews);
    }

    // GET: CRITICREVIEWS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        CriticReview? criticreview;

        if (User.IsInRole("administrator"))
        {
            criticreview = await _context.CriticReview
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        else
        {
            criticreview = await _context.CriticReview
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.CreatedBy == User.GetUserId());
        }

        if (criticreview == null)
        {
            return NotFound();
        }

        return View(criticreview);
    }

    // GET: CRITICREVIEWS/Create
    public IActionResult Create(int? movieId)
    {
        if (movieId == null)
        {
            return RedirectToAction("Index", "Movies");
        }
        var movie = _context.Movie.FirstOrDefault(m => m.Id == movieId);

        if (movie == null)
        {
            return NotFound();
        }
        ViewBag.MovieTitle = movie.Title;

        var review = new CriticReview
        {
            MovieId = movie.Id
        };

        return View();
    }

    // POST: CRITICREVIEWS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Description,Rating,MovieId")] CriticReview criticreview)
    {
        if (ModelState.IsValid)
        {
            criticreview.CreatedBy = User.GetUserId(); // Set the CreatedBy property to the current user's ID
            criticreview.CreatedDate = DateTime.UtcNow; // Set the CreatedDate property to the current UTC date and time
            criticreview.IsPublished = false; // Set the IsPublished property to false by default

            criticreview.AuthorName =User.FindFirst("nickname")?.Value ?? User.Identity?.Name?? "User";
            criticreview.AuthorPicture = User.FindFirst("picture")?.Value;


            bool alreadyReviewed = await _context.CriticReview
                .AnyAsync(r => r.MovieId == criticreview.MovieId && r.CreatedBy == User.GetUserId()); // Check if the user has already submitted a review for the selected movie
            if (alreadyReviewed)
            {
                ModelState.AddModelError("", "You have already reviewed this movie.");
                ViewBag.MovieId = new SelectList(
                    _context.Movie.OrderBy(m => m.Title),
                    "Id",
                    "Title"
                );
                return View(criticreview);
            }

            _context.Add(criticreview);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        ViewBag.MovieId = new SelectList(
            _context.Movie.OrderBy(m => m.Title),
            "Id",
            "Title",
            criticreview.MovieId
        );
        return View(criticreview);
    }

    // GET: CRITICREVIEWS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        string userId = User.GetUserId(); // Get the current user's ID from the claims
        var criticreview = await _context.CriticReview
            .FirstOrDefaultAsync(r => r.Id == id && r.CreatedBy == userId);

        if (criticreview == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("administrator") && criticreview.IsPublished)
        {
            return RedirectToAction(nameof(Index));
        }
        ViewBag.MovieId = new SelectList(
            _context.Movie.OrderBy(m => m.Title),
            "Id",
            "Title",
            criticreview.MovieId
        );
        return View(criticreview);
    }

    // POST: CRITICREVIEWS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Description,Rating,MovieId")] CriticReview criticreview)
    {
        if (id != criticreview.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingReview = await _context.CriticReview
                .FirstOrDefaultAsync(r => r.Id == id && r.CreatedBy == User.GetUserId());

            if (existingReview == null)
            {
                return NotFound();
            }

            if (existingReview.IsPublished)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                existingReview.Description = criticreview.Description;
                existingReview.Rating = criticreview.Rating;
                existingReview.MovieId = criticreview.MovieId;

                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CriticReviewExists(criticreview.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.MovieId = new SelectList(
            _context.Movie.OrderBy(m => m.Title),
            "Id",
            "Title",
            criticreview.MovieId
        );
        return View(criticreview);
    }

    // GET: CRITICREVIEWS/Publish/5
    public async Task<IActionResult> Publish(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var criticreview = await _context.CriticReview
            .FirstOrDefaultAsync(r => r.Id == id && r.CreatedBy == User.GetUserId());
        if (criticreview == null)
        {
            return NotFound();
        }
        if (!User.IsInRole("administrator") && criticreview.IsPublished)
        {
            return RedirectToAction(nameof(Index));
        }
        criticreview.IsPublished = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: CRITICREVIEWS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        CriticReview? criticreview;

        if (User.IsInRole("administrator"))
        {
            criticreview = await _context.CriticReview
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        else
        {
            criticreview = await _context.CriticReview
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.CreatedBy == User.GetUserId());
        }

        if (criticreview == null)
        {
            return NotFound();
        }
        if (!User.IsInRole("administrator") && criticreview.IsPublished)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(criticreview);
    }

    // POST: CRITICREVIEWS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        CriticReview? criticreview;

        if (User.IsInRole("administrator"))
        {
            criticreview = await _context.CriticReview
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        else
        {
            criticreview = await _context.CriticReview
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.CreatedBy == User.GetUserId());
        }

        if (criticreview == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("administrator") && criticreview.IsPublished)
        {
            return RedirectToAction(nameof(Index));
        }

        _context.CriticReview.Remove(criticreview);
        
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool CriticReviewExists(int? id)
    {
        return _context.CriticReview.Any(e => e.Id == id);
    }
}
