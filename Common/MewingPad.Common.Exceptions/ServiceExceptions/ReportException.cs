namespace MewingPad.Common.Exceptions;

public class ReportExistsException : BaseException
{
    public ReportExistsException() : base() { }
    public ReportExistsException(Guid id) : base($"Report with ID = {id} already exists") { }
    public ReportExistsException(string message) : base(message) { }
    public ReportExistsException(string message, Exception innerException) : base(message, innerException) { }
    public ReportExistsException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}
public class ReportNotFoundException : BaseException 
{
    public ReportNotFoundException() : base() { }
    public ReportNotFoundException(Guid id) : base($"Report with ID = {id} not found") { }
    public ReportNotFoundException(string message) : base(message) { }
    public ReportNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    public ReportNotFoundException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}