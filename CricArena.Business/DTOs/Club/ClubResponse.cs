using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.DTOs.Club
{
    public class ClubResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedByPlayerId { get; set; }
    }
}
