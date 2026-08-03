using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Core.Entities
{
    public class Club
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public Guid CreatedByPlayerId   { get; set; }
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
        public ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
