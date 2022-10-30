namespace EADA.Infrastructure.Exceptions;

public class DatabaseInitException : Exception
{
    private const string DefaultMessage =
        "An error occurred while ttempting to initialize the database during startup.";

    public DatabaseInitException() : base(DefaultMessage) { }

    public DatabaseInitException(string message) : base(message) { }
    public DatabaseInitException(string message, Exception innerException) : base(message,innerException) { }
    public DatabaseInitException(Exception innerException) : base(DefaultMessage, innerException) { }

}