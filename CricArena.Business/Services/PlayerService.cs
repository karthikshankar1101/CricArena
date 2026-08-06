using CricArena.Business.DTOs.Player;
using CricArena.Business.Exceptions;
using CricArena.Business.Services.Interfaces;
using CricArena.Core.Entities;
using CricArena.Data.Context;
using CricArena.Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PlayerService> _logger;
        public PlayerService(IPlayerRepository playerRepository, AppDbContext dbContext, ILogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<PlayerResponse> CreatePlayerAsync(CreatePlayerRequest request)
        {
            _logger.LogInformation(
                "Creating player with email {Email}",
                request.Email);
            ValidateCreatePlayerRequest(request);
            if (await _playerRepository.EmailExistsAsync(request.Email))
            {
                _logger.LogWarning(
                    "Duplicate email {Email}",
                    request.Email);
                throw new DuplicateEmailException(request.Email);
            }
            request.Email = request.Email.Trim().ToLowerInvariant();
            var player = new Player
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            await _playerRepository.AddAsync(player);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation(
                "Player created successfully. Id {Id}",
                player.Id);

            return MapToPlayerResponse(player);
        }

        public async Task DeletePlayerAsync(Guid Id)
        {
            var player = await _playerRepository.GetByIdAsync(Id);
            if (player == null)
            {
                throw new PlayerNotFoundException(Id);
            }
            await _playerRepository.DeleteAsync(player);
            _logger.LogInformation(
                "Player deleted successfully. Id {Id}",
                player.Id);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PlayerResponse>> GetAllPlayersAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            return players
                .Select(MapToPlayerResponse)
                .ToList();
        }

        public async Task<PlayerResponse?> GetPlayerByIdAsync(Guid Id)
        {
            var player = await _playerRepository.GetByIdAsync(Id);
            if (player == null)
            {
                throw new PlayerNotFoundException(Id);
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
                throw new PlayerNotFoundException(Id);
            }
            ValidateUpdatePlayerRequest(request);
            player.Name = request.Name ?? player.Name;
            player.PhoneNumber = request.PhoneNumber ?? player.PhoneNumber;

            _logger.LogInformation(
                "Updating player. Id {Id}",
                player.Id);
            await _playerRepository.UpdateAsync(player);
            await _dbContext.SaveChangesAsync();
        }

        private static PlayerResponse MapToPlayerResponse(Player player)
        {
            return new PlayerResponse
            {
                Id = player.Id,
                Name = player.Name,
                Email = player.Email,
                PhoneNumber = player.PhoneNumber
            };
        }

        private static void ValidateCreatePlayerRequest(CreatePlayerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Full Name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                throw new ArgumentException("Phone Number is required.");
        }

        private static void ValidateUpdatePlayerRequest(UpdatePlayerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Full Name is required.");

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                throw new ArgumentException("Phone Number is required.");
        }
    }
}
