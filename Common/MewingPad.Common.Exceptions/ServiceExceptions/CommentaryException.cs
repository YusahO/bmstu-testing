namespace MewingPad.Common.Exceptions;

public class CommentaryExistsException : BaseException
{
    public CommentaryExistsException() : base() { }
    public CommentaryExistsException(Guid id) : base($"Commentary ID = {id} already exists") { }
    public CommentaryExistsException(string message) : base(message) { }
    public CommentaryExistsException(string message, Exception innerException) : base(message, innerException) { }
    public CommentaryExistsException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}
public class CommentaryNotFoundException : BaseException 
{
    public CommentaryNotFoundException() : base() { }
    public CommentaryNotFoundException(Guid id) : base($"Commentary ID = {id} not found") { }
    public CommentaryNotFoundException(string message) : base(message) { }
    public CommentaryNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    public CommentaryNotFoundException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}