namespace MewingPad.Common.Exceptions;

public class RepositoryException : BaseException
{
    public RepositoryException() : base() { }
    public RepositoryException(string message) : base(message) { }
    public RepositoryException(string message, Exception? innerException) : base(message, innerException) { }
    public RepositoryException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}