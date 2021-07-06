using System;
using System.Collections.Generic;

#nullable disable

namespace MyChessOnline
{
    public partial class Tournament
    {
        public Tournament()
        {
            TournamentParticipants = new HashSet<TournamentParticipant>();
        }
        public long Id { get; set; }
        public int CountPlayer { get; set; }
        public int CountConnect { get; set; }
        public int StatusTournament { get; set; }
        public string NamingTournament { get; set; }

        public virtual ICollection<TournamentParticipant> TournamentParticipants { get; set; }
    }
}
