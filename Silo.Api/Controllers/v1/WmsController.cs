using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Mvc;
using Silo.Api.Business;
using Silo.Application;
using Silo.Base.Controllers.Base;
using Silo.Domains.Android;
using Silo.Domains.Services;

namespace Silo.Api.Controllers.v1;

public class WmsController : SiloBaseController
{
    private readonly ILogger<WmsController> logger;
    private readonly IWmsBusiness business;
    private readonly WmsApiContext apiContext;
    private readonly WmsAndroidContext context;
    private readonly NotificationBusiness notificationBusiness;
    private readonly CustomerGuaranteeCheckBusiness customerGuaranteeBusiness;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;

    public WmsController(ILogger<WmsController> logger
        , IWmsBusiness business
        , WmsApiContext apiContext
        , WmsAndroidContext context
        , NotificationBusiness notificationBusiness
        , CustomerGuaranteeCheckBusiness customerGuaranteeBusiness
        , IMapper mapper
        , IConfiguration configuration) : base(logger)
    {
        this.logger = logger;
        this.business = business;
        this.apiContext = apiContext;
        this.context = context;
        this.notificationBusiness = notificationBusiness;
        this.customerGuaranteeBusiness = customerGuaranteeBusiness;
        this.mapper = mapper;
        this.configuration = configuration;
    }

    /// <summary>
    /// Send request object for execute it's method from specified business
    /// 1- Sort parameters and their datatypes. Method is not sensetive to parameter name spell
    /// 2- Http status code meanings:
    /// HttpStatusCode.ServiceUnavailable: SqlException
    /// HttpStatusCode.BadGateway: SqliteException
    /// HttpStatusCode.NotFound: MethodNotFoundException
    /// HttpStatusCode.BadRequest: Unhandled Exception
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    public async Task<IActionResult> Post(ApiRequest request)
    => Ok(ProccessRequest(request, business));

