
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;

public class AudienceRatingsController : Controller
{
    private readonly MoviesAdminContext _context;

    public AudienceRatingsController(MoviesAdminContext context)
    {
        _context = context;
    }

    // GET: AUDIENCERATINGS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.AudienceRating.ToListAsync());
    }

    // GET: AUDIENCERATINGS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var audiencerating = await _context.AudienceRating
            .FirstOrDefaultAsync(m => m.Id == id);
        if (audiencerating == null)
        {
            return NotFound();
        }

        return View(audiencerating);
    }

    // GET: AUDIENCERATINGS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: AUDIENCERATINGS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Movies")] AudienceRating audiencerating)
    {
        if (ModelState.IsValid)
        {
            _context.Add(audiencerating);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(audiencerating);
    }

    // GET: AUDIENCERATINGS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var audiencerating = await _context.AudienceRating.FindAsync(id);
        if (audiencerating == null)
        {
            return NotFound();
        }
        return View(audiencerating);
    }

    // POST: AUDIENCERATINGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Movies")] AudienceRating audiencerating)
    {
        if (id != audiencerating.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(audiencerating);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AudienceRatingExists(audiencerating.Id))
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
        return View(audiencerating);
    }

    // GET: AUDIENCERATINGS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var audiencerating = await _context.AudienceRating
            .FirstOrDefaultAsync(m => m.Id == id);
        if (audiencerating == null)
        {
            return NotFound();
        }

        return View(audiencerating);
    }

    // POST: AUDIENCERATINGS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var audiencerating = await _context.AudienceRating.FindAsync(id);
        if (audiencerating != null)
        {
            _context.AudienceRating.Remove(audiencerating);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AudienceRatingExists(int? id)
    {
        return _context.AudienceRating.Any(e => e.Id == id);
    }
}
