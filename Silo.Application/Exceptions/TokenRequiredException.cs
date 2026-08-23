namespace Silo.Application.Exceptions;

public class TokenRequiredException : Exception       
{
	public TokenRequiredException() 
		: base(TextResources.APP_StringKeys_Error_TokenRequiredException)	
	{}
}
