namespace CricArena.Business.Exceptions;

public class PlayerNotFoundException : Exception
{
    public PlayerNotFoundException(Guid id)
        : base($"Player '{id}' was not found.")
    {
    }
}