    /// <summary>
    /// Send request object for execute it's method from specified business
    /// 1- Sort parameters and their datatypes. Method is not case-sensetiveS
    /// 2- Http status code meanings:
    /// HttpStatusCode.ServiceUnavailable: SqlException
    /// HttpStatusCode.BadGateway: SqliteException
    /// HttpStatusCode.NotFound: MethodNotFoundException
    /// HttpStatusCode.BadRequest: Unhandled Exception
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    public async Task<IActionResult> PostObject(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, business));

    /// <summary>
    /// Send request object for execute methods of Notification business 
    /// 1- Sort parameters and their datatypes. Method is not case-sensetive
    /// 2- Http status code meanings:
    /// HttpStatusCode.ServiceUnavailable: SqlException
    /// HttpStatusCode.BadGateway: SqliteException
    /// HttpStatusCode.NotFound: MethodNotFoundException
    /// HttpStatusCode.BadRequest: Unhandled Exception
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    public async Task<IActionResult> Notification(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, notificationBusiness));

    [HttpPost("[action]")]
    public async Task<IActionResult> GetLatestDatabase(string? userToken)
    {
        logger.LogInformation("GetLatestDatabase: userToken=> " + userToken);

        var lst = TextResourceTools.GetTextResourceList(configuration);

        context.DeleteData();

        var tags = ((WmsBusiness)business).SGetAllTag().Select().ToList();

        List<Tag> data = new();

        foreach (DataRow row in tags)
        {
            if (row.ItemArray[2].ToString().HasNoValue())
            {
                continue;
            }

            if (row.ItemArray[8].ToString().HasNoValue())
            {
                continue;
            }

            data.Add(new()
            {
                ProductCode = row.ItemArray[0].ToString(),
                Serial = row.ItemArray[1].ToString(),
                ProductCount = row.ItemArray[3].ToString(),
                TagEpc = row.ItemArray[4].ToString(),
                TagStatus = row.ItemArray[5].ToString(),
                TagZone = row.ItemArray[6].ToString(),
                technicalCode = row.ItemArray[7].ToString(),
                //Name = row.ItemArray[8].ToString(),
                Id = (int)row.ItemArray[9],
                TagInDestination = row.ItemArray[10].ToString(),
                ProductProperties = row.ItemArray[11].ToString()
            });
        }

        var products = ((WmsBusiness)business).SPSearchProduct("-1", "-1", "-1", "-1").Select().ToList();

        List<Product> dataProducts = new();

        foreach (DataRow row in products)
        {
            if (string.IsNullOrEmpty(row.ItemArray[0].ToString()))
            {
                continue;
            }

            dataProducts.Add(new()
            {
                ProductCode = row.ItemArray[1].ToString(),
                ProductName = row.ItemArray[2].ToString(),
                Quality = row.ItemArray[17].ToString(),
                TechnicalCode = row.ItemArray[9].ToString(),
                ProductStatus = row.ItemArray[14].ToString(),
                Id = (int)row.ItemArray[0],
                ProductBrandCode = row.ItemArray[22].ToString(),
                ProductGroupCode = row.ItemArray[20].ToString(),
                ProductSizeCode = row.ItemArray[18].ToString(),
                ProductTypeCode = row.ItemArray[11].ToString()
            });
        }

        List<InspectElement> elements = ((WmsBusiness)business)
                                              .SGetAllElementsDataTable()
                                              .Select()
                                              .Select(p => new InspectElement()
                                              {
                                                  Id = (int)p.ItemArray[0],
                                                  Name = p.ItemArray[1].ToString(),
                                                  InspectElementType = (Domains.Android.InspectElementType)p.ItemArray[2],
                                                  Value = p.ItemArray[3].ToString(),
                                                  MinValue = p.ItemArray[4] is not null ? (int)p.ItemArray[4] : 0,
                                                  MaxValue = p.ItemArray[5] is not null ? (int)p.ItemArray[5] : 0,
                                                  Prevent = (bool)p.ItemArray[6],
                                                  IsActive = (bool)p.ItemArray[7],
                                                  IsRequired = (bool)p.ItemArray[8],
                                                  ProductTypes = p.ItemArray[9].ToString(),
                                                  Options = p.ItemArray[10].ToString(),
                                                  RowIdentifier = (int)p.ItemArray[11]
                                              }).ToList();

        List<CustomerAccountingData> customerAccounts = ((WmsBusiness)business)
                                              .SGetLastCAD()
                                              .Select(p => new CustomerAccountingData()
                                              {
                                                  Id = p.Id,
                                                  Count = (decimal)p.ProductCount,
                                                  ProductCode = p.ProductCode
                                              })
                                              .ToList();

        List<InventoryTags> invTags = ((WmsBusiness)business)
                                              .SGetLastInventoryTags()
                                              .Select()
                                              .Select(p => new InventoryTags()
                                              {
                                                  Epc = p.ItemArray[1].ToString(),
                                                  HeaderId = (int)p.ItemArray[2]
                                              })
                                              .DistinctBy(p => p.Epc)
                                              .ToList();

        List<Zone> zones = ((WmsBusiness)business)
                                             .SPGetAllZones()
                                             .Select(p => new Zone()
                                             {
                                                 Id = p.Id,
                                                 Code = p.ZoneCode,
                                                 StoreCode = p.StoreCode,
                                                 Title = p.Title
                                             })
                                             .ToList();

        var fields = ((WmsBusiness)business).SGetDynamicFieldsForAndroid()
                               .Select(p => new AndroidDynamicFields()
                               {
                                   Id = p.Id,
                                   ActionType = p.ActionType.ToString(),
                                   FieldTitle = p.Title,
                                   DefaultValue = p.DefaultValue,
                                   ValueOptions = p.ValueOptions,
                                   ValueType = p.ValueType
                               })
                               .ToList();

        List<Station> stations = ((WmsBusiness)business).SGetAllStationEntities()
            .Where(p => p.MacAddress.HasNoValue())
            .Select(p => new Station()
            {
                Id = p.Id,
                Code = p.Code ?? string.Empty,
                Name = p.Name ?? string.Empty,
                Type = p.Type,
                Status = p.StationStatus,
                Description = p.Desc ?? string.Empty,
                FromDestination = p.From ?? string.Empty,
                ToDestination = p.To ?? string.Empty,
                ActionType = p.ActionType ?? 0
            })
            .ToList();

        List<Destination> destinations = ((WmsBusiness)business).SGetAllWarehouseEntities()
            .Select(p => new Destination()
            {
                Id = p.Id,
                Title = p.Title ?? string.Empty,
                St = p.IsActive ?? 0,
                Desc = p.InventoryType ?? string.Empty,
                Code = p.Code ?? string.Empty,
                Type = p.OperationalType,
                ParentId = p.IsDefault,
                ParentsId = p.Parents ?? string.Empty,
                Epc = p.Permissions ?? string.Empty
            }).ToList();

        var lines = ((WmsBusiness)business).SGetAllLines()
     .Select(p => new AndroidLine()
     {
         Id = p.Code,
         Title = p.Title,
         Data = p.Data ?? string.Empty,
         Desc = p.Desc
     })
     .ToList();




        var shifts = ((WmsBusiness)business).SGetAllShifts()
            .Select()
            .Select(p => new AndroidShift()
            {
                Id = p.ItemArray[0].ToString(),
                Title = p.ItemArray[1].ToString(),
                Data = p.ItemArray[3].ToString(),
                Desc = p.ItemArray[2].ToString(),
                LineId = p.ItemArray[4].ToString()
            }).ToList();

        var actionTypes = ((WmsBusiness)business).SGetActionTypes()
                        .Select(p => new AndroidActionType()
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Code = p.Code,
                            From = p.From,
                            To = p.To,
                            ActiveControls = p.ActiveControls,
                            DocStatusChange = p.DocStatusChange,
                            DocStatusPermitted = p.DocStatusPermitted,
                            RFIDPower = p.RFIDPower
                        })
                        .ToList();

        var textResources = TextResourceTools.GetTextResourceList(configuration).Select(p => new TextResourceEntity()
        {
            Key = p.Key,
            Value = p.Value
        }).ToList();

        var productTypes = ((WmsBusiness)business).SGetAllProductTypes()
                       .Select(p => new AndroidProductType()
                       {
                           ProductTypeId = p.Id,
                           ProductTypeTitle = p.Title,
                           ProductTypeCode = p.Code,
                           ProductTypeParentId = p.ParentId,
                           ProductTypeParentsId = p.ParentsId
                       })
                       .ToList();


        var productBrands = ((WmsBusiness)business).SGetAllProductBrands()
                     .Select(p => new AndroidProductBrand()
                     {
                         ProductBrandCode = p.Code,
                         ProductBrandTitle = p.Title
                     })
                     .ToList();

        var productSize = ((WmsBusiness)business).SGetAllProductSizesList()
                     .Select(p => new AndroidProductSize()
                     {
                         Id = p.Code,
                         Title = p.Title
                     })
                     .ToList();

        var productGroups = ((WmsBusiness)business).SGetAllProductGroups()
                    .Select(p => new AndroidProductGroup()
                    {
                        ProductGroupCode = p.Code,
                        ProductGroupTitle = p.Title
                    })
                    .ToList();


        var productSubGroups = ((WmsBusiness)business).SGetAllProductSubGroups()
                    .Select(p => new AndroidProductSubGroup()
                    {
                        ProductGroupCode = p.ProductGroupCode,
                        ProductSubGroupTitle = p.Title,
                        ProductSubGroupCode = p.Code
                    })
                    .ToList();

        var productStatus = ((WmsBusiness)business).SGetAllQcs()
                   .Select(p => new AndroidProductStatus()
                   {
                       ProductStatusCode = p.Code,
                       ProductStatusTitle = p.Title
                   })
                   .ToList();


        await context.BulkInsertAsync(data);

        await context.BulkInsertAsync(dataProducts);

        await context.BulkInsertAsync(elements);

        await context.BulkInsertAsync(customerAccounts);

        await context.BulkInsertAsync(invTags);

        await context.BulkInsertAsync(zones);

        await context.BulkInsertAsync(fields);

        await context.BulkInsertAsync(stations);

        await context.BulkInsertAsync(destinations);

        await context.BulkInsertAsync(lines);

        await context.BulkInsertAsync(shifts);

        await context.BulkInsertAsync(actionTypes);

        await context.BulkInsertAsync(textResources);

        await context.BulkInsertAsync(productTypes);


        await context.BulkInsertAsync(productBrands);

        await context.BulkInsertAsync(productSize);

        await context.BulkInsertAsync(productGroups);

        await context.BulkInsertAsync(productSubGroups);

        await context.BulkInsertAsync(productStatus);


        if (!Directory.Exists($"{Environment.CurrentDirectory}\\Files\\Temp"))
        {
            Directory.CreateDirectory($"{Environment.CurrentDirectory}\\Files\\Temp");
        }

        string destFile = Environment.CurrentDirectory + $"\\Files\\Temp\\{Guid.NewGuid().ToString().Substring(0, 5)}.db";

        System.IO.File.Copy(Environment.CurrentDirectory + "\\Wms.db", destFile);

        var content = await System.IO.File.ReadAllBytesAsync(destFile);

        return File(content, "application/octet-stream", "Wms.db");
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CustomerGuarantee(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, customerGuaranteeBusiness));
}
