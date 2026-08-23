namespace Silo.Application.Exceptions;

public class MethodExecutionFailedException : ApplicationException
{
    public MethodExecutionFailedException() : base(TextResources.APP_StringKeys_Error_Unexpected)
    {
    }
}
