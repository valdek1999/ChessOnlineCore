using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyChessOnline.Models;

#nullable disable

namespace MyChessOnline
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Game> Games { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentParticipant> TournamentParticipants { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Game>().ToTable("Game");
            modelBuilder.Entity<Session>().ToTable("Session");
            modelBuilder.Entity<Tournament>().ToTable("Tournament");
            modelBuilder.Entity<TournamentParticipant>().ToTable("TournamentParticipant");
            base.OnModelCreating(modelBuilder);
        }
    }
}
