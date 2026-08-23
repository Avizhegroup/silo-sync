CREATE TABLE [dbo].[tbl_InventoryTags] (
    [fld_InventoryId]               INT             IDENTITY (1, 1) NOT NULL,
    [fld_InventoryTagStatus]        INT             NULL,
    [fld_InventoryTagEPC]           NVARCHAR (50)   NULL,
    [fld_InventoryDate]             NVARCHAR (50)   NULL,
    [fld_InventoryDateTime]         DATETIME        NULL,
    [fld_InventoryStoreCode]        NVARCHAR (50)   NULL,
    [fld_InventoryUser]             NVARCHAR (250)  NULL,
    [fld_InventoryDeviceIp]         NVARCHAR (50)   NULL,
    [fld_InventoryDeviceId]         NVARCHAR (50)   NULL,
    [fld_InventoryProductSerial]    NVARCHAR (50)   NULL,
    [fld_InventoryProductCode]      NVARCHAR (50)   NULL,
    [fld_InventoryProductType]      NVARCHAR (250)  NULL,
    [fld_InventoryProductParentId]  INT             NULL,
    [fld_InventoryProjectCode]      NVARCHAR (50)   NULL,
    [fld_InventoryProductCount]     DECIMAL (18, 2) NULL,
    [fld_InventoryProductName]      NVARCHAR (MAX)  NULL,
    [fld_InventoryProductNewSerial] NVARCHAR (50)   NULL,
    [fld_InventoryHeaderId]         INT             NULL,
    [fld_InventoryProductDesc]      NVARCHAR (500)  NULL,
    [fld_InventoryIdentifyType]     INT             NULL,
    [fld_InventoryPlace]            NVARCHAR (50)   NULL
);

