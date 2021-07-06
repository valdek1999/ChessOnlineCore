using ChessApi;
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
    public class TournamentController : Controller
    {
        ApplicationDbContext _context;


        public TournamentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var Model = _context.Tournaments.Select(x=>x);
            return View("TournamentList",Model);
        }

        public IActionResult Create()
        {
            return View("TournamentCreate");
        }
        public async Task<IActionResult> Join(int id)
        {
            var Games = _context.Games.Where(x => x.TournamentId == id).OrderByDescending(x=>x.Id).ToList();
            var Tour = _context.Tournaments.FirstOrDefault(x => x.Id == id);
            ViewBag.Parts = _context.TournamentParticipants.Where(x => x.Tournament == Tour).ToList();
            ViewBag.Games = Games;
            ViewBag.FinallyGame = Games.Last();
            ViewBag.Users = _context.Users.ToList();
            return View("Tournament", Tour);
        }

        
        public async Task<IActionResult> ChoiseSlot(long game, int slot)
        {
            
            var Game = _context.Games.Find(game);
            var game_id = Game.Id.ToString();
            var Sessions = _context.Sessions.Where(x => x.GameId == game).ToList();            
            var User = await _context.Users.FirstAsync(x => x.UserName == HttpContext.User.Identity.Name);
            
            var Participant = _context.TournamentParticipants.FirstOrDefault(x => x.TournamentId == Game.TournamentId && slot == x.Slot);
            var Tournament = _context.Tournaments.Find((long)Game.TournamentId);
            while(true)
            {
                if(Game.WhoWin != "no winner")
                {
                    if(Game.WhoWin != User.UserName.Split('@')[0])
                        return RedirectToAction("Join", "Tournament", new { id = Game.TournamentId });
                    Game = _context.Games.Find(Game.NextGameId);
                    continue;
                }
                break;
            }

            if(Participant != null)
            {
                if(User != Participant.User)
                {
                    return RedirectToAction("Join", "Tournament", new { id = Game.TournamentId });
                }
                else
                {
                    return RedirectToAction("Game", "Chess", new { game = Game.Id });
                }
            }
            else
            {
                var temp = _context.TournamentParticipants.FirstOrDefault(x => x.User == User && x.TournamentId == Game.TournamentId);
                if(temp != null)
                {
                    return RedirectToAction("Join", "Tournament", new { id = Game.TournamentId });
                }
            }
            await Task.Run(async () => {
                Debug.WriteLine("Добавляется новый элемент");
                Game = _context.Games.Find(game);
                Session session = new Session
                {

                    UserId = User.Id,
                    YourColor = Sessions.Count == 0 ? "white":"black",
                    IsYourMove = Sessions.Count == 0 ? 1 : 0,
                    GameId = Game.Id,
                    Game = Game,
                    User = User
                };
                TournamentParticipant tournamentParticipant = new TournamentParticipant
                {
                    TournamentId = (int)Game.TournamentId,
                    CountWin = 0,
                    Slot = slot,
                    User = User
                };
                Tournament.CountConnect++;
                _context.Tournaments.Update(Tournament);
                _context.Sessions.Add(session);
                _context.Games.Update(Game);
                _context.TournamentParticipants.Add(tournamentParticipant);
                await _context.SaveChangesAsync();
            });
            
            return RedirectToAction("Game","Chess",new { game = game_id });
        }



        [BindProperty]
        Tournament tournament { get; set; } = new Tournament();
        [HttpPost]
        public async Task<IActionResult> Create(Tournament tournament)
        {
            this.tournament = tournament;
            ChessApi.Chess chess = new ChessApi.Chess();
            await Task.Run(async () => {
                await _context.Tournaments.AddAsync(tournament);
                await _context.SaveChangesAsync();

                Game game = new Game
                {
                    Fen = chess.fen,
                    NextGameId = -1,
                    Status = "wait",
                    TournamentId = (int)tournament.Id,
                    WhoWin = "no winner",
                };
                await _context.Games.AddAsync(game);
                await _context.SaveChangesAsync();
                await GameR(2, game.Id);

                
            });
            
            return RedirectToAction("Index");
        }
        private async Task GameR(int count_game, long next_game)
        {
            Chess chess = new ChessApi.Chess();

            Game game1 = new Game
            {
                Fen = chess.fen,
                NextGameId = next_game,
                Status = "wait",
                TournamentId = (int)tournament.Id,
                WhoWin = "no winner",
            };
            Game game2 = new Game
            {
                Fen = chess.fen,
                NextGameId = next_game,
                Status = "wait",
                TournamentId = (int)tournament.Id,
                WhoWin = "no winner",
            };
            
            await _context.Games.AddRangeAsync(game1,game2);
            await _context.SaveChangesAsync();
            if(tournament.CountPlayer==Math.Pow(2, count_game))
            {
                return;
            }
            var count = ++count_game;
            await GameR(count, game1.Id);
            await GameR(count, game2.Id);
            
        }
    }

    
}
