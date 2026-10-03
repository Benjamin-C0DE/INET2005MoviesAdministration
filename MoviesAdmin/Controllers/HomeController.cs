using Microsoft.AspNetCore.Mvc;
using MoviesAdmin.Models;
using System.Diagnostics;

namespace MoviesAdmin.Controllers
{
    public class HomeController : Controller
    {
        // Index
        public IActionResult Index()
        {
            return View();
        }
        // Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // /MovieJson
        public JsonResult MovieJson()
        {
            var movie = new Movie
            {
                Id = 1,
                Title = "Inception",
                Synopsis = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                Genre = "Action, Adventure, Sci-Fi",
                Rating = "PG-13",
                RuntimeMinutes = 148,
                ReleaseDate = new DateTime(2010, 7, 16)
            };

            return Json(movie);
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
