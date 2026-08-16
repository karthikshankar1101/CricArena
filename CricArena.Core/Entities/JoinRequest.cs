using CricArena.Core.Enums;

namespace CricArena.Core.Entities
{
    public class JoinRequest
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid ClubId { get; set; }
        public MembershipStatus Status { get; set; } = MembershipStatus.Pending;
        public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedOn { get; set; }
        public string? Remarks { get; set; }

        // Navigation Properties (for EF Core)
        public Player Player { get; set; } = null!;

        public Club Club { get; set; } = null!;
    }
}
