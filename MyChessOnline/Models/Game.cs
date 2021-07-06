using System;
using System.Collections.Generic;

#nullable disable

namespace MyChessOnline
{
    public partial class Game
    {
        public Game()
        {
            Sessions = new HashSet<Session>();
        }

        public long Id { get; set; }
        public string Fen { get; set; }
        public string Status { get; set; }
        public long? NextGameId { get; set; }
        public int? TournamentId { get; set; }
        public string WhoWin { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
