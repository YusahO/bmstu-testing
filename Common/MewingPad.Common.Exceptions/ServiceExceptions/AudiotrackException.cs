namespace MewingPad.Common.Exceptions;

public class AudiotrackExistsException : BaseException
{
    public AudiotrackExistsException() : base() { }
    public AudiotrackExistsException(Guid id) : base($"Audiotrack ID = {id} already exists") { }
    public AudiotrackExistsException(string message) : base(message) { }
    public AudiotrackExistsException(string message, Exception innerException) : base(message, innerException) { }
    public AudiotrackExistsException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}
public class AudiotrackNotFoundException : BaseException 
{
    public AudiotrackNotFoundException() : base() { }
    public AudiotrackNotFoundException(Guid id) : base($"Audiotrack ID = {id} not found") { }
    public AudiotrackNotFoundException(string message) : base(message) { }
    public AudiotrackNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    public AudiotrackNotFoundException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class AudiotrackServerUploadException : BaseException 
{
    public AudiotrackServerUploadException() : base() { }
    public AudiotrackServerUploadException(string message) : base(message) { }
    public AudiotrackServerUploadException(string message, Exception innerException) : base(message, innerException) { }
    public AudiotrackServerUploadException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class AudiotrackServerUpdateException : BaseException 
{
    public AudiotrackServerUpdateException() : base() { }
    public AudiotrackServerUpdateException(string message) : base(message) { }
    public AudiotrackServerUpdateException(string message, Exception innerException) : base(message, innerException) { }
    public AudiotrackServerUpdateException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class AudiotrackServerDeleteException : BaseException 
{
    public AudiotrackServerDeleteException() : base() { }
    public AudiotrackServerDeleteException(string message) : base(message) { }
    public AudiotrackServerDeleteException(string message, Exception innerException) : base(message, innerException) { }
    public AudiotrackServerDeleteException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class AudiotrackServerGetException : BaseException 
{
    public AudiotrackServerGetException() : base() { }
    public AudiotrackServerGetException(string message) : base(message) { }
    public AudiotrackServerGetException(string message, Exception innerException) : base(message, innerException) { }
    public AudiotrackServerGetException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}