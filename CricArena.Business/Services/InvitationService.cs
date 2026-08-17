using CricArena.Business.DTOs.Invitation;
using CricArena.Business.Exceptions;
using CricArena.Business.Services.Interfaces;
using CricArena.Core.Entities;
using CricArena.Core.Enums;
using CricArena.Data.Context;
using CricArena.Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace CricArena.Business.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppDbContext _context;
        private readonly ILogger<InvitationService> _logger;

        public InvitationService(
            IInvitationRepository invitationRepository,
            IClubRepository clubRepository,
            IPlayerRepository playerRepository,
            IMembershipRepository membershipRepository,
            ICurrentUserService currentUserService,
            AppDbContext context,
            ILogger<InvitationService> logger)
        {
            _invitationRepository = invitationRepository;
            _clubRepository = clubRepository;
            _playerRepository = playerRepository;
            _membershipRepository = membershipRepository;
            _currentUserService = currentUserService;
            _context = context;
            _logger = logger;
        }

        public async Task<List<InvitationPlayerSearchResponse>> SearchPlayersByPhoneNumberAsync(
            Guid clubId,
            string phoneNumber)
        {
            await EnsureCanManageClubAsync(clubId);

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            var players = await _playerRepository.GetByPhoneNumberAsync(phoneNumber.Trim());
            return players.Select(player => new InvitationPlayerSearchResponse
            {
                Id = player.Id,
                Name = player.Name,
                PhoneNumber = player.PhoneNumber
            }).ToList();
        }

        public async Task<InvitationResponse> CreateAsync(CreateInvitationRequest request)
        {
            if (request.ClubId == Guid.Empty || request.PlayerId == Guid.Empty)
            {
                throw new ArgumentException("Club ID and Player ID must be valid GUIDs.");
            }

            await EnsureCanManageClubAsync(request.ClubId);

            var player = await _playerRepository.GetByIdAsync(request.PlayerId);
            if (player == null)
            {
                throw new PlayerNotFoundException(request.PlayerId);
            }

            var existingMembership = await _membershipRepository.GetByClubAndPlayerAsync(
                request.ClubId,
                request.PlayerId);
            if (existingMembership != null)
            {
                throw new InvalidOperationException("The player is already a member of this club.");
            }

            var pendingInvitation = await _invitationRepository.GetPendingByClubIdAndPlayerIdAsync(
                request.ClubId,
                request.PlayerId);
            if (pendingInvitation != null)
            {
                throw new InvalidOperationException("The player already has a pending invitation for this club.");
            }

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                ClubId = request.ClubId,
                PlayerId = request.PlayerId,
                SentOn = DateTime.UtcNow,
                Status = InvitationStatus.Pending
            };

            await _invitationRepository.AddAsync(invitation);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created invitation {InvitationId} for player {PlayerId} and club {ClubId}.", invitation.Id, invitation.PlayerId, invitation.ClubId);

            return MapToResponse(invitation);
        }

        public async Task<List<InvitationResponse>> GetClubInvitationsAsync(Guid clubId)
        {
            await EnsureCanManageClubAsync(clubId);
            var invitations = await _invitationRepository.GetByClubIdAsync(clubId);
            return invitations.Select(MapToResponse).ToList();
        }

        public async Task<List<InvitationResponse>> GetMyInvitationsAsync()
        {
            var player = await GetCurrentPlayerAsync();
            var invitations = await _invitationRepository.GetByPlayerIdAsync(player.Id);
            return invitations.Select(MapToResponse).ToList();
        }

        public async Task<InvitationResponse> GetByIdAsync(Guid invitationId)
        {
            var invitation = await GetInvitationAsync(invitationId);
            var currentPlayer = await GetCurrentPlayerAsync();

            if (invitation.PlayerId != currentPlayer.Id)
            {
                await EnsureCanManageClubAsync(invitation.ClubId);
            }

            return MapToResponse(invitation);
        }

        public async Task AcceptAsync(Guid invitationId)
        {
            var invitation = await GetInvitationAsync(invitationId);
            await EnsureRecipientAsync(invitation);
            EnsurePending(invitation, "accepted");

            var existingMembership = await _membershipRepository.GetByClubAndPlayerAsync(
                invitation.ClubId,
                invitation.PlayerId);
            if (existingMembership != null)
            {
                throw new InvalidOperationException("The player is already a member of this club.");
            }

            invitation.Status = InvitationStatus.Accepted;
            invitation.RespondedOn = DateTime.UtcNow;
            await _invitationRepository.UpdateAsync(invitation);

            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                ClubId = invitation.ClubId,
                PlayerId = invitation.PlayerId,
                Role = ClubRole.Player,
                Status = MembershipStatus.Approved,
                JoinedOn = DateTime.UtcNow
            };
            await _membershipRepository.AddAsync(membership);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Accepted invitation {InvitationId}.", invitationId);
        }

        public async Task RejectAsync(Guid invitationId)
        {
            var invitation = await GetInvitationAsync(invitationId);
            await EnsureRecipientAsync(invitation);
            EnsurePending(invitation, "rejected");

            invitation.Status = InvitationStatus.Rejected;
            invitation.RespondedOn = DateTime.UtcNow;
            await _invitationRepository.UpdateAsync(invitation);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Rejected invitation {InvitationId}.", invitationId);
        }

        public async Task CancelAsync(Guid invitationId)
        {
            var invitation = await GetInvitationAsync(invitationId);
            await EnsureCanManageClubAsync(invitation.ClubId);
            EnsurePending(invitation, "canceled");

            await _invitationRepository.DeleteAsync(invitation);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Canceled invitation {InvitationId}.", invitationId);
        }

        private async Task<Invitation> GetInvitationAsync(Guid invitationId)
        {
            if (invitationId == Guid.Empty)
            {
                throw new ArgumentException("Invitation ID must be a valid GUID.");
            }

            return await _invitationRepository.GetByIdAsync(invitationId)
                ?? throw new InvitationNotFoundException(invitationId);
        }

        private async Task<Player> GetCurrentPlayerAsync()
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue || userId.Value == Guid.Empty)
            {
                throw new UnauthorizedAccessException("The authenticated user could not be identified.");
            }

            return await _playerRepository.GetPlayerByUserIdAsync(userId.Value)
                ?? throw new PlayerNotFoundException(Guid.Empty);
        }

        private async Task EnsureRecipientAsync(Invitation invitation)
        {
            var currentPlayer = await GetCurrentPlayerAsync();
            if (currentPlayer.Id != invitation.PlayerId)
            {
                throw new UnauthorizedAccessException("Only the invited player can respond to this invitation.");
            }
        }

        private async Task EnsureCanManageClubAsync(Guid clubId)
        {
            if (clubId == Guid.Empty)
            {
                throw new ArgumentException("Club ID must be a valid GUID.");
            }

            var club = await _clubRepository.GetByIdAsync(clubId);
            if (club == null)
            {
                throw new ClubNotFoundException(clubId);
            }

            if (_currentUserService.Role == "Admin")
            {
                return;
            }

            var currentPlayer = await GetCurrentPlayerAsync();
            var membership = await _membershipRepository.GetByClubAndPlayerAsync(clubId, currentPlayer.Id);
            if (membership == null || membership.Status != MembershipStatus.Approved ||
                (membership.Role != ClubRole.ClubAdmin && membership.Role != ClubRole.Captain))
            {
                throw new UnauthorizedAccessException("Only club administrators or captains can manage invitations.");
            }
        }

        private static void EnsurePending(Invitation invitation, string action)
        {
            if (invitation.Status != InvitationStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Invitation with ID: {invitation.Id} cannot be {action}. Current status: {invitation.Status}.");
            }
        }

        private static InvitationResponse MapToResponse(Invitation invitation)
        {
            return new InvitationResponse
            {
                Id = invitation.Id,
                ClubId = invitation.ClubId,
                PlayerId = invitation.PlayerId,
                Status = invitation.Status.ToString(),
                SentOn = invitation.SentOn,
                RespondedOn = invitation.RespondedOn
            };
        }
    }
}
