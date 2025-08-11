namespace EC.Protobuf.Exceptions;

public class NotFoundEntryAssemblyException : Exception
{
    public NotFoundEntryAssemblyException() { }
    public NotFoundEntryAssemblyException(string message) : base(message) { }
    public NotFoundEntryAssemblyException(string message, Exception inner) : base(message, inner) { }
}
