using CricArena.Core.Entities;

namespace CricArena.Data.Repositories.Interfaces
{
    public interface IJoinRequestRepository
    {
        Task<JoinRequest?> GetByIdAsync(Guid id);
        Task<List<JoinRequest>> GetByClubIdAsync(Guid clubId);

        Task<JoinRequest?> GetByClubIdAndPlayerIdAsync(Guid clubId, Guid playerId);
        Task<JoinRequest> AddAsync(JoinRequest joinRequest);
        Task UpdateAsync(JoinRequest joinRequest);
        Task DeleteAsync(JoinRequest joinRequest);
    }
}
