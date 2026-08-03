using CricArena.Core.Enums;

namespace CricArena.Core.Entities
{
    public class Match
    {
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string GroundName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public TimeOnly MatchTime { get; set; }
        public int overs { get; set; }
        public BallType BallType { get; set; }
        public Enums.MatchType MatchType { get; set; }
        public decimal GroundFees { get; set; }
        public MatchStatus MatchStatus { get; set; }

        //Navigation Properties
        public Club Club { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<MatchAvailability> MatchAvailabilities { get; set; } = new List<MatchAvailability>();

    }
}
