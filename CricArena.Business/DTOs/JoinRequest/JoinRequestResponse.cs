namespace CricArena.Business.DTOs.JoinRequest
{
    public class JoinRequestResponse
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid ClubId { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}
