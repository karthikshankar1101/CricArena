using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.DTOs.JoinRequest
{
    public class CreateJoinRequestRequest
    {
        public Guid ClubId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
