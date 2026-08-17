namespace CricArena.Business.DTOs.Invitation
{
    public class InvitationPlayerSearchResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
