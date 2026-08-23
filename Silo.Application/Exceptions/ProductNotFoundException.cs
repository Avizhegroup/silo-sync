namespace Silo.Application.Exceptions;

public class ProductNotFoundException : Exception       
{
	public List<string> Errors;
	public ProductNotFoundException(List<string> errors) 
		  :base(TextResources.APP_StringKeys_Error_ProductCodeNotFound)
	{
        Errors = errors;

        Errors.Add(TextResources.APP_StringKeys_Error_ProductCodeNotFound);
    }
}
