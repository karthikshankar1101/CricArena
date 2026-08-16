using CricArena.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.DTOs.Club
{
    public class ClubMemberResponse
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime JoinedOn { get; set; }
    }
}
