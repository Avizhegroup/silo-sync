CREATE TABLE [dbo].[tbl_ProductExpire](
	[fld_ProductExpireId] [int] IDENTITY(1,1) NOT NULL,
	[fld_ProductExpireProductSerial] NVARCHAR(50) NULL,
	[fld_ProductExpireProductCode] NVARCHAR(50) NULL,
	[fld_ProductExpireStatus] int NULL,
	[fld_ProductExpireStartDate] NVARCHAR(10) NULL,
	[fld_ProductExpireEndDate] NVARCHAR(10) NULL,
    [fld_ProductExpireActivationType] int NULL,
    [fld_ProductExpireLastModifiedDateTime] datetime NULL,
	[fld_ProductExpireLastModifiedUserId] NVARCHAR(128) NULL
)