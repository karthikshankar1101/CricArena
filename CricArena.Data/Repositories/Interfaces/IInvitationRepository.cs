using CricArena.Core.Entities;

namespace CricArena.Data.Repositories.Interfaces
{
    public interface IInvitationRepository
    {
        Task<Invitation?> GetByIdAsync(Guid id);
        Task<List<Invitation>> GetByClubIdAsync(Guid clubId);
        Task<List<Invitation>> GetByPlayerIdAsync(Guid playerId);
        Task<Invitation?> GetPendingByClubIdAndPlayerIdAsync(Guid clubId, Guid playerId);
        Task AddAsync(Invitation invitation);
        Task UpdateAsync(Invitation invitation);
        Task DeleteAsync(Invitation invitation);
    }
}
