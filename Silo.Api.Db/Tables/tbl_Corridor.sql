CREATE TABLE [dbo].[tbl_Corridor] (
    [fld_CorridorId]              INT            IDENTITY (1, 1) NOT NULL,
    [fld_CorridorName]            NVARCHAR (128) NOT NULL,
    [fld_CorridorWarehouseCode]   NVARCHAR (50)  NOT NULL,
    [fld_CorridorDirection]       INT            NULL,
    [fld_CorridorVerticalOrder]   INT            NULL,
    [fld_CorridorHorizontalOrder] INT            NULL,
    [fld_CorridorWidth]           INT            NULL
);

