using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessApi;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MyChessOnline.Hubs
{
    public class ChessHub: Hub
    {
        ApplicationDbContext _context;
        Random random = new Random(DateTime.Now.Millisecond);
        public ChessHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Send(string message, string user)
        {
            await Clients.All.SendAsync("Receive", message, user, Context.ConnectionId);
        }

        public async Task Enter(string game)
        {
            var Game = _context.Games.First(x => x.Id.ToString() == game);
            await Groups.AddToGroupAsync(Context.ConnectionId, game);
            await Clients.Group(game).SendAsync("Connect", game, Game.Fen, Context.ConnectionId);
        }

        public async Task OverGame(bool mat, string game, string winner)
        {
            var Game = _context.Games.First(x => x.Id.ToString() == game);
            if(Game.NextGameId == null)
            {
                await Clients.Group(game).SendAsync("NextGame", false, -1);
                return;
            }
            var GameNext = _context.Games.First(x => x.Id == Game.NextGameId);
            var Sides = _context.Sessions.Where(x => x.Game == Game).ToList();
            var SidesNext = _context.Sessions.Where(x => x.Game == GameNext).ToList();
         
            foreach(var el in Sides)
            {
                if(el.YourColor[0] != winner[0])
                {
                    var User = await _context.Users.FirstAsync(x => x.Id == el.UserId);
                    Game.WhoWin = User.UserName.Split('@')[0];

                    el.Game = GameNext;
                    el.GameId = GameNext.Id;
                    if(SidesNext.Count == 0)
                    {
                        lock(random){
                            el.IsYourMove = random.Next(1);
                                
                            el.YourColor = el.IsYourMove == 1 ? "white":"black";

                        }

                    }
                    if(SidesNext.Count == 1)
                    {
                        el.IsYourMove = SidesNext.First().IsYourMove == 1 ? 0 : 1;
                        el.YourColor = SidesNext.First().YourColor == "white" ? "black" : "white";
                    }
                    if(SidesNext.Count == 2)
                    {
                        break;
                    }
                    _context.Update(el);
                }
            }
            _context.Update(Game);
            await _context.SaveChangesAsync();
            await Clients.Group(game).SendAsync("NextGame", true, GameNext.Id);
        }

        public async Task Move(string fen, string move, string game)
        {
            //Chess chess = new Chess(fen);
            //var Game = _context.Games.First(x => x.Id.ToString() == game);
            //try
            //{
            //    if(Game == null)
            //    {
            //        throw new Exception("Ошибка в игре");
            //    }
            //}
            //catch
            //{

            //}
            //if(Game.Fen != fen)
            //    Game.Fen = fen;
            //_context.Update(Game);
            //await _context.SaveChangesAsync();
            //await Clients.Group(game).SendAsync("Chess", Game.Fen, move, Context.ConnectionId);
            Chess chess = new Chess(fen);
            var Game = _context.Games.First(x => x.Id.ToString() == game);
            try
            {
                if(Game == null)
                {
                    throw new Exception("Ошибка в игре");
                }
            }
            catch
            {
                await Clients.Group(game).SendAsync("Chess", Game.Fen, Context.ConnectionId);
            }
            if(Game.Fen != fen)
                Game.Fen = fen;
            _context.Update(Game);

            var Sessions = Game.Sessions;
            foreach(var s in Sessions)
            {
                s.IsYourMove = s.IsYourMove == 1 ? 0 : 1;
            }
            _context.UpdateRange(Sessions);

            await _context.SaveChangesAsync();
            await Clients.Group(game).SendAsync("Chess", Game.Fen, Context.ConnectionId);
        }

        public override async Task OnConnectedAsync()
        {
            var context = this.Context.GetHttpContext();
            if(context.Request.Cookies.ContainsKey("name"))
            {
                string userName;
                if(context.Request.Cookies.TryGetValue("name", out userName))
                {
                }
            }
            await base.OnConnectedAsync();
        }
    }
}
