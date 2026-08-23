CREATE TABLE [dbo].[tbl_WarehouseCorridor] (
    [fld_WarehouseCorridorId]         INT            IDENTITY(1,1)  NOT NULL,
    [fld_WarehouseCorridorContextKey] NVARCHAR(50)   NOT NULL        DEFAULT (''),
    [fld_WarehouseCorridorX1]         REAL           NOT NULL,
    [fld_WarehouseCorridorZ1]         REAL           NOT NULL,
    [fld_WarehouseCorridorX2]         REAL           NOT NULL,
    [fld_WarehouseCorridorZ2]         REAL           NOT NULL,
    [fld_WarehouseCorridorWidth]      REAL           NOT NULL        DEFAULT (1.0),
    [fld_WarehouseCorridorLabel]      NVARCHAR(200)  NULL,
    CONSTRAINT [PK_tbl_WarehouseCorridor] PRIMARY KEY CLUSTERED ([fld_WarehouseCorridorId] ASC)
);