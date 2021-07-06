using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyChessOnline.Data;
using MyChessOnline.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyChessOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
       
        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            var auth = HttpContext.User.Identity.IsAuthenticated;
            var type = HttpContext.User.Identity.AuthenticationType;
            var name = HttpContext.User.Identity.Name;
            var claims_name = HttpContext.User.Identities.ToList();
            
            ViewData["auth"] = auth;
            ViewData["type"] = type;
            ViewData["name"] = name;
            ViewData["claims_name"] = claims_name;
            //_context.Games.ToList();
            //var id = _context.Users.First().Id;
            return View(users);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Chat => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
