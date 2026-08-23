CREATE TABLE [dbo].[tbl_TruckCrossShippingFee]
(
[fld_TruckCrossShippingFeeId] [int] IDENTITY(1,1) NOT NULL,
[fld_TruckCrossShippingFeeCompanyId] [int] NULL, 
[fld_TruckCrossShippingFeeCustomerId] [int] NULL, 
[fld_TruckCrossShippingFeeProductTypeId] [int] NULL, 
[fld_TruckCrossShippingFeeShipmentId] [int] NULL,
[fld_TruckCrossShippingFeeFromDate] [nvarchar](50) NULL, 
[fld_TruckCrossShippingFeeToDate] [nvarchar](50) NULL, 
[fld_TruckCrossShippingFeeStatus] [bit] NULL, 
[fld_TruckCrossShippingFeeAmount] DECIMAL (18, 2) NULL, 
[fld_TruckCrossShippingFeeWeight] DECIMAL (18, 2) NULL, 
[fld_TruckCrossShippingFeeDistance] DECIMAL (18, 2) NULL
) ON [PRIMARY]
