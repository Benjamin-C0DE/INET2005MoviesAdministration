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
