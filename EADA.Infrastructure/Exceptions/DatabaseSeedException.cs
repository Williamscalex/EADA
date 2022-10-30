namespace EADA.Infrastructure.Exceptions;

public sealed class DatabaseSeedException : Exception
{
    public const string DefaultMessage =
        "An error occurred while attempting to seed  the dtatbase. See inner exceptions(s) for details.";
    public DatabaseSeedException(Exception e) : base(DefaultMessage,e){}
}