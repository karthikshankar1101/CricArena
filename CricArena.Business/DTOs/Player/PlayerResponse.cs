using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.DTOs.Player
{
    public class PlayerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
