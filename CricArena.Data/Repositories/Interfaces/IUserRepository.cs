using CricArena.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAync(Guid id);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);
    }
}
