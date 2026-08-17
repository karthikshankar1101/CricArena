using CricArena.Core.Entities;
using CricArena.Data.Context;
using CricArena.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace CricArena.Data.Repositories
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Player?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(p => p.Email == email);
        }

        public async Task<List<Player>> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbSet
                .Where(p => p.PhoneNumber == phoneNumber)
                .ToListAsync();
        }

        public async Task<bool> PhoneNumberExistsAsync(
            string phoneNumber,
            Guid? excludedPlayerId = null)
        {
            return await _dbSet.AnyAsync(p =>
                p.PhoneNumber == phoneNumber &&
                (!excludedPlayerId.HasValue || p.Id != excludedPlayerId.Value));
        }

        public async Task<Player?> GetPlayerByUserIdAsync(Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
