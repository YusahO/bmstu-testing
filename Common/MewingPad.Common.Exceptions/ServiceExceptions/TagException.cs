namespace MewingPad.Common.Exceptions;

public class TagExistsException : BaseException
{
    public TagExistsException() : base() { }
    public TagExistsException(Guid id) : base($"Tag ID = {id} already exists") { }
    public TagExistsException(string message) : base(message) { }
    public TagExistsException(string message, Exception innerException) : base(message, innerException) { }
    public TagExistsException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}
public class TagNotFoundException : BaseException 
{
    public TagNotFoundException() : base() { }
    public TagNotFoundException(Guid id) : base($"Tag ID = {id} not found") { }
    public TagNotFoundException(string message) : base(message) { }
    public TagNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    public TagNotFoundException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}