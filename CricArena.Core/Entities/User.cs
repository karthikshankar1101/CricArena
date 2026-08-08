using CricArena.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ClubRole Role { get; set; } = ClubRole.Player;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
