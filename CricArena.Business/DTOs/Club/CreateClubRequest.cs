using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.DTOs.Club
{
    public class CreateClubRequest
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
