using CricArena.Core.Entities;
using CricArena.Data.Context;
using CricArena.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Data.Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly AppDbContext _context;

        public MembershipRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Membership?> GetByIdAsync(Guid id)
        {
            return await _context.Memberships
                .Include(m => m.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Membership?> GetByClubAndPlayerAsync(
            Guid clubId,
            Guid playerId)
        {
            return await _context.Memberships
                .Include(m => m.Player)
                .FirstOrDefaultAsync(
                    m => m.ClubId == clubId &&
                         m.PlayerId == playerId);
        }

        public async Task<List<Membership>> GetByClubIdAsync(
            Guid clubId)
        {
            return await _context.Memberships
                .Include(m => m.Player)
                .Where(m => m.ClubId == clubId)
                .ToListAsync();
        }

        public async Task<Membership> AddAsync(
            Membership membership)
        {
            await _context.Memberships.AddAsync(membership);

            return membership;
        }

        public async Task UpdateAsync(
            Membership membership)
        {
            _context.Memberships.Update(membership);
        }

        public async Task DeleteAsync(
            Membership membership)
        {
            _context.Memberships.Remove(membership);
        }
    }
}
