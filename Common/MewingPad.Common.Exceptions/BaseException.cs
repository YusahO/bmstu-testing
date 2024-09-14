namespace MewingPad.Common.Exceptions;

public class BaseException : Exception
{
    public BaseException() : base() { }
    public BaseException(string message) : base(message) { }
    public BaseException(string message, Exception? innerException) : base(message, innerException) { }
    public BaseException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
    public int ErrorCode { get; protected set; }
}