using CricArena.Core.Entities;
using CricArena.Core.Enums;
using CricArena.Data.Context;
using CricArena.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CricArena.Data.Repositories
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly AppDbContext _context;

        public InvitationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Invitation?> GetByIdAsync(Guid id)
        {
            return await _context.Invitations
                .Include(i => i.Player)
                .Include(i => i.Club)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Invitation>> GetByClubIdAsync(Guid clubId)
        {
            return await _context.Invitations
                .Include(i => i.Player)
                .Where(i => i.ClubId == clubId)
                .ToListAsync();
        }

        public async Task<List<Invitation>> GetByPlayerIdAsync(Guid playerId)
        {
            return await _context.Invitations
                .Include(i => i.Club)
                .Where(i => i.PlayerId == playerId)
                .ToListAsync();
        }

        public async Task<Invitation?> GetPendingByClubIdAndPlayerIdAsync(
            Guid clubId,
            Guid playerId)
        {
            return await _context.Invitations.FirstOrDefaultAsync(i =>
                i.ClubId == clubId &&
                i.PlayerId == playerId &&
                i.Status == InvitationStatus.Pending);
        }

        public async Task AddAsync(Invitation invitation)
        {
            await _context.Invitations.AddAsync(invitation);
        }

        public Task UpdateAsync(Invitation invitation)
        {
            _context.Invitations.Update(invitation);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Invitation invitation)
        {
            _context.Invitations.Remove(invitation);
            return Task.CompletedTask;
        }
    }
}
