namespace CricArena.Business.DTOs.Invitation
{
    public class CreateInvitationRequest
    {
        public Guid ClubId { get; set; }
        public Guid PlayerId { get; set; }
    }
}
