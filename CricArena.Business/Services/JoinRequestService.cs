using CricArena.Business.DTOs.Auth;
using CricArena.Business.DTOs.JoinRequest;
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
    public class JoinRequestService : IJoinRequestService
    {
        private readonly IJoinRequestRepository _joinRequestRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<JoinRequestService> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMembershipRepository _membershipRepository;

        public JoinRequestService(
            IJoinRequestRepository joinRequestRepository,
            AppDbContext context,
            ILogger<JoinRequestService> logger,
            ICurrentUserService currentUserService,
            IPlayerRepository playerRepository,
            IMembershipRepository membershipRepository)
        {
            _joinRequestRepository = joinRequestRepository;
            _context = context;
            _logger = logger;
            _currentUserService = currentUserService;
            _playerRepository = playerRepository;
            _membershipRepository = membershipRepository;
        }
        public async Task<JoinRequestResponse> CreateAsync(CreateJoinRequestRequest request)
        {
            if (request.ClubId == Guid.Empty)
            {
                throw new ArgumentException("ClubId cannot be empty.");
            }

            var userId = _currentUserService.UserId;
            if (!userId.HasValue || userId.Value == Guid.Empty)
            {
                throw new UnauthorizedAccessException(
                    "The authenticated user could not be identified.");
            }

            var player = await _playerRepository.GetPlayerByUserIdAsync(userId.Value);
            var playerId = player?.Id ?? throw new PlayerNotFoundException(player?.Id ?? Guid.Empty);

            var existingRequest = await _joinRequestRepository.GetByClubIdAndPlayerIdAsync(request.ClubId, playerId);
            if (existingRequest != null)
            {
                throw new InvalidOperationException(
                    $"A join request for ClubId: {request.ClubId} and PlayerId: {playerId} already exists.");
            }

            _logger.LogInformation("Creating join request for ClubId: {ClubId}", request.ClubId);
            var joinRequest = new JoinRequest
            {
                Id = Guid.NewGuid(),
                PlayerId = playerId,
                ClubId = request.ClubId,
                Status = MembershipStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                Remarks = request.Message
            };

            await _joinRequestRepository.AddAsync(joinRequest);
            await _context.SaveChangesAsync();

            return new JoinRequestResponse
            {
                Id = joinRequest.Id,
                PlayerId = joinRequest.PlayerId,
                ClubId = joinRequest.ClubId,
                Status = joinRequest.Status.ToString(),
            };
        }

        public async Task<List<JoinRequestResponse>> GetClubRequestsAsync(Guid clubId)
        {
            if (clubId == Guid.Empty)
            {
                throw new ArgumentException("ClubId cannot be empty.");
            }

            var (isAssociated, role) = await IsAssociatedWithClub(clubId);

            if (!isAssociated && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("The authenticated user is not a member of the specified club.");
            }

            // Allow ClubAdmin or Captain to view club join requests
            if (isAssociated && role != ClubRole.ClubAdmin && role != ClubRole.Captain && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Only club administrators or captains can view join requests.");
            }

            var requests = await _joinRequestRepository.GetByClubIdAsync(clubId);
            return requests.Select(r => new JoinRequestResponse
            {
                Id = r.Id,
                PlayerId = r.PlayerId,
                ClubId = r.ClubId,
                Status = r.Status.ToString(),
                Message = r.Remarks
            }).ToList();
        }
        public async Task ApproveAsync(Guid requestId)
        {
            if (requestId == Guid.Empty)
            {
                throw new ArgumentException("RequestId cannot be empty");
            }

            var joinRequest = await _joinRequestRepository.GetByIdAsync(requestId);
            if (joinRequest == null)
            {
                throw new JoinRequestNotFoundException(requestId);
            }

            if (joinRequest.Status != MembershipStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Join request with ID: {requestId} cannot be approved. Current status: {joinRequest.Status}");
            }

            var (isAssociated, role) = await IsAssociatedWithClub(joinRequest.ClubId);

            if (!isAssociated && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("The authenticated user is not a member of the specified club.");
            }

            // Allow ClubAdmin or Captain to approve join requests
            if (isAssociated && role != ClubRole.ClubAdmin && role != ClubRole.Captain && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Only club administrators or captains can approve join requests.");
            }

            _logger.LogInformation("Approving join request with ID: {RequestId}", requestId);

            // Update join request status to approved
            joinRequest.Status = MembershipStatus.Approved;
            joinRequest.ReviewedOn = DateTime.UtcNow;
            await _joinRequestRepository.UpdateAsync(joinRequest);

            // Create membership entry
            var membership = new Membership
            {
                Id = Guid.NewGuid(),
                PlayerId = joinRequest.PlayerId,
                ClubId = joinRequest.ClubId,
                Role = ClubRole.Player,
                JoinedOn = DateTime.UtcNow,
                Status = MembershipStatus.Approved
            };
            await _membershipRepository.AddAsync(membership);

            // Save all changes
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Join request approved successfully. Created membership for PlayerId: {PlayerId}, ClubId: {ClubId}",
                joinRequest.PlayerId,
                joinRequest.ClubId);
        }

        public async Task RejectAsync(Guid requestId)
        {
            if (requestId == Guid.Empty)
            {
                throw new ArgumentException("RequestId cannot be empty");
            }

            var joinRequest = await _joinRequestRepository.GetByIdAsync(requestId);
            if (joinRequest == null)
            {
                throw new JoinRequestNotFoundException(requestId);
            }

            if (joinRequest.Status != MembershipStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Join request with ID: {requestId} cannot be rejected. Current status: {joinRequest.Status}");
            }

            var (isAssociated, role) = await IsAssociatedWithClub(joinRequest.ClubId);

            if (!isAssociated && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("The authenticated user is not a member of the specified club.");
            }

            // Allow ClubAdmin or Captain to reject join requests
            if (isAssociated && role != ClubRole.ClubAdmin && role != ClubRole.Captain && _currentUserService.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Only club administrators or captains can reject join requests.");
            }

            _logger.LogInformation("Rejecting join request with ID: {RequestId}", requestId);
            joinRequest.Status = MembershipStatus.Rejected;
            joinRequest.ReviewedOn = DateTime.UtcNow;
            await _joinRequestRepository.UpdateAsync(joinRequest);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Join request rejected successfully. Join request ID: {RequestId}",
                joinRequest.Id);
        }

        public async Task CancelAsync(Guid requestId)
        {
            if (requestId == Guid.Empty)
            {
                throw new ArgumentException("RequestId cannot be empty");
            }

            var userId = _currentUserService.UserId;
            var player = await _playerRepository.GetPlayerByUserIdAsync(userId ?? Guid.Empty);
            var playerId = player?.Id;

            var joinRequest = await _joinRequestRepository.GetByIdAsync(requestId);
            if (joinRequest == null)
            {
                throw new JoinRequestNotFoundException(requestId);
            }
            if (joinRequest.Status == MembershipStatus.Approved || joinRequest.Status == MembershipStatus.Rejected)
            {
                throw new InvalidOperationException(
                    $"Join request with ID: {requestId} cannot be canceled as it has already been approved.");
            }
            if (joinRequest.PlayerId != playerId)
            {
                throw new UnauthorizedAccessException(
                    "The authenticated user is not authorized to cancel this join request.");
            }
            if (joinRequest.Status == MembershipStatus.Pending || joinRequest.Status == MembershipStatus.Rejected)
            {
                throw new InvalidOperationException(
                    $"Join request with ID: {requestId} cannot be canceled. Current status: {joinRequest.Status}");
            }

            await _joinRequestRepository.DeleteAsync(joinRequest);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Join request canceled successfully. Join request ID: {RequestId}",
                joinRequest.Id);
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
