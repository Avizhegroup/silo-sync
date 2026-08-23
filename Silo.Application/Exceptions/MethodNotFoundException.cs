namespace Silo.Application.Exceptions;

public class MethodNotFoundException : ApplicationException
{
    public MethodNotFoundException(string data):base($"Cannot find method with signature: {data}")
    {
    }
}
