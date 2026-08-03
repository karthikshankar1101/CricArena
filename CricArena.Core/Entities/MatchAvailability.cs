using CricArena.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Core.Entities
{
    public class MatchAvailability
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public Guid Playerid { get; set; }
        public AvailabilityStatus Status { get; set; }
        public DateTime RespondedOn { get; set; } = DateTime.UtcNow;

        // Navigation Properties (for EF Core)
        public Match Match { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
