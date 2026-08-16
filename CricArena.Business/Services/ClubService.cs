using CricArena.Business.DTOs.Club;
using CricArena.Business.Exceptions;
using CricArena.Business.Services.Interfaces;
using CricArena.Core.Entities;
using CricArena.Core.Enums;
using CricArena.Data.Context;
using CricArena.Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Services
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly ILogger<ClubService> _logger;
        private readonly AppDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMembershipRepository _membershipRepository;

        public ClubService(IClubRepository clubRepository, ILogger<ClubService> logger, AppDbContext dbContext, ICurrentUserService currentUserService, IPlayerRepository playerRepository, IMembershipRepository membershipRepository)
        {
            _clubRepository = clubRepository;
            _logger = logger;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _playerRepository = playerRepository;
            _membershipRepository = membershipRepository;
        }

        public async Task<ClubResponse> CreateClubAsync(CreateClubRequest request)
        {
            ValidateCreateClubRequest(request);
            _logger.LogInformation("Creating a new club with name: {ClubName}", request.Name);

            //Fetch the current user's player ID which is required to set the CreatedByPlayerId property of the new club
            var userId = _currentUserService.UserId;
            if (!userId.HasValue || userId.Value == Guid.Empty)
            {
                throw new UnauthorizedAccessException(
                    "The authenticated user could not be identified.");
            }

            var player = await _playerRepository.GetPlayerByUserIdAsync(userId.Value);
            var playerId = player?.Id ?? throw new PlayerNotFoundException(player?.Id ?? Guid.Empty);

            var newClub = new Club
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Location = request.Location,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedByPlayerId = playerId
            };
            await _clubRepository.AddAsync(newClub);

            _logger.LogInformation("Creating membership for the club creator with player ID: {PlayerId}", playerId);
            var newMembership = new Membership
            {
                Id = Guid.NewGuid(),
                ClubId = newClub.Id,
                PlayerId = playerId,
                Role = ClubRole.ClubAdmin,
                JoinedOn = DateTime.UtcNow,
                Status = MembershipStatus.Approved
            };
            await _membershipRepository.AddAsync(newMembership);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("New club created with ID: {ClubId}", newClub.Id);
            _logger.LogInformation("Membership created for player ID: {PlayerId} in club ID: {ClubId}", playerId, newClub.Id);

            return new ClubResponse
            {
                Id = newClub.Id,
                Name = newClub.Name,
                Description = newClub.Description,
                Location = newClub.Location,
                IsActive = newClub.IsActive,
                CreatedOn = newClub.CreatedOn,
                CreatedByPlayerId = newClub.CreatedByPlayerId
            };
        }

        public async Task DeleteClubAsync(Guid id)
        {
            var (isAssociated, role) = await IsAssociatedWithClub(id);

            if (!isAssociated && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("The authenticated user is not a member of the specified club.");
            }

            if (isAssociated && role != ClubRole.ClubAdmin && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Only club administrators can delete this club.");
            }

            _logger.LogInformation("Deleting club with ID: {ClubId}", id);
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                _logger.LogWarning("Club with ID: {ClubId} not found.", id);
                throw new ClubNotFoundException(id);
            }

            await _clubRepository.DeleteAsync(club);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Club deleted with ID: {ClubId}", id);
        }

        public async Task<List<ClubResponse>> GetAllClubsAsync()
        {
            _logger.LogInformation("Fetching all clubs from the repository.");
            var clubs = await _clubRepository.GetAllAsync();
            return clubs.Select(c => new ClubResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Location = c.Location,
                IsActive = c.IsActive,
                CreatedOn = c.CreatedOn,
                CreatedByPlayerId = c.CreatedByPlayerId
            }).ToList();
        }

        public async Task<ClubResponse?> GetClubByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching club with ID: {ClubId}", id);
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                _logger.LogWarning("Club with ID: {ClubId} not found.", id);
                throw new ClubNotFoundException(id);
            }

            return new ClubResponse
            {
                Id = club.Id,
                Name = club.Name,
                Description = club.Description,
                Location = club.Location,
                IsActive = club.IsActive,
                CreatedOn = club.CreatedOn,
                CreatedByPlayerId = club.CreatedByPlayerId
            };
        }

        public async Task<List<ClubMemberResponse>> GetMembersAsync(Guid clubId)
        {
            var (isAssociated, role) = await IsAssociatedWithClub(clubId);

            if (!isAssociated && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("The authenticated user is not a member of the specified club.");
            }

            _logger.LogInformation("Fetching members for club with ID: {ClubId}", clubId);
            var members = await _membershipRepository.GetByClubIdAsync(clubId);
            return members.Select(m => new ClubMemberResponse
            {
                PlayerId = m.PlayerId,
                PlayerName = m.Player.Name,
                Email = m.Player.Email,
                PhoneNumber = m.Player.PhoneNumber,
                Role = m.Role.ToString(),
                Status = m.Status.ToString(),
                JoinedOn = m.JoinedOn
            }).ToList();
        }

        public async Task UpdateClubAsync(Guid id, UpdateClubRequest request)
        {
            var (isAssociated, role) = await IsAssociatedWithClub(id);

            if (!isAssociated && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("The authenticated user is not a member of the specified club.");
            }
            if (isAssociated && role != ClubRole.ClubAdmin && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Only club administrators can update this club.");
            }

            _logger.LogInformation("Updating club with ID: {ClubId}", id);
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                _logger.LogWarning("Club with ID: {ClubId} not found.", id);
                throw new ClubNotFoundException(id);
            }

            club.Name = request.Name;
            club.Description = request.Description;
            club.Location = request.Location;

            await _clubRepository.UpdateAsync(club);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Club updated with ID: {ClubId}", id);
        }

        public async Task UpdateMemberRoleAsync(Guid clubId, Guid playerId, UpdateMemberRoleRequest request)
        {
            var (isAssociated, role) = await IsAssociatedWithClub(clubId);

            if (!isAssociated && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("The authenticated user is not a member of the specified club.");
            }
            if (isAssociated && role != ClubRole.ClubAdmin && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Only club administrators can update member roles.");
            }
            if (request.Role != ClubRole.Player &&
                request.Role != ClubRole.Admin &&
                request.Role != ClubRole.Captain &&
                request.Role != ClubRole.ClubAdmin)
            {
                throw new ArgumentException("Invalid role specified.");
            }
            if (clubId == Guid.Empty || playerId == Guid.Empty)
            {
                throw new ArgumentException("Club ID and Player ID must be valid GUIDs.");
            }

            _logger.LogInformation("Updating role for player ID: {PlayerId} in club ID: {ClubId} to role: {Role}", playerId, clubId, request.Role);
            var membership = await _membershipRepository.GetByClubAndPlayerAsync(clubId, playerId);
            if (membership == null)
            {
                throw new MembershipNotFoundException(clubId, playerId);
            }

            var updatedMembership = new Membership
            {
                Id = membership.Id,
                ClubId = membership.ClubId,
                PlayerId = membership.PlayerId,
                Role = request.Role,
                JoinedOn = membership.JoinedOn,
                Status = membership.Status,
            };
            await _membershipRepository.UpdateAsync(updatedMembership);
            await _dbContext.SaveChangesAsync();
        }

        private static void ValidateCreateClubRequest(CreateClubRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Club name is required.");
            }
            if (string.IsNullOrWhiteSpace(request.Location))
            {
                throw new ArgumentException("Club location is required.");
            }
        }

        private async Task<(bool isAssociated, ClubRole? role)> IsAssociatedWithClub(Guid clubId)
        {
            var userId = _currentUserService.UserId;
            var player = await _playerRepository.GetPlayerByUserIdAsync(userId ?? Guid.Empty);
            var playerId = player?.Id;

            var membership = await _membershipRepository.GetByClubAndPlayerAsync(clubId, playerId ?? Guid.Empty);
            if (membership == null)
            {
                return (false, null);
            }
            else if (membership.Status != MembershipStatus.Approved)
            {
                return (false, null);
            }
            else
            {
                return (true, membership.Role);
            }
        }
    }
}
