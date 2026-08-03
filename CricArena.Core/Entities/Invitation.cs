using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Core.Entities
{
    public class Invitation
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid ClubId { get; set; }
        public DateTime SentOn { get; set; } = DateTime.UtcNow;
        public bool IsAccepted { get; set; }
        public DateTime? RepondedOn { get; set; }

        // Navigation Properties (for EF Core)
        public Player Player { get; set; } = null!;
        public Club Club { get; set; } = null!;
    }
}
