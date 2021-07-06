using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MyChessOnline.Hubs
{
    public class ChatHub:Hub
    {
        ApplicationDbContext _context;
        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Send(string message, string user)
        {
            await Clients.All.SendAsync("Receive", message, user, Context.ConnectionId);
            Debug.WriteLine($"{message}");
        }
        public async Task Move(string fen, string game)
        {
            
            Chess chess = new Chess();
            fen = chess.fen;
            chess = chess.Move("Pa2a3");
            await Groups.AddToGroupAsync(Context.ConnectionId, game);
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
                await Clients.Group(game).SendAsync("Chess", Game.Fen);
            }
            if(Game.Fen != fen)
                    Game.Fen = fen;
            
            _context.Update(Game);
            await _context.SaveChangesAsync();
            await Clients.Group(game).SendAsync("Chess",Game.Fen);
        }

        public override async Task OnConnectedAsync()
        {
            var context = this.Context.GetHttpContext();
            // получаем кук name
            if(context.Request.Cookies.ContainsKey("name"))
            {
                string userName;
                if(context.Request.Cookies.TryGetValue("name", out userName))
                {
                    //Debug.WriteLine($"name = {userName}");
                }
            }
            // получаем юзер-агент
            //Debug.WriteLine($"UserAgent = {context.Request.Headers["User-Agent"]}");
            // получаем ip
            //Debug.WriteLine($"RemoteIpAddress = {context.Connection.RemoteIpAddress.ToString()}");

            await base.OnConnectedAsync();
        }
    }
}
