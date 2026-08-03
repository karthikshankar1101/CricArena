using CricArena.Core.Enums;

namespace CricArena.Core.Entities
{
    public class Membership
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Player Player { get; set; } = null!;
        public Guid ClubId { get; set; }
        public Club Club { get; set; } = null!;
        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;
        public ClubRole Role { get; set; }
        public MembershipStatus Status { get; set; }
    }
}
