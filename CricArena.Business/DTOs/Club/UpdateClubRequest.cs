using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.DTOs.Club
{
    public class UpdateClubRequest
    {
        public required string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
