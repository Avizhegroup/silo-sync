using System.IO.Compression;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Silo.Api.Business;
using Silo.Application;
using Silo.Base.Controllers.Base;
using Silo.Domains.Android;
using Silo.Domains.Services;
using Silo.Identity.Server;

namespace Silo.Api.Controllers.v2;

#if DEBUG
[AllowAnonymous]
#endif
public class WmsController : SiloBaseControllerVersion2
{
    private readonly IJwtService jwtService;
    private readonly DocumentBusiness documentBusiness;
    private readonly IWmsBusiness business;
    private readonly WmsApiContext apiContext;
    private readonly WmsAndroidContext context;
    private readonly TruckCrossBusiness truckCrossBusiness;
    private readonly ProductBusiness productBusiness;
    private readonly ReportBusiness reportBusiness;
    private readonly AppSettingsBusiness settingsBusiness;
    private readonly ReportFormatBusiness reportFormatBusiness;
    private readonly InspectBusiness inspectBusiness;
    private readonly DocumentLogBusiness documentLogBusiness;
    private readonly NotificationBusiness notificationBusiness;
    private readonly CustomerGuaranteeCheckBusiness customerGuaranteeBusiness;
    private readonly IConfiguration configuration;

    public WmsController(ILogger<WmsController> logger
         , IWmsBusiness business
         , WmsApiContext apiContext
         , WmsAndroidContext context
         , IJwtService jwtService
         , TruckCrossBusiness truckCrossBusiness
         , DocumentBusiness documentBusiness
         , ProductBusiness productBusiness
         , ReportBusiness reportBusiness
         , InspectBusiness inspectBusiness
         , AppSettingsBusiness settingsBusiness
         , ReportFormatBusiness reportFormatBusiness
         , DocumentLogBusiness documentLogBusiness
         , NotificationBusiness notificationBusiness
         , CustomerGuaranteeCheckBusiness customerGuaranteeBusiness
        , IConfiguration configuration) : base(logger)
    {
        this.business = business;
        this.apiContext = apiContext;
        this.context = context;
        this.jwtService = jwtService;
        this.documentBusiness = documentBusiness;
        this.truckCrossBusiness = truckCrossBusiness;
        this.productBusiness = productBusiness;
        this.reportBusiness = reportBusiness;
        this.inspectBusiness = inspectBusiness;
        this.settingsBusiness = settingsBusiness;
        this.reportFormatBusiness = reportFormatBusiness;
        this.documentLogBusiness = documentLogBusiness;
        this.notificationBusiness = notificationBusiness;
        this.customerGuaranteeBusiness = customerGuaranteeBusiness;
        this.configuration = configuration;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Post(ApiRequest request)
    => Ok(ProccessRequest(request, business));

    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PostObject(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, business));

    [HttpPost("[action]")]
    public async Task<IActionResult> GetLatestDatabase(string? userToken)
    {
        logger.LogInformation("GetLatestDatabase: userToken=> " + userToken);

        var stationCode = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.SerialNumber)?.Value;

        #region Prepare Data

        var allStations = apiContext.Stations.AsNoTracking().ToList();

        List<string> stationTypes = new();

        if (stationCode.HasValue())
        {
            var settings = allStations.FirstOrDefault(p => p.Code == stationCode)?.Settings;

            if (settings.HasValue())
            {
                try
                {
                    stationTypes = JToken.Parse(settings)["ProductTypes"].ToObject<List<string>>();
                }
                catch (Exception)
                {
                }
            }
        }

        var stations = allStations
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
                ActionType = p.ActionType ?? 0,
                MacAddress = p.MacAddress ?? string.Empty
            })
            .ToList();

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
                technicalCode = "",
                Id = (int)row.ItemArray[9],
                TagInDestination = row.ItemArray[10].ToString(),
                ProductProperties = row.ItemArray[11].ToString(),
                FreezStatus = (row.ItemArray[12].ToString() == "عدم فریز") ? "0" : "1",
                InspectStatus = (row.ItemArray[13].ToString() == "بازرسی تأیید") ? "0" : ((row.ItemArray[13].ToString() == "بازرسی نشده") ? "1" : "2"),
                LockStatus = row.ItemArray[14].ToString(),
                MiladiRegisterDate = row.ItemArray[16].ToString(),
                ShamsiRegisterDateUnix = row.ItemArray[15].ToString(),
                TagTreeParentsEpc = row.ItemArray[17].ToString(),
                TagEpcSecond = row.ItemArray[18].ToString()
            });
        }

