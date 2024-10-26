namespace RoomSchedulerAPI.Core.Exceptions;

public static class CustomExceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException()
            : base($"User already exists")
        {
        }
    }
}
