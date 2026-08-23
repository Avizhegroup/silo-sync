using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Silo.Application.Contracts;
using Silo.Domains.Entities;
using Silo.Domains.Services;

namespace Silo.Api.Business;
public class CustomerGuaranteeCheckBusiness(ILogger<CustomerGuaranteeCheckBusiness> logger
        , IDataAccess dataAccess
        , IHttpContextAccessor httpContextAccessor
        , IConfiguration configuration
        , WmsApiContext apiContext
        , IMapper mapper) : ProjectBusiness(dataAccess, logger, httpContextAccessor)
{
    public CheckCustomerGuaranteeVm SCheckProductGuaranteeForCustomer(CheckCustomerGuaranteeForCustomerQuery command)
    {
        if (SIsAnotherCustomerCheckedBefore(command))
        {
            return new()
            {
                GuaranteeCheckResultStatus = CustomerCheckGuaranteePageMode.CheckedBefore
            };
        }

        string productGuaranteeQuery = "SELECT TOP(1) ProductSerial FROM tbl_Tags WHERE tbl_Tags.DeviceIp = @deviceIp AND tbl_Tags.RegCode = @regCode";

        KeyValuePair<string, object>[] parameters = new[]
        {
            new KeyValuePair<string, object>("deviceIp", command.DeviceIp),
            new KeyValuePair<string, object>("regCode", command.RegCode)
        };

        var sqlData = dataAccess.SqlDataAdapter(productGuaranteeQuery, parameters).Select();

        if (sqlData.Any())
        {
            command.ProductSerial = (string)sqlData.First().ItemArray.First();

            var productGuarantee = SGetCustomerGuaranteeBySerial(command.ProductSerial);

            if (SIsSameCustomerCheckedBefore(command))
            {
                productGuarantee.GuaranteeCheckResultStatus = CustomerCheckGuaranteePageMode.Exist;

                return productGuarantee;
            }
            else
            {
                if (productGuarantee.GuaranteeStatus != 1 && productGuarantee.GuaranteeActivationType == GuaranteeTypes.Customer)
                {
                    if (ActivateProductGuarantee(productGuarantee.ProductSerial))
                    {                        productGuarantee = SGetCustomerGuaranteeBySerial(command.ProductSerial);

                        productGuarantee.GuaranteeCheckResultStatus = CustomerCheckGuaranteePageMode.ActivedNow;
                    }
                }
                else
                {
                    LogCustomerGuaranteeCheck();

                    productGuarantee.GuaranteeCheckResultStatus = CustomerCheckGuaranteePageMode.Exist;
                }

                return productGuarantee;
            }
        }

        return new()
        {
            GuaranteeCheckResultStatus = CustomerCheckGuaranteePageMode.NotExist
        };

        bool ActivateProductGuarantee(string serial)
        {
            var commands = new List<KeyValuePair<string, KeyValuePair<string, object>[]>>();

            string userId = httpContext.User.GetUserId();

            string logCustomerGuaranteeCheck = @"
                    INSERT INTO tbl_CustomerGuaranteeCheckLog (
                        fld_CGCLogDeviceIp,
                        fld_CGCLogRegCode,
                        fld_CGCLogProductSerial,
                        fld_CGCLogCustomerFullName,
                        fld_CGCLogPhoneNumber,
                        fld_CGCLogNationalCode,
                        fld_CGCLogProvinceCode,
                        fld_CGCLogCityCode,
                        fld_CGCLogDateTime
                    )
                    VALUES (
                        @DeviceIp,
                        @RegCode,
                        @Serial,
                        @CustomerFullName,
                        @PhoneNumber,
                        @NationalCode,
                        @Province,
                        @City,
                        GETDATE()
                    )";

            KeyValuePair<string, object>[] customerCheckLogParameters = new[]
            {
                        new KeyValuePair<string, object>("@DeviceIp", command.DeviceIp),
                        new KeyValuePair<string, object>("@RegCode", command.RegCode),
                        new KeyValuePair<string, object>("@Serial", command.ProductSerial),
                        new KeyValuePair<string, object>("@CustomerFullName", command.CustomerFullName),
                        new KeyValuePair<string, object>("@PhoneNumber", command.PhoneNumber),
                        new KeyValuePair<string, object>("@NationalCode", command.NationalCode),
                        new KeyValuePair<string, object>("@Province", command.Province),
                        new KeyValuePair<string, object>("@City", command.City)
                    };

            commands.Add(new(logCustomerGuaranteeCheck, customerCheckLogParameters));

            string activeProductGuarantee =
                """
                        UPDATE tbl_ProductGuarantee
                        SET fld_ProductGuaranteeStatus = @status ,fld_ProductGuaranteeStartDate = @startDate,
                        fld_ProductGuaranteeLastModifiedDateTime = GETDATE() ,fld_ProductGuaranteeLastModifiedUserId = @user
                        WHERE  tbl_ProductGuarantee.fld_ProductGuaranteeProductSerial = @serial 
                        """;

            KeyValuePair<string, object>[] guaranteeActivationParameters = new[]
            {
                        new KeyValuePair<string, object>("@status", "1"),
                        new KeyValuePair<string, object>("@serial", serial),
                        new KeyValuePair<string, object>("@startDate", DateTimeTools.GetPersianDate()),
                        new KeyValuePair<string, object>("@user", userId)
                    };

            commands.Add(new(activeProductGuarantee, guaranteeActivationParameters));

            return dataAccess.CmdSqlExecuteNonQueryWithTransaction(commands) > 0;
        }

        void LogCustomerGuaranteeCheck()
        {
            string logCustomerGuaranteeCheck = @"
                    INSERT INTO tbl_CustomerGuaranteeCheckLog (
                        fld_CGCLogDeviceIp,
                        fld_CGCLogRegCode,
                        fld_CGCLogProductSerial,
                        fld_CGCLogCustomerFullName,
                        fld_CGCLogPhoneNumber,
                        fld_CGCLogNationalCode,
                        fld_CGCLogProvinceCode,
                        fld_CGCLogCityCode,
                        fld_CGCLogDateTime
                    )
                    VALUES (
                        @DeviceIp,
                        @RegCode,
                        @Serial,
                        @CustomerFullName,
                        @PhoneNumber,
                        @NationalCode,
                        @Province,
                        @City,
                        GETDATE()
                    )";

            KeyValuePair<string, object>[] customerCheckLogParameters = new[]
            {
                new KeyValuePair<string, object>("@DeviceIp", command.DeviceIp),
                new KeyValuePair<string, object>("@RegCode", command.RegCode),
                new KeyValuePair<string, object>("@Serial", command.ProductSerial),
                new KeyValuePair<string, object>("@CustomerFullName", command.CustomerFullName),
                new KeyValuePair<string, object>("@PhoneNumber", command.PhoneNumber),
                new KeyValuePair<string, object>("@NationalCode", command.NationalCode),
                new KeyValuePair<string, object>("@Province", command.Province),
                new KeyValuePair<string, object>("@City", command.City)
            };

            dataAccess.CmdSqlExecuteNonQuery(logCustomerGuaranteeCheck, customerCheckLogParameters);
        }
    }

    public CheckCustomerGuaranteeVm SGetCustomerGuaranteeBySerial(string serial)
    {
        var command =
            $"""
            SELECT 
            COALESCE(Guarantee.fld_ProductGuaranteeStatus,0) AS GuaranteeStatus,
            CASE WHEN Guarantee.fld_ProductGuaranteeStatus = 0 THEN N'{TextResources.APP_StringKeys_NotChoosed}' 
                        	WHEN Guarantee.fld_ProductGuaranteeStatus = 1 THEN N'{TextResources.APP_StringKeys_Active}' 
                        	WHEN Guarantee.fld_ProductGuaranteeStatus = 2 THEN N'{TextResources.APP_StringKeys_Finished}' 
                        	ELSE N'' END AS GuaranteeStatusTitle,
            Guarantee.fld_ProductGuaranteeActivationType AS [GuaranteeActivationType],
            CASE Guarantee.fld_ProductGuaranteeActivationType 
            WHEN {(int)GuaranteeTypes.EnterToWarehouse} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Enter}' 
            WHEN {(int)GuaranteeTypes.ExitFromWarehouse} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Exit}' 
            WHEN {(int)GuaranteeTypes.AcceptInspect} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Inspect}' 
            WHEN {(int)GuaranteeTypes.ExitFromFactory} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Factory}'
            WHEN {(int)GuaranteeTypes.Sell} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Sell}'
            WHEN {(int)GuaranteeTypes.Install} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Install}'
            WHEN {(int)GuaranteeTypes.Customer} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Customer}'
            WHEN {(int)GuaranteeTypes.Date} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Date}'
            ELSE N'{TextResources.APP_StringKeys_NotChoosed}' END AS [GuaranteeActivationTypeString],
            COALESCE((SELECT ProductName FROM tbl_Tags WHERE ProductSerial = Guarantee.fld_ProductGuaranteeProductSerial),N'') AS ProductTitle,
            COALESCE(Guarantee.fld_ProductGuaranteeStartDate,N'') AS GuaranteeStartDate,
            COALESCE(Guarantee.fld_ProductGuaranteeEndDate,N'') AS GuaranteeEndDate,
            COALESCE(Guarantee.fld_ProductGuaranteeProductSerial,N'') AS ProductSerial

            FROM tbl_ProductGuarantee AS Guarantee
            WHERE fld_ProductGuaranteeProductSerial = @serial
        
            """;

        var sqlData = dataAccess.SqlDataAdapter(command, new KeyValuePair<string, object>("serial", serial)).Select();

        if (sqlData.Any())
        {
            CheckCustomerGuaranteeVm result = new()
            {
                GuaranteeStatus = (int)sqlData.First().ItemArray[0],
                GuaranteeStatusTitle = sqlData.First().ItemArray[1].ToString(),
                GuaranteeActivationType = (GuaranteeTypes)(int)sqlData.First().ItemArray[2],
                GuaranteeActivationTypeString = sqlData.First().ItemArray[3].ToString(),
                ProductTitle = sqlData.First().ItemArray[4].ToString(),
                GuaranteeStartDate = sqlData.First().ItemArray[5].ToString(),
                GuaranteeEndDate = sqlData.First().ItemArray[6].ToString(),
                ProductSerial = sqlData.First().ItemArray[7].ToString(),
                GuaranteeCheckResultStatus = CustomerCheckGuaranteePageMode.Exist
            };

            return result;
        }

        return new()
        {
            GuaranteeCheckResultStatus = CustomerCheckGuaranteePageMode.NotExist
        };
    }

    public CheckCustomerGuaranteeVm SCheckProductGuarantee(CheckCustomerGuaranteeQuery command)
    {
        string productGuaranteeQuery = "SELECT TOP(1) ProductSerial FROM tbl_Tags WHERE tbl_Tags.DeviceIp = @deviceIp AND tbl_Tags.RegCode = @regCode";

        KeyValuePair<string, object>[] parameters = new[]
        {
            new KeyValuePair<string, object>("deviceIp", command.DeviceIp),
            new KeyValuePair<string, object>("regCode", command.RegCode)
        };

        var sqlData = dataAccess.SqlDataAdapter(productGuaranteeQuery, parameters).Select();

        if (sqlData.Any())
        {
            command.ProductSerial = (string)sqlData.First().ItemArray.First();

            var productGuarantee = SGetCustomerGuaranteeBySerial(command.ProductSerial);

            if (productGuarantee.GuaranteeStatus != 1 && productGuarantee.GuaranteeActivationType == command.ActivationType)
            {
                if (ActivateProductGuarantee(productGuarantee.ProductSerial))
                {
                    productGuarantee = SGetCustomerGuaranteeBySerial(command.ProductSerial);

                    productGuarantee.GuaranteeCheckResultStatus = CustomerCheckGuaranteePageMode.ActivedNow;
                }
            }
            else
            {
                productGuarantee.GuaranteeCheckResultStatus = CustomerCheckGuaranteePageMode.Exist;
            }

            return productGuarantee;
        }

        return new()
        {
            GuaranteeCheckResultStatus = CustomerCheckGuaranteePageMode.NotExist
        };

        bool ActivateProductGuarantee(string serial)
        {
            string userId = httpContext.User.GetUserId();

            string activeProductGuarantee =
                """
                        UPDATE tbl_ProductGuarantee
                        SET fld_ProductGuaranteeStatus = @status ,fld_ProductGuaranteeStartDate = @startDate,
                        fld_ProductGuaranteeLastModifiedDateTime = GETDATE() ,fld_ProductGuaranteeLastModifiedUserId = @user
                        WHERE  tbl_ProductGuarantee.fld_ProductGuaranteeProductSerial = @serial 
                        """;

            KeyValuePair<string, object>[] guaranteeActivationParameters = new[]
            {
                        new KeyValuePair<string, object>("@status", "1"),
                        new KeyValuePair<string, object>("@serial", serial),
                        new KeyValuePair<string, object>("@startDate", DateTimeTools.GetPersianDate()),
                        new KeyValuePair<string, object>("@user", userId)
                    };


            return dataAccess.CmdSqlExecuteNonQuery(activeProductGuarantee, guaranteeActivationParameters) > 0;
        }
    }

    public List<GetAllProvinceVm> GetAllProvinces()
    {
        return mapper.Map<List<GetAllProvinceVm>>(apiContext.Provinces.ToList());
    }

    public List<GetCitiesVm> GetAllCities()
    {
        return mapper.Map<List<GetCitiesVm>>(apiContext.Cities.ToList());
    }

    public List<GetProductModelsVm> SGetProductModels()
    {
        var products = apiContext.Products.Select(p => new GetProductModelsVm()
        {
            TechnicalCode = p.TechnicalCode,
            ProductGroup = p.ProductGroup,
            ProductSubGroup = p.ProductSubGroup
        }).Where(p => p.TechnicalCode != null).ToList();

        return products;
    }

    public List<ProductGroup> SGetAllProductGroups()
    {
        var result = apiContext.ProductGroups
                               .OrderBy(p => p.Id)
                               .ToList();
        return result;
    }

    public List<GetAllProductSubGroupVm> SGetAllProductSubGroups()
    {
        var result = apiContext.ProductSubGroups
                               .Include(p => p.ProductGroup)
                               .OrderBy(p => p.Id)
                               .ToList();

        return mapper.Map<List<GetAllProductSubGroupVm>>(result);
    }

    public GetSalesInstallerByCodeVm SGetSalesInstallerByCode(string installerCode)
    {
        return mapper.Map<GetSalesInstallerByCodeVm>(apiContext.SalesInstallers.FirstOrDefault(p => p.Code == installerCode));
    }

    public GetSalesShopByShopCodeVm SGetSalesShopByShopCode(string shopCode)
    {
        return mapper.Map<GetSalesShopByShopCodeVm>(apiContext.SalesShops.FirstOrDefault(p => p.Code == shopCode));
    }

    private bool SIsAnotherCustomerCheckedBefore(CheckCustomerGuaranteeForCustomerQuery command)
    {
        string query = @"SELECT COUNT(fld_CGCLogId) FROM tbl_CustomerGuaranteeCheckLog WHERE fld_CGCLogDeviceIp = @deviceIp 
                         AND fld_CGCLogRegCode = @regCode AND fld_CGCLogNationalCode <> @nationalCode";

        KeyValuePair<string, object>[] parameters = new[]
        {
            new KeyValuePair<string, object>("deviceIp", command.DeviceIp),
            new KeyValuePair<string, object>("regCode",command.RegCode),
            new KeyValuePair<string, object>("nationalCode",command.NationalCode)
        };

        var count = (int)dataAccess.SqlDataAdapter(query, parameters).Select().First().ItemArray.First();

        return count > 0;
    }

    private bool SIsSameCustomerCheckedBefore(CheckCustomerGuaranteeForCustomerQuery command)
    {
        string query = @"SELECT COUNT(fld_CGCLogId) FROM tbl_CustomerGuaranteeCheckLog WHERE fld_CGCLogDeviceIp = @deviceIp 
                         AND fld_CGCLogRegCode = @regCode AND fld_CGCLogNationalCode = @nationalCode";

        KeyValuePair<string, object>[] parameters = new[]
        {
            new KeyValuePair<string, object>("deviceIp", command.DeviceIp),
            new KeyValuePair<string, object>("regCode",command.RegCode),
            new KeyValuePair<string, object>("nationalCode",command.NationalCode)
        };

        var count = (int)dataAccess.SqlDataAdapter(query, parameters).Select().First().ItemArray.First();

        return count > 0;
    }
}
