using CricArena.Business.DTOs.Player;
using CricArena.Business.Services.Interfaces;
using CricArena.Core.Entities;
using CricArena.Data.Context;
using CricArena.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly AppDbContext _dbContext;
        public PlayerService(IPlayerRepository playerRepository, AppDbContext dbContext)
        {
            _playerRepository = playerRepository;
            _dbContext = dbContext;
        }
        public async Task<PlayerResponse> CreatePlayerAsync(CreatePlayerRequest request)
        {
            if (await _playerRepository.EmailExistsAsync(request.Email))
            {
                throw new Exception("Email already exists.");
            }

            var player = new Player
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            await _playerRepository.AddAsync(player);
            await _dbContext.SaveChangesAsync();

            return new PlayerResponse
            {
                Id = player.Id,
                Name = player.Name,
                Email = player.Email,
                PhoneNumber = player.PhoneNumber
            };
        }

        public async Task DeletePlayerAsync(Guid Id)
        {
            var player = await _playerRepository.GetByIdAsync(Id);
            if (player == null)
            {
                throw new Exception("Player does not exist.");
            }
            await _playerRepository.DeleteAsync(player);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PlayerResponse>> GetAllPlayersAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            return players.Select(player => new PlayerResponse
            {
                Id = player.Id,
                Name = player.Name,
                Email = player.Email,
                PhoneNumber = player.PhoneNumber
            }).ToList();
        }

        public async Task<PlayerResponse?> GetPlayerByIdAsync(Guid Id)
        {
            var player = await _playerRepository.GetByIdAsync(Id);
            if (player == null)
            {
                throw new Exception("Player does not exist.");
            }

            return new PlayerResponse
            {
                Id = player.Id,
                Name = player.Name,
                Email = player.Email,
                PhoneNumber = player.PhoneNumber
            };
        }

        public async Task UpdatePlayerAsync(Guid Id, UpdatePlayerRequest request)
        {
            var player = await _playerRepository.GetByIdAsync(Id);
            if (player == null)
            {
                throw new Exception("Player does not exist.");
            }
            player.Name = request.Name ?? player.Name;
            player.PhoneNumber = request.PhoneNumber ?? player.PhoneNumber;

            await _playerRepository.UpdateAsync(player);
            await _dbContext.SaveChangesAsync();
        }
    }
}
