CREATE TABLE [dbo].[tbl_TruckCrossItem] (
    [fld_TruckCrossItemId]    INT            IDENTITY (1, 1) NOT NULL,
    [fld_TruckCrossItemType]           INT                 NULL,
    [fld_TruckCrossItemTitle]          NVARCHAR (256)       NULL,
    [fld_TruckCrossProductType]        INT                 NULL,
    [fld_TruckCrossItemProductUnit]    NVARCHAR (50)       NULL,
    [fld_TruckCrossItemProductCount]   DECIMAL (18, 2)     NULL,
    [fld_TruckCrossItemProductSerial]  NVARCHAR (50)       NULL,
    [fld_TruckCrossItemProductCode]    NVARCHAR (50)       NULL,
    [fld_TruckCross]                   BIGINT              NULL,
);
