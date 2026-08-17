using CricArena.Business.DTOs.Invitation;

namespace CricArena.Business.Services.Interfaces
{
    public interface IInvitationService
    {
        Task<List<InvitationPlayerSearchResponse>> SearchPlayersByPhoneNumberAsync(Guid clubId, string phoneNumber);
        Task<InvitationResponse> CreateAsync(CreateInvitationRequest request);
        Task<List<InvitationResponse>> GetClubInvitationsAsync(Guid clubId);
        Task<List<InvitationResponse>> GetMyInvitationsAsync();
        Task<InvitationResponse> GetByIdAsync(Guid invitationId);
        Task AcceptAsync(Guid invitationId);
        Task RejectAsync(Guid invitationId);
        Task CancelAsync(Guid invitationId);
    }
}
