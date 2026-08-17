using CricArena.Core.Entities;
using CricArena.Data.Context;
using CricArena.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Data.Repositories
{
    public class JoinRequestRepository : IJoinRequestRepository
    {
        private readonly AppDbContext _context;
        public JoinRequestRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<JoinRequest> AddAsync(JoinRequest joinRequest)
        {
            await _context.JoinRequests.AddAsync(joinRequest);
            return joinRequest;
        }

        public async Task DeleteAsync(JoinRequest joinRequest)
        {
            _context.JoinRequests.Remove(joinRequest);
        }

        public async Task<JoinRequest?> GetByClubIdAndPlayerIdAsync(Guid clubId, Guid playerId)
        {
            return await _context.JoinRequests
                .Include(x => x.Player)
                .Include(x => x.Club)
                .FirstOrDefaultAsync(x => x.ClubId == clubId && x.PlayerId == playerId);
        }

        public async Task<JoinRequest?> GetPendingByClubIdAndPlayerIdAsync(Guid clubId, Guid playerId)
        {
            return await _context.JoinRequests
                .FirstOrDefaultAsync(x =>
                    x.ClubId == clubId &&
                    x.PlayerId == playerId &&
                    x.Status == CricArena.Core.Enums.MembershipStatus.Pending);
        }

        public async Task<List<JoinRequest>> GetByClubIdAsync(Guid clubId)
        {
            return await _context.JoinRequests
                .Include(x => x.Player)
                .Where(x => x.ClubId == clubId).ToListAsync();
        }

        public async Task<JoinRequest?> GetByIdAsync(Guid id)
        {
            return await _context.JoinRequests
                .Include(x => x.Player)
                .Include(x => x.Club)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(JoinRequest joinRequest)
        {
            _context.JoinRequests.Update(joinRequest);
        }
    }
}
