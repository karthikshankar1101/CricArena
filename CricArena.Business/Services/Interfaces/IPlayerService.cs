using CricArena.Business.DTOs.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<PlayerResponse> CreatePlayerAsync(CreatePlayerRequest request);
        Task<List<PlayerResponse>> GetAllPlayersAsync();
        Task<PlayerResponse?> GetPlayerByIdAsync(Guid Id);
        Task UpdatePlayerAsync(Guid Id, UpdatePlayerRequest request);
        Task DeletePlayerAsync(Guid Id);
    }
}
