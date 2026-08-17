namespace CricArena.Business.DTOs.Invitation
{
    public class InvitationResponse
    {
        public Guid Id { get; set; }
        public Guid ClubId { get; set; }
        public Guid PlayerId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SentOn { get; set; }
        public DateTime? RespondedOn { get; set; }
    }
}
