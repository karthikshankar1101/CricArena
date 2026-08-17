namespace CricArena.Business.Exceptions
{
    public class DuplicatePhoneNumberException : Exception
    {
        public DuplicatePhoneNumberException(string phoneNumber)
            : base($"A player with phone number '{phoneNumber}' already exists.")
        {
        }
    }
}
