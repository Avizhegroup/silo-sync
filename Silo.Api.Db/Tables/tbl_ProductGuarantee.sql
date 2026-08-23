CREATE TABLE [dbo].[tbl_ProductGuarantee](
	[fld_ProductGuaranteeId] [int] IDENTITY(1,1) NOT NULL,
	[fld_ProductGuaranteeProductSerial] NVARCHAR(50) NULL,
	[fld_ProductGuaranteeProductCode] NVARCHAR(50) NULL,
	[fld_ProductGuaranteeStatus] int NULL,
	[fld_ProductGuaranteeStartDate] NVARCHAR(10) NULL,
	[fld_ProductGuaranteeEndDate] NVARCHAR(10) NULL,
    [fld_ProductGuaranteeActivationType] int NULL,
    [fld_ProductGuaranteeLastModifiedDateTime] datetime NULL,
	[fld_ProductGuaranteeLastModifiedUserId] NVARCHAR(128) NULL
)