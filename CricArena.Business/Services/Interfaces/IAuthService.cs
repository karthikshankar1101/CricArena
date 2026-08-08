using CricArena.Business.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);

        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
