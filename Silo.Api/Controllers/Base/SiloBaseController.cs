using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Silo.Api.Business;
using Silo.Application.Exceptions;

namespace Silo.Base.Controllers.Base;

/// <summary>
/// In .net 7 because of json serializing cannot serialize utf-8 char at serialization
/// Skip and Use newtonsoft
/// </summary>
[ApiController]
[EnableCors("OpenCors")]
[Route("RfidCore/v{version:apiversion}/[controller]")]
public class SiloBaseController : ControllerBase
{
    internal ILogger logger;
    internal ProjectBusiness business;

    public SiloBaseController(ILogger<SiloBaseController> logger)
    {
        this.logger = logger;
    }

    public SiloBaseController(ILogger<SiloBaseController> logger
        , ProjectBusiness business)
    {
        this.logger = logger;
        this.business = business;
    }

    protected ApiResponse ProccessRequest(ApiRequest request)
    {
        try
        {
            var type = business.GetType();

            var methodInfo = type.GetMethod(request.Method);

            var data = JToken.Parse(request.Parameters.ToString());

            List<object?> objs = new();

            var parameters = methodInfo.GetParameters();

            var i = 0;

            foreach (JToken dataItem in data)
            {
                if (i >= parameters.Length)
                    break;

                var parameterItem = parameters[i];
                objs.Add(((JProperty)dataItem).Value.ToObject(parameterItem.ParameterType));
                i++;
            }

            if (methodInfo.GetParameters().Count() > objs.Count)
            {
                for (int j = 0; j < methodInfo.GetParameters().Count() - objs.Count; j++)
                {
                    objs.Add(null);
                }
            }

            var result = methodInfo.Invoke(business, objs.ToArray());

            if (result is DataTable)
            {
                return new ApiResponse()
                {
                    Successful = true,
                    Value = JsonConvert.SerializeObject(result)
                };
            }
            else
            {
                return new ApiResponse()
                {
                    Successful = true,
                    Value = result
                };
            }
        }
        catch (JsonReaderException ex)
        {
            logger.LogWarning(ex, ex.Message);

            throw new MethodNotFoundException(System.Text.Json.JsonSerializer.Serialize(request));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected ApiResponse ProccessRequest(ApiRequest request
        , object passedObject)
    {
        try
        {
            var type = passedObject.GetType();

            var methodInfo = type.GetMethod(request.Method);

            var data = JToken.Parse(request.Parameters.ToString());

            List<object?> objs = new();

            var parameters = methodInfo.GetParameters();

            var i = 0;

            foreach (JToken dataItem in data)
            {
                if (i >= parameters.Length)
                    break;

                var parameterItem = parameters[i];
                objs.Add(((JProperty)dataItem).Value.ToObject(parameterItem.ParameterType));
                i++;
            }

            if (methodInfo.GetParameters().Count() > objs.Count)
            {
                for (int j = 0; j < methodInfo.GetParameters().Count() - objs.Count; j++)
                {
                    objs.Add(null);
                }
            }

            var result = methodInfo.Invoke(passedObject, objs.ToArray());

            if (result is DataTable)
            {
                return new ApiResponse()
                {
                    Successful = true,
                    Value = JsonConvert.SerializeObject(result)
                };
            }
            else
            {
                return new ApiResponse()
                {
                    Successful = true,
                    Value = result
                };
            }
        }
        catch (JsonReaderException ex)
        {
            logger.LogWarning(ex, ex.Message);

            throw new MethodNotFoundException(System.Text.Json.JsonSerializer.Serialize(request));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected ApiResponse ProccessRequestObjectList(ApiRequest request)
    {
        try
        {
            var type = business.GetType();

            var methodInfo = type.GetMethod(request.Method);

            var data = JToken.Parse(request.Parameters.ToString());

            List<object?> objs = new();

            var parameters = methodInfo.GetParameters();

            var i = 0;

            foreach (JToken dataItem in data)
            {
                if (i >= parameters.Length)
                    break;

                var parameterItem = parameters[i];
                objs.Add(((JProperty)dataItem).Value.ToObject(parameterItem.ParameterType));
                i++;
            }

            if (methodInfo.GetParameters().Count() > objs.Count)
            {
                for (int j = 0; j < methodInfo.GetParameters().Count() - objs.Count; j++)
                {
                    objs.Add(null);
                }
            }

            var result = methodInfo.Invoke(business, objs.ToArray());

            if (result is DataTable resultDt)
            {
                return new ApiResponse()
                {
                    Successful = true,
                    Value = DataTableTools.DataTableToObjects(resultDt)
                };
            }
            else
            {
                return new ApiResponse()
                {
                    Successful = true,
                    Value = result
                };
            }
        }
        catch (JsonReaderException ex)
        {
            logger.LogWarning(ex, ex.Message);

            throw new MethodNotFoundException(System.Text.Json.JsonSerializer.Serialize(request));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected ApiResponse ProccessRequestObjectListByBusiness(ApiRequest request
        , object passedBusiness)
    {
        try
        {
            var type = passedBusiness.GetType();

            var methodInfo = type.GetMethod(request.Method);

            var data = JToken.Parse(request.Parameters.ToString());

            List<object?> objs = new();

            var parameters = methodInfo.GetParameters();

            var i = 0;

            foreach (JToken dataItem in data)
            {
                if (i >= parameters.Length)
                    break;

                var parameterItem = parameters[i];
                objs.Add(((JProperty)dataItem).Value.ToObject(parameterItem.ParameterType));
                i++;
            }

            if (methodInfo.GetParameters().Count() > objs.Count)
            {
                for (int j = 0; j < methodInfo.GetParameters().Count() - objs.Count; j++)
                {
                    objs.Add(null);
                }
            }

            var result = methodInfo.Invoke(passedBusiness, objs.ToArray());

            if (result is DataTable resultDt)
            {
                return new ApiResponse()
                {
                    Successful = true,
                    Value = DataTableTools.DataTableToObjects(resultDt)
                };
            }
            else
            {
                return new ApiResponse()
                {
                    Successful = true,
                    Value = result
                };
            }
        }
        catch (JsonReaderException ex)
        {
            logger.LogWarning(ex, ex.Message);

            throw new MethodNotFoundException(System.Text.Json.JsonSerializer.Serialize(request));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
