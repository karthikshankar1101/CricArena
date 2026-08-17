namespace CricArena.Business.Exceptions
{
    public class InvitationNotFoundException : Exception
    {
        public InvitationNotFoundException(Guid id)
            : base($"Invitation '{id}' was not found.")
        {
        }
    }
}
