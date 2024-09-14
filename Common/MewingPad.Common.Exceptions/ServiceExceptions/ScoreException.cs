namespace MewingPad.Common.Exceptions;

public class ScoreExistsException : BaseException
{
    public ScoreExistsException() : base() { }
    public ScoreExistsException(Guid authorId, Guid audiofileId) : base($"Score with author ID = {authorId}, audiofile ID = {audiofileId} already exists") { }
    public ScoreExistsException(string message) : base(message) { }
    public ScoreExistsException(string message, Exception innerException) : base(message, innerException) { }
    public ScoreExistsException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}
public class ScoreNotFoundException : BaseException
{
    public ScoreNotFoundException() : base() { }
    public ScoreNotFoundException(Guid authorId, Guid audiofileId) : base($"Score with author ID = {authorId}, audiofile ID = {audiofileId} not found") { }
    public ScoreNotFoundException(string message) : base(message) { }
    public ScoreNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    public ScoreNotFoundException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class ScoreInvalidValueException : BaseException
{
    public ScoreInvalidValueException() : base() { }
    public ScoreInvalidValueException(int value) : base($"Score with value = {value} is not allowed") { }
    public ScoreInvalidValueException(string message) : base(message) { }
    public ScoreInvalidValueException(string message, Exception innerException) : base(message, innerException) { }
    public ScoreInvalidValueException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}