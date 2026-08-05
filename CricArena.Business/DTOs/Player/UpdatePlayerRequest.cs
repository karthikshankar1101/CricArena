using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.DTOs.Player
{
    public class UpdatePlayerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
