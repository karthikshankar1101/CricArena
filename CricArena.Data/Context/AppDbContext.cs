using CricArena.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<JoinRequest> JoinRequests { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchAvailability> MatchAvailabilities { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for Match.GroundFees
            modelBuilder.Entity<Match>()
                .Property(m => m.GroundFees)
                .HasPrecision(10, 2);

            // Configure decimal precision for Payment.Amount
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);
        }
    }
}
