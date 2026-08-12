using CricArena.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.DTOs.Club
{
    public class UpdateMemberRoleRequest
    {
        public ClubRole Role { get; set; }
    }
}
