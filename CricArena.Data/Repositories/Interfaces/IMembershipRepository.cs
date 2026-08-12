using CricArena.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Data.Repositories.Interfaces
{
    public interface IMembershipRepository
    {
        Task<Membership?> GetByIdAsync(Guid id);

        Task<Membership?> GetByClubAndPlayerAsync(
            Guid clubId,
            Guid playerId);

        Task<List<Membership>> GetByClubIdAsync(Guid clubId);

        Task<Membership> AddAsync(Membership membership);

        Task UpdateAsync(Membership membership);

        Task DeleteAsync(Membership membership);
    }
}
