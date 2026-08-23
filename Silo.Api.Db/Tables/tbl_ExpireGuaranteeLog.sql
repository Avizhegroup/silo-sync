CREATE TABLE [dbo].[tbl_ExpireGuaranteeLog](
	[fld_ExpireGuaranteeId] [int] IDENTITY(1,1) NOT NULL,
	[fld_ExpireGuaranteeDateTime] [datetime] NULL,
	[fld_ExpireGuaranteeDate] [nvarchar](10) NULL,
	[fld_ExpireGuaranteeTime] [nvarchar](5) NULL,
	[fld_ExpireGuaranteeProductCode] [nvarchar](50) NULL,
	[fld_ExpireGuaranteeProductSerial] [nvarchar](50) NULL,
	[fld_ExpireGuaranteeUserId] [nvarchar](128) NULL,
	[fld_ExpireGuaranteeGuaranteeType] [int] NULL,
	[fld_ExpireGuaranteeGuaranteeDays] [int] NULL,
	[fld_ExpireGuaranteeExpireType] [int] NULL,
	[fld_ExpireGuaranteeExpireDays] [int] NULL, 
    [fld_ExpireGuaranteeGuaranteeEndDate] NVARCHAR(10) NULL, 
    [fld_ExpireGuaranteeExpireEndDate] NVARCHAR(10) NULL, 
    [fld_ExpireGuaranteeExpireMonths] INT NULL, 
    [fld_ExpireGuaranteeGuaranteeMonths] INT NULL
)