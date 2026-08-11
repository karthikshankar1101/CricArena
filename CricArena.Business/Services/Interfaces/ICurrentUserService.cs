using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Email { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }

    }
}
