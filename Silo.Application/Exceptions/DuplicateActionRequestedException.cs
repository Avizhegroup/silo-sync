namespace Silo.Application.Shared.Exceptions;

public class DuplicateActionRequestedException : Exception
{
    public string ? ActionName { get; set; }
    public string? ActionCode { get; set; }

    public DuplicateActionRequestedException(string? actionName, string? actionCode) 
    {
        ActionName = actionName;
        ActionCode = actionCode;
    }
}
