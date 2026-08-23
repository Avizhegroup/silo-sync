CREATE TABLE [dbo].[tbl_UHF_ReaderLog] (
    [id]                   INT            IDENTITY (1, 1) NOT NULL,
    [fld_TagSerial]        NVARCHAR (24)  NULL,
    [fld_Reader_Gate]      NVARCHAR (256) NULL,
    [fld_ReaderIp]         NVARCHAR (15)  NULL,
    [fld_TagRead_DateTime] NVARCHAR (50)  NULL,
    [fld_TagSelectedFlag]  TINYINT        NULL,
    [fld_InventoryId]      INT            NULL,
    [fld_Reader_GateType]  INT            NULL,
    [fld_Desc]             NVARCHAR (256) NULL,
    [fld_ReaderDeviceType] INT            NULL,
    [ActionStatus]         INT            NULL,
    [ActionDesc]           NVARCHAR (250) NULL,
    [fld_DocumentId]       NVARCHAR (128) NULL,
    [fld_WMUserId]         NVARCHAR (50)  NULL,
    [fld_InventoryPackage] INT            NULL,
    [MovementActionId] INT            NULL, 
    [fld_TagRead_DateTimeMiladi] DATETIME NULL,
    [fld_SaveUserId]       NVARCHAR(128) NULL, 
    [fld_ProductSerial] NVARCHAR(50) NULL
);

