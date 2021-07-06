using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace MyChessOnline
{
    public partial class TournamentParticipant
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        
        public int? CountWin { get; set; }
        public int? Slot { get; set; }

        public long TournamentId { get; set; }
        [ForeignKey("TournamentId")]
        public virtual Tournament Tournament { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}