#if DEBUG
        data = data.Take(50).ToList();
#endif

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
                ProductBrandCode = row.ItemArray[23].ToString(),
                ProductGroupCode = row.ItemArray[20].ToString(),
                ProductSizeCode = row.ItemArray[18].ToString(),
                ProductTypeCode = row.ItemArray[11].ToString(),
                ProductPackValue = row.ItemArray[4].ToString(),
                ProductUnit = row.ItemArray[13].ToString(),
                ProductSubGroup = row.ItemArray[25].ToString(),
                ProductClass = row.ItemArray[26].ToString(),
                ProductValue = row.ItemArray[8].ToString(),
                CountInNextlevelUnit = row.ItemArray[27].ToString(),
                NextlevelUnitTitle = row.ItemArray[28].ToString(),
                HasDoubleTag = (int)row.ItemArray[29] == 1
            });
        }

        if (stationTypes.Any())
        {
            dataProducts = dataProducts
        .Where(p => stationTypes.Contains(p.ProductTypeCode))
        .ToList();
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

        List<CustomerAccountingData> customerAccounts = new();

        foreach (var p in ((WmsBusiness)business).SGetLastCAD())
        {
            try
            {
                customerAccounts.Add(new()
                {
                    Id = p.Id,
                    Count = (decimal)p.ProductCount,
                    ProductCode = p.ProductCode
                });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        List<InventoryTags> invTags = ((WmsBusiness)business)
                                              .SGetLastInventoryTags()
                                              .Select()
                                              .Select(p => new InventoryTags()
                                              {
                                                  Epc = p.ItemArray[1].ToString(),
                                                  HeaderId = (int)p.ItemArray[2],
                                                  StoreCode = p.ItemArray[3].ToString()
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

        var fields = apiContext.DynamicFields
                               .Select(p => new AndroidDynamicFields()
                               {
                                   Id = p.Id,
                                   ActionType = p.ActionType.ToString(),
                                   FieldTitle = p.Title,
                                   DefaultValue = p.DefaultValue,
                                   ValueOptions = p.ValueOptions,
                                   ValueType = p.ValueType,
                                   IsFieldRequired = p.IsRequired != null ? (p.IsRequired.Value ? 1 : 0) : 0,
                                   SectionId = p.SectionId,
                                   FieldType = p.FieldType,
                                   Order = p.Order,
                                   IsReadOnly = p.IsReadOnly ?? false,
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

        var actionTypes = apiContext.ActionTypes.ToList().DistinctBy(p => p.Id)
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

        var textResources = apiContext.TextResources.ToList().Select(p => new TextResourceEntity()
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

        var users = ((WmsBusiness)business).SGetUserDataForOfflineLoginAndroid()
                                                    .Select()
                                                    .Select(p => new AndroidUser()
                                                    {
                                                        Id = p.ItemArray[0].ToString(),
                                                        Name = p.ItemArray[1].ToString(),
                                                        Username = p.ItemArray[3].ToString(),
                                                        PasswordHash = p.ItemArray[2].ToString()
                                                    })
                                                    .ToList();

        var claims = ((WmsBusiness)business).GetAllAndroidClaims()
                                                   .Select()
                                                   .Select(p => new AndroidPermission()
                                                   {
                                                       UserId = p.ItemArray[0].ToString(),
                                                       Permission = p.ItemArray[1].ToString()
                                                   })
                                                   .ToList();

        if (stationTypes.Any())
        {
            productTypes = productTypes.Where(p => stationTypes.Contains(p.ProductTypeCode))
                                       .ToList();
        }

        var galleries = ((WmsBusiness)business).SGetAllGalleryMediasForAndroid();


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


        List<AndroidItems> androidItems = new();

        if (apiContext.Items.Any())
        {
            var item = apiContext.Items.First();
            androidItems.Add(new()
            {
                Formula = item.Data,
                Id = item.Id
            });
        }

        var tcCauses = apiContext.TruckCrossCauses.Select(p => new AndroidTruckCrossCause
        {
            Id = p.Id,
            Title = p.Title,
            EnterActionTypeId = p.EnterActionTypeId,
            ExitActionTypeId = p.ExitActionTypeId
        }).ToList();

        var tcCompanies = apiContext.TruckCompanies.Select(p => new AndroidTruckCrossCompany
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var tcOpTypes = apiContext.TruckCrossOperationTypes.Select(p => new AndroidTruckCrossOperationType
        {
            Id = p.Id,
            Title = p.Title,
            TruckCrossCauseId = p.TruckCrossCauseId
        }).ToList();

        var tcOpDests = apiContext.TruckCrossOperationDestinations.Select(p => new AndroidTruckCrossOperationDestination
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var tcShipments = apiContext.TruckCrossShipments.Select(p => new AndroidTruckCrossShipment
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var tcCustomers = apiContext.TruckCrossCustomers.Select(p => new AndroidTruckCrossCustomer
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var tcProductTypes = apiContext.TruckCrossProductTypes.Select(p => new AndroidTruckCrossProductType
        {
            Id = p.Id,
            Title = p.Title,
            TruckCrossCauseIdsArray = p.TruckCrossCauseIdsArray
        }).ToList();

        var tcAcceptPlaces = apiContext.TruckCrossAcceptPlaces.Select(p => new AndroidTruckCrossAcceptPlace
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var tcShipmentFees = apiContext.TruckCrossShipmentFees.Select(p => new AndroidTruckCrossShipmentFee
        {
            Id = p.Id,
            CompanyId = p.CompanyId,
            CustomerId = p.CustomerId,
            ProductTypeId = p.ProductTypeId,
            ShipmentId = p.ShipmentId,
            FromDate = p.FromDate,
            ToDate = p.ToDate,
            FeeStatus = p.FeeStatus,
            FeeAmount = p.FeeAmount,
            FeeWeight = p.FeeWeight,
            FeeDistance = p.FeeDistance
        }).ToList();

        var androidTruckTypes = apiContext.TruckTypes.Select(p => new AndroidTruckType
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var androidDynamicFieldSections = apiContext.DynamicFieldSections.Select(p => new AndroidDynamicFieldSection
        {
            Id = p.Id,
            DynamicFieldType = p.DynamicFieldType,
            Title = p.Title
        }).ToList();

        var androidDocumentStatuses = apiContext.DocumentStatuses.Select(p => new AndroidDocumentStatus
        {
            Id = p.Id,
            Title = p.Title,
            IsCartablePermitted = p.IsCartablePermitted ? 1 : 0,
            IsUpdatePermitted = p.IsUpdatePermitted ? 1 : 0
        }).ToList();

        List<AndroidPrint> insertingPrints = new();

        if (bool.TryParse(configuration["ProjectConfigs:WmsConfigs:Android:DatabaseFull:PrintMustFill"], out bool printMustFill))
        {
            if (printMustFill)
            {
                insertingPrints = apiContext.Prints.Select(p => new AndroidPrint
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
                    ProductSerial = p.ProductSerial,
                    ProductCount = p.ProductCount
                }).ToList();
            }
        }

        List<AndroidProductClass> insertingClasses = new();

        apiContext.ProductClasses.ToList().ForEach(p =>
        {
            insertingClasses.Add(new AndroidProductClass
            {
                Id = p.Id,
                Code = p.Code,
                Title = p.Title
            });
        });
        #endregion

        await InsertData();

        string tempDir = Path.Combine(Environment.CurrentDirectory, "Files", "Temp");

        if (!Directory.Exists(tempDir))
        {
            Directory.CreateDirectory(tempDir);
        }

        string destFile = Path.Combine(tempDir, $"{Guid.NewGuid().ToString()[..5]}.db");

        string sourceFile = Path.Combine(Environment.CurrentDirectory, "Wms.db");

        System.IO.File.Copy(sourceFile, destFile, overwrite: true);

        MemoryStream memoryStream = new();

        using (ZipArchive archive = new(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry = archive.CreateEntry(Path.GetFileName(destFile), CompressionLevel.Fastest);

            using var entryStream = entry.Open();
            using var fileStream = System.IO.File.OpenRead(destFile);

            await fileStream.CopyToAsync(entryStream);
        }

        // Clean up the temp file
        try
        {
            System.IO.File.Delete(destFile);
        }
        catch
        {
            // Ignore cleanup errors
        }

        memoryStream.Position = 0;

        return File(memoryStream, "application/zip", "Wms.zip");

        async Task InsertData()
        {
            await context.AddRangeAsync(data);
            await context.AddRangeAsync(dataProducts);
            await context.AddRangeAsync(elements);
            await context.AddRangeAsync(customerAccounts);
            await context.AddRangeAsync(invTags);
            await context.AddRangeAsync(zones);
            await context.AddRangeAsync(fields);
            await context.AddRangeAsync(stations);
            await context.AddRangeAsync(destinations);
            await context.AddRangeAsync(lines);
            await context.AddRangeAsync(shifts);
            await context.AddRangeAsync(actionTypes);
            await context.AddRangeAsync(textResources);
            await context.AddRangeAsync(productTypes);
            await context.AddRangeAsync(users);
            await context.AddRangeAsync(claims);
            await context.AddRangeAsync(galleries);
            await context.AddRangeAsync(productBrands);
            await context.AddRangeAsync(productSize);
            await context.AddRangeAsync(productGroups);
            await context.AddRangeAsync(productSubGroups);
            await context.AddRangeAsync(productStatus);
            await context.AddRangeAsync(androidItems);
            await context.AddRangeAsync(tcCauses);
            await context.AddRangeAsync(tcCompanies);
            await context.AddRangeAsync(tcOpTypes);
            await context.AddRangeAsync(tcOpDests);
            await context.AddRangeAsync(tcShipments);
            await context.AddRangeAsync(tcCustomers);
            await context.AddRangeAsync(tcProductTypes);
            await context.AddRangeAsync(tcAcceptPlaces);
            await context.AddRangeAsync(tcShipmentFees);
            await context.AddRangeAsync(androidTruckTypes);
            await context.AddRangeAsync(fields);
            await context.AddRangeAsync(androidDynamicFieldSections);
            await context.AddRangeAsync(androidDocumentStatuses);
            await context.AddRangeAsync(insertingPrints);
            await context.AddRangeAsync(insertingClasses);
            await context.SaveChangesAsync();
        }
    }



    [HttpPost("[action]")]
    public async Task<IActionResult> GetLatestDatabaseForStart(string? userToken)
    {
        logger.LogInformation("GetLatestDatabase: userToken=> " + userToken);

        var stationCode = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.SerialNumber)?.Value;

        #region Prepare Data

        var allStations = apiContext.Stations.AsNoTracking().ToList();

        List<string> stationTypes = new();

        if (stationCode.HasValue())
        {
            var settings = allStations.FirstOrDefault(p => p.Code == stationCode)?.Settings;

            if (settings.HasValue())
            {
                stationTypes = JToken.Parse(settings)["ProductTypes"].ToObject<List<string>>();
            }
        }

        var stations = allStations
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
                ActionType = p.ActionType ?? 0,
                MacAddress = p.MacAddress ?? string.Empty
            })
            .ToList();

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
                technicalCode = "",
                Id = (int)row.ItemArray[9],
                TagInDestination = row.ItemArray[10].ToString(),
                ProductProperties = row.ItemArray[11].ToString(),
                FreezStatus = (row.ItemArray[12].ToString() == "عدم فریز") ? "0" : "1",
                InspectStatus = (row.ItemArray[13].ToString() == "بازرسی تأیید") ? "0" : ((row.ItemArray[13].ToString() == "بازرسی نشده") ? "1" : "2"),
                LockStatus = row.ItemArray[14].ToString(),
                MiladiRegisterDate = row.ItemArray[16].ToString(),
                ShamsiRegisterDateUnix = row.ItemArray[15].ToString(),
                TagTreeParentsEpc = row.ItemArray[17].ToString()
            });
        }

#if DEBUG
        data = data.Take(100).ToList();
#endif

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
                ProductBrandCode = row.ItemArray[23].ToString(),
                ProductGroupCode = row.ItemArray[20].ToString(),
                ProductSizeCode = row.ItemArray[18].ToString(),
                ProductTypeCode = row.ItemArray[11].ToString(),
                ProductPackValue = row.ItemArray[4].ToString(),
                ProductUnit = row.ItemArray[13].ToString(),
                ProductSubGroup = row.ItemArray[25].ToString(),
                ProductClass = row.ItemArray[26].ToString(),
                ProductValue = row.ItemArray[8].ToString(),
                CountInNextlevelUnit = row.ItemArray[27].ToString(),
                NextlevelUnitTitle = row.ItemArray[28].ToString(),
                HasDoubleTag = (int)row.ItemArray[29] == 1
            });
        }

        if (stationTypes.Any())
        {
            dataProducts = dataProducts
        .Where(p => stationTypes.Contains(p.ProductTypeCode))
        .ToList();
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

        List<CustomerAccountingData> customerAccounts = new();

        //foreach (var p in ((WmsBusiness)business).SGetLastCAD())
        //{
        //    try
        //    {
        //        customerAccounts.Add(new()
        //        {
        //            Id = p.Id,
        //            Count = (decimal)p.ProductCount,
        //            ProductCode = p.ProductCode
        //        });
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}
        List<InventoryTags> invTags = new();
        // invTags = ((WmsBusiness)business)
        //                                      .SGetLastInventoryTags()
        //                                      .Select()
        //                                      .Select(p => new InventoryTags()
        //                                      {
        //                                          Epc = p.ItemArray[1].ToString(),
        //                                          HeaderId = (int)p.ItemArray[2],
        //                                          StoreCode = p.ItemArray[3].ToString()
        //                                      })
        //                                      .DistinctBy(p => p.Epc)
        //                                      .ToList();

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

        var fields = apiContext.DynamicFields
                               .Select(p => new AndroidDynamicFields()
                               {
                                   Id = p.Id,
                                   ActionType = p.ActionType.ToString(),
                                   FieldTitle = p.Title,
                                   DefaultValue = p.DefaultValue,
                                   ValueOptions = p.ValueOptions,
                                   ValueType = p.ValueType,
                                   IsFieldRequired = p.IsRequired != null ? (p.IsRequired.Value ? 1 : 0) : 0,
                                   SectionId = p.SectionId,
                                   FieldType = p.FieldType,
                                   Order = p.Order,
                                   IsReadOnly = p.IsReadOnly ?? false,
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

        var actionTypes = apiContext.ActionTypes.ToList().DistinctBy(p => p.Id)
                        .Select(p => new AndroidActionType()
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Code = p.Code,
                            From = p.From,
                            To = p.To,
                            ActiveControls = p.ActiveControls,
                            DocStatusChange = p.DocStatusChange,
                            DocStatusPermitted = p.DocStatusPermitted
                        })
                        .ToList();

        var textResources = apiContext.TextResources.ToList().Select(p => new TextResourceEntity()
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

        var users = ((WmsBusiness)business).SGetUserDataForOfflineLoginAndroid()
                                                    .Select()
                                                    .Select(p => new AndroidUser()
                                                    {
                                                        Id = p.ItemArray[0].ToString(),
                                                        Name = p.ItemArray[1].ToString(),
                                                        Username = p.ItemArray[3].ToString(),
                                                        PasswordHash = p.ItemArray[2].ToString()
                                                    })
                                                    .ToList();

        var claims = ((WmsBusiness)business).GetAllAndroidClaims()
                                                   .Select()
                                                   .Select(p => new AndroidPermission()
                                                   {
                                                       UserId = p.ItemArray[0].ToString(),
                                                       Permission = p.ItemArray[1].ToString()
                                                   })
                                                   .ToList();

        if (stationTypes.Any())
        {
            productTypes = productTypes.Where(p => stationTypes.Contains(p.ProductTypeCode))
                                       .ToList();
        }

        var galleries = ((WmsBusiness)business).SGetAllGalleryMediasForAndroid();


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


        List<AndroidItems> androidItems = new();

        if (apiContext.Items.Any())
        {
            var item = apiContext.Items.First();
            androidItems.Add(new()
            {
                Formula = item.Data,
                Id = item.Id
            });
        }

        var tcCauses = apiContext.TruckCrossCauses.Select(p => new AndroidTruckCrossCause
        {
            Id = p.Id,
            Title = p.Title,
            EnterActionTypeId = p.EnterActionTypeId,
            ExitActionTypeId = p.ExitActionTypeId
        }).ToList();

        var tcCompanies = apiContext.TruckCompanies.Select(p => new AndroidTruckCrossCompany
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var tcOpTypes = apiContext.TruckCrossOperationTypes.Select(p => new AndroidTruckCrossOperationType
        {
            Id = p.Id,
            Title = p.Title,
            TruckCrossCauseId = p.TruckCrossCauseId
        }).ToList();

        var tcOpDests = apiContext.TruckCrossOperationDestinations.Select(p => new AndroidTruckCrossOperationDestination
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var tcShipments = apiContext.TruckCrossShipments.Select(p => new AndroidTruckCrossShipment
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var tcCustomers = apiContext.TruckCrossCustomers.Select(p => new AndroidTruckCrossCustomer
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var tcProductTypes = apiContext.TruckCrossProductTypes.Select(p => new AndroidTruckCrossProductType
        {
            Id = p.Id,
            Title = p.Title,
            TruckCrossCauseIdsArray = p.TruckCrossCauseIdsArray
        }).ToList();

        var tcAcceptPlaces = apiContext.TruckCrossAcceptPlaces.Select(p => new AndroidTruckCrossAcceptPlace
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var tcShipmentFees = apiContext.TruckCrossShipmentFees.Select(p => new AndroidTruckCrossShipmentFee
        {
            Id = p.Id,
            CompanyId = p.CompanyId,
            CustomerId = p.CustomerId,
            ProductTypeId = p.ProductTypeId,
            ShipmentId = p.ShipmentId,
            FromDate = p.FromDate,
            ToDate = p.ToDate,
            FeeStatus = p.FeeStatus,
            FeeAmount = p.FeeAmount,
            FeeWeight = p.FeeWeight,
            FeeDistance = p.FeeDistance
        }).ToList();

        var androidTruckTypes = apiContext.TruckTypes.Select(p => new AndroidTruckType
        {
            Id = p.Id,
            Title = p.Title
        }).ToList();

        var androidDynamicFieldSections = apiContext.DynamicFieldSections.Select(p => new AndroidDynamicFieldSection
        {
            Id = p.Id,
            DynamicFieldType = p.DynamicFieldType,
            Title = p.Title
        }).ToList();

        var androidDocumentStatuses = apiContext.DocumentStatuses.Select(p => new AndroidDocumentStatus
        {
            Id = p.Id,
            Title = p.Title,
            IsCartablePermitted = p.IsCartablePermitted ? 1 : 0,
            IsUpdatePermitted = p.IsUpdatePermitted ? 1 : 0
        }).ToList();
        #endregion

        await InsertData();

        string tempDir = Path.Combine(Environment.CurrentDirectory, "Files", "Temp");

        if (!Directory.Exists(tempDir))
        {
            Directory.CreateDirectory(tempDir);
        }

        string destFile = Path.Combine(tempDir, $"{Guid.NewGuid().ToString()[..5]}.db");

        string sourceFile = Path.Combine(Environment.CurrentDirectory, "Wms.db");

        System.IO.File.Copy(sourceFile, destFile, overwrite: true);

        MemoryStream memoryStream = new();

        using (ZipArchive archive = new(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry = archive.CreateEntry(Path.GetFileName(destFile), CompressionLevel.Fastest);

            using var entryStream = entry.Open();
            using var fileStream = System.IO.File.OpenRead(destFile);

            await fileStream.CopyToAsync(entryStream);
        }

        // Clean up the temp file
        try
        {
            System.IO.File.Delete(destFile);
        }
        catch
        {
            // Ignore cleanup errors
        }

        memoryStream.Position = 0;

        return File(memoryStream, "application/zip", "Wms.zip");

        async Task InsertData()
        {
            await context.AddRangeAsync(data);
            await context.AddRangeAsync(dataProducts);
            await context.AddRangeAsync(elements);
            await context.AddRangeAsync(customerAccounts);
            await context.AddRangeAsync(invTags);
            await context.AddRangeAsync(zones);
            await context.AddRangeAsync(fields);
            await context.AddRangeAsync(stations);
            await context.AddRangeAsync(destinations);
            await context.AddRangeAsync(lines);
            await context.AddRangeAsync(shifts);
            await context.AddRangeAsync(actionTypes);
            await context.AddRangeAsync(textResources);
            await context.AddRangeAsync(productTypes);
            await context.AddRangeAsync(users);
            await context.AddRangeAsync(claims);
            await context.AddRangeAsync(galleries);
            await context.AddRangeAsync(productBrands);
            await context.AddRangeAsync(productSize);
            await context.AddRangeAsync(productGroups);
            await context.AddRangeAsync(productSubGroups);
            await context.AddRangeAsync(productStatus);
            await context.AddRangeAsync(androidItems);
            await context.AddRangeAsync(tcCauses);
            await context.AddRangeAsync(tcCompanies);
            await context.AddRangeAsync(tcOpTypes);
            await context.AddRangeAsync(tcOpDests);
            await context.AddRangeAsync(tcShipments);
            await context.AddRangeAsync(tcCustomers);
            await context.AddRangeAsync(tcProductTypes);
            await context.AddRangeAsync(tcAcceptPlaces);
            await context.AddRangeAsync(tcShipmentFees);
            await context.AddRangeAsync(androidTruckTypes);
            await context.AddRangeAsync(fields);
            await context.AddRangeAsync(androidDynamicFieldSections);
            await context.AddRangeAsync(androidDocumentStatuses);
            await context.SaveChangesAsync();
        }
    }


    /// <summary>
    /// Send request object for execute methods of Document business 
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
    public async Task<IActionResult> Document(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, documentBusiness));

    /// <summary>
    /// Upload Excel file for dynamic processing
    /// </summary>
    /// <param name="file">Excel file to upload</param>
    /// <param name="type">Processing type for the uploaded file</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="documentType">Type of document being processed</param>
    /// <param name="documentCheckType">Document check type (default: 1)</param>
    /// <returns>API response indicating success or failure</returns>
    [HttpPost("[action]")]
    [Consumes("multipart/form-data")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesDefaultResponseType(typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadExcelDynamic([FromForm] IFormFile file
        , [FromHeader] string type
        , [FromHeader] string fileName
        , [FromHeader] string documentType
        , [FromHeader] int documentCheckType = 1)
    {
        logger.LogInformation(@$"InputDynamicExcelFile:
                                type:{type}
                                userId:{User.GetUserId()}
                                fileName:{fileName}");

        string path = $"{Environment.CurrentDirectory}\\Files\\Storage";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        if (System.IO.File.Exists(path))
        {
            return Ok(new ApiResponse()
            {
                Successful = true,
                Value = false
            });
        }

        using var fileStream = System.IO.File.Create($"{path}\\{file.FileName}");

        await file.CopyToAsync(fileStream);

        fileStream.Close();

        if (type.Equals("dynamicExcel"))
        {
            return Ok(new ApiResponse()
            {
                Successful = true,
                Value = ((WmsBusiness)business).SSaveDynamicExcel(path, file.FileName, fileName, type, documentType, documentCheckType)
            });
        }

        return Ok(new ApiResponse()
        {
            Successful = true,
            Value = null
        });
    }

    [HttpPost("[action]")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
    public async Task<IActionResult> SaveFileAsync([FromForm] SaveFileRequest request
        , [FromHeader] string stationCode)
    {
        if (request?.File is null || request.File.Length == 0)
        {
            return BadRequest(new ApiResponse
            {
                Successful = false,
                Value = false
            });
        }

        string path = $"{Environment.CurrentDirectory}\\Files\\AndroidLogs\\Station_{stationCode}";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filePath = Path.Combine(path, $"{PersianCalendarTools.GregorianToPersianWithManualSeprator(DateTime.Now, "")}-{DateTime.Now.ToString("HH:mm").Replace(":", "")}-{Guid.NewGuid().ToString().Substring(0, 3)}.{request.File.FileName.Split('.')[1]}");

        using var fileStream = new FileStream(filePath, FileMode.Create);

        await request.File.CopyToAsync(fileStream);

        fileStream.Close();

        return Ok(new ApiResponse
        {
            Successful = true,
            Value = true
        });
    }

    /// <summary>
    /// Upload Excel file for processing
    /// </summary>
    /// <param name="file">Excel file to upload</param>
    /// <param name="type">Processing type for the uploaded file</param>
    /// <param name="userToken">User authentication token</param>
    /// <returns>API response indicating success or failure</returns>
    [HttpPost("[action]")]
    [Consumes("multipart/form-data")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> InputExcelFile([FromForm] IFormFile file
        , [FromHeader] string type
        , [FromHeader] string userToken)
    {
        logger.LogInformation("InputExcelFile:" + Environment.NewLine
                           + $"type:{type}" + $"user{userToken}");

        string fileName = file.FileName;

        string path = $"{Environment.CurrentDirectory}\\Files\\Storage";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        if (System.IO.File.Exists(path))
        {
            return Ok(new ApiResponse()
            {
                Successful = true,
                Value = false
            });
        }

        using var fileStream = System.IO.File.Create($"{path}\\{fileName}");

        await file.CopyToAsync(fileStream);

        fileStream.Close();

        DataTable data = DataTableTools.ReadExcelDataOutDataTable($"{path}\\{fileName}");

        if (type.Equals("techdata"))
        {
            string jsonString = JsonConvert.SerializeObject(data);

            logger.LogInformation("InputExcelFile:" + Environment.NewLine
                          + $"type:{type}" + Environment.NewLine
                          + $"data:{jsonString}");

            JArray jsonArray = JArray.Parse(jsonString);

            return Ok(new ApiResponse()
            {
                Successful = true,
                Value = ((WmsBusiness)business).SSaveTechnicalInformation(jsonArray)
            });
        }

        return Ok(new ApiResponse()
        {
            Successful = true,
            Value = null
        });
    }

    /// <summary>
    /// Upload Excel file for place processing
    /// </summary>
    /// <param name="file">Excel file to upload</param>
    /// <param name="sourceWarehouseCode">Source warehouse code</param>
    /// <param name="logGateActionId">Log gate action identifier</param>
    /// <param name="userToken">User authentication token</param>
    /// <returns>API response with serials or validation errors</returns>
    [HttpPost("[action]")]
    [Consumes("multipart/form-data")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> InputExcelFilePlace([FromForm] IFormFile file
   , [FromHeader] string sourceWarehouseCode
        , [FromHeader] string logGateActionId
    , [FromHeader] string userToken)
    {
        logger.LogInformation("InputExcelFilePlace:" + Environment.NewLine
                          + $"sourceWarehouseCode:{sourceWarehouseCode}" + Environment.NewLine
                          + $"logGateActionId:{logGateActionId}" + Environment.NewLine
                          + $"userToken:{userToken}");

        using var fileStream = file.OpenReadStream();

        DataTable data = DataTableTools.ReadExcelDataOutDataTable(fileStream);

        var rows = data.Select().Select(row => row.ItemArray[0].ToString()).ToList();

        List<string> serials = new();

        foreach (var row in rows)
        {
            serials.Add(row.ToString());
        }

        var result = ((WmsBusiness)business).SGetAllNotFoundSerials(serials);

        if (result.Any())
        {
            return Ok(new ApiResponse()
            {
                Successful = false,
                Value = result
            });
        }
        else
        {
            return Ok(new ApiResponse()
            {
                Successful = true,
                Value = serials
            });
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> TruckCross(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, truckCrossBusiness));

    [HttpPost("[action]")]
    public async Task<IActionResult> Product(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, productBusiness));

    [HttpPost("[action]")]
    public async Task<IActionResult> Report(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, reportBusiness));

    [HttpPost("[action]")]
    public async Task<IActionResult> Inspect(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, inspectBusiness));

    [HttpPost("[action]")]
    public async Task<IActionResult> Settings(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, settingsBusiness));

    [HttpPost("[action]")]
    public async Task<IActionResult> ReportFormat(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, reportFormatBusiness));

    [HttpPost("[action]")]
    public async Task<IActionResult> ReportExcel(ApiRequest request)
    {
        var response = ProccessRequestObjectList(request);

        if (response.Value is IList list
         && response.Value.GetType().IsGenericType
         && response.Value.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
        {
            return File(DataTableTools.GetExcelFromDataTable(DataTableTools.GetDataTableUsingDisplayAttribute(list, list.GetType())), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        else if (response.Value is DataTable data)
        {
            return File(DataTableTools.GetExcelFromDataTable(data), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        throw new Exception("Method returns not Ok");
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> DocumentLog(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, documentLogBusiness));

    [HttpPost("[action]")]
    public async Task<IActionResult> Notification(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, notificationBusiness));

    [HttpPost("[action]")]
    public async Task<IActionResult> CustomerGuarantee(ApiRequest request)
    => Ok(ProccessRequestObjectListByBusiness(request, customerGuaranteeBusiness));
}
