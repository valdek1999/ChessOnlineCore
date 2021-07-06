using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

#nullable disable

namespace MyChessOnline
{
    public partial class Session
    {
        public long Id { get; set; }
        public long GameId { get; set; }
        public string UserId { get; set; }
        public string YourColor { get; set; }
        public int? IsYourMove { get; set; }

        public virtual Game Game { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}
