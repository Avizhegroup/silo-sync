namespace Silo.Application.Contracts;
public interface IExternalApiNegotiate
{
    /// <summary>
    /// Get data of product from a foreign api 
    /// </summary>
    Task GetProducts();

    /// <summary>
    /// Request to create serials
    /// </summary>
    /// <param name="count">Count request serials for create</param>
    Task RequestCreateSerial(int count);

    /// <summary>
    /// Request to create serials
    /// </summary>
    /// <param name="operationCode">Opposition api operation code</param>
    /// <param name="fromSerial">From serial range</param>
    /// <param name="toSerial"></param>
    Task RequestCreateSerial(int operationCode, string fromSerial, string toSerial);

    /// <summary>
    /// Get documents from a foreign api
    /// </summary>
    Task GetDocuments();
}
