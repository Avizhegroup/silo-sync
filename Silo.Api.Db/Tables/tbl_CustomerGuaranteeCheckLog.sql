CREATE TABLE [dbo].[tbl_CustomerGuaranteeCheckLog](
	[fld_CGCLogId] [int] IDENTITY(1,1) NOT NULL,
	[fld_CGCLogDeviceIp] nvarchar(50) NULL,
	[fld_CGCLogRegCode] nvarchar(50) NULL,
	[fld_CGCLogProductSerial] nvarchar(50) NULL,
	[fld_CGCLogCustomerFullName] nvarchar(512) NULL,
	[fld_CGCLogPhoneNumber] nvarchar(14) NULL,
	[fld_CGCLogNationalCode] nvarchar(10) NULL,
	[fld_CGCLogProvinceCode] nvarchar(10) NULL,
	[fld_CGCLogCityCode] nvarchar(10) NULL,
	[fld_CGCLogDateTime] datetime NULL
)