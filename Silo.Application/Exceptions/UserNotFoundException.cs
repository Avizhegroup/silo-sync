namespace Silo.Application.Exceptions;

public class UserNotFoundException : Exception
{
	public UserNotFoundException() 
		: base(TextResources.APP_StringKeys_Error_UserNotFoundException) 
	{
	}
}
