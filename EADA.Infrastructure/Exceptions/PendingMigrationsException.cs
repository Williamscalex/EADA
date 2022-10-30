namespace EADA.Infrastructure.Exceptions;

/// <summary>
/// Represents an error that occurs when pending migrations are detected at runtime.
/// </summary>
public class PendingMigrationsException : Exception
{
    private const string DefaultMessage =
        "The database could not be initialized because pending migrations were detected.";

    public PendingMigrationsException() : base(DefaultMessage)
    {
    }

    public PendingMigrationsException(Exception innException) : base(DefaultMessage, innException)
    {
    }
}