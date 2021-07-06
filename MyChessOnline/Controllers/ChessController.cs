using ChessApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyChessOnline.Controllers
{
    [Authorize]
    public class ChessController : Controller
    {

        ApplicationDbContext _context;


        public ChessController(ApplicationDbContext context)
        {
            _context = context;
        }        
        public IActionResult Index()
        {
            var User = _context.Users.First(x => x.UserName == HttpContext.User.Identity.Name);
            var _sessions = _context.Sessions.Where(x => x.User == User);
            var _games = _sessions.Select(x => x.Game);
            ViewData["Id"] = User.Id;
            return View(_games);
        }

        [HttpPost]
        public async Task<IActionResult> IndexPost()
        {
           
            var User = await _context.Users.FirstAsync(x => x.UserName == HttpContext.User.Identity.Name);
            Chess chess = new Chess();
            await Task.Run(async() => {
                Debug.WriteLine("Добавляется новый элемент");
                Game game = new Game { Fen = chess.fen, Status = "wait", WhoWin = "no winner"};
                
                Session session = new Session
                {
                    
                    UserId = User.Id,
                    YourColor = "white",
                    IsYourMove= 1,
                    Game = game
                };
                _context.Sessions.Add(session);           
                await _context.SaveChangesAsync();
            });
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Game(string game)
        {
            try
            {
                var User = _context.Users.First(x => x.UserName == HttpContext.User.Identity.Name);
                
                if(User == null)
                {
                    throw new Exception("запрещенно заходить под таким");
                }
                var Game = _context.Games.First(x => x.Id.ToString() == game);
                var Session = _context.Sessions.First(x => x.User == User && x.Game==Game);
                ViewBag.Id = User.Id;
                ViewBag.Game = Game;
                ViewBag.Session = Session;
                ViewBag.IsYourMove = Session.IsYourMove;
                ViewBag.YourColor = Session.YourColor;
                if(Game.Sessions.Count == 2)
                {
                    var enemy = _context.Sessions.First(x => x.User != User);
                    ViewBag.Enemy = enemy;
                }
                else
                    ViewBag.Enemy = "Нет еще противника";
                return View("Game",Game);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                RedirectToAction("Index");
            }
            return View("Game");
        }

        [HttpPost]
        public async Task<IActionResult> Join(long Id)
        {
            var User = await _context.Users.FirstAsync(x => x.UserName == HttpContext.User.Identity.Name);
            Chess chess = new Chess();
            await Task.Run(async () => {
                Debug.WriteLine("Добавляется новый элемент");

                Session session = new Session
                {

                    UserId = User.Id,
                    YourColor = "black",
                    IsYourMove = 0,
                    GameId = Id,
                };
                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();
            });
            return RedirectToAction("Index");
        }

        public IActionResult Tournament()
        {
            return View();
        }



    }  
}
