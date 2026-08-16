using CricArena.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Data.Repositories.Interfaces
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Task<Player?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<Player?> GetPlayerByUserIdAsync(Guid userId);
    }
}
