using CricArena.Business.DTOs.Club;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Services.Interfaces
{
    public interface IClubService
    {
        Task<ClubResponse> CreateClubAsync(
        CreateClubRequest request);

        Task<List<ClubResponse>> GetAllClubsAsync();

        Task<ClubResponse?> GetClubByIdAsync(
            Guid id);

        Task UpdateClubAsync(
            Guid id,
            UpdateClubRequest request);

        Task DeleteClubAsync(
            Guid id);

        Task<List<ClubMemberResponse>> GetMembersAsync(
            Guid clubId);

        Task UpdateMemberRoleAsync(
            Guid clubId,
            Guid playerId,
            UpdateMemberRoleRequest request);
    }
}
