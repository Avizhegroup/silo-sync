CREATE TABLE [dbo].[tbl_InventoryHeader] (
    [fld_InventoryHeaderId]      INT            IDENTITY (1, 1) NOT NULL,
    [fld_InventoryDate]          NVARCHAR (10)  NULL,
    [fld_InventoryTime]          NVARCHAR (5)   NULL,
    [fld_InventoryUserId]        NVARCHAR (128) NULL,
    [fld_InventoryDescription]   NVARCHAR (500) NULL,
    [fld_InventoryStatus]        TINYINT        NULL,
    [fld_InventoryAlExistTgCunt] INT            NULL,
    [fld_InventoryCntCunt]       INT            NULL,
    [fld_InventoryErrCunt]       INT            NULL,
    [fld_InventoryZoneCunt]      INT            NULL
);

