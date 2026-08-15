namespace CricArena.Core.Entities
{
    public class Player
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
        public ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
        public ICollection<MatchAvailability> MatchAvailabilities { get; set; } = new List<MatchAvailability>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
