using CricArena.Business.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CricArena.Business.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userId =
                    _httpContextAccessor.HttpContext?
                        .User
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value;

                if (Guid.TryParse(userId, out var id))
                {
                    return id;
                }

                return null;
            }
        }

        public string? Email =>
            _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.Email)?
                .Value;

        public string? Role =>
            _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.Role)?
                .Value;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?
                .User
                .Identity?
                .IsAuthenticated ?? false;
    }
}
