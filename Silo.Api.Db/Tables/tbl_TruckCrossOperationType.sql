CREATE TABLE [dbo].[tbl_TruckCrossOperationType] (
    [fld_TruckCrossOperationTypeId]    INT            IDENTITY (1, 1) NOT NULL,
    [fld_TruckCrossOperationTypeTitle] NVARCHAR (256) NOT NULL,
    [fld_TruckCrossCause]              INT            NOT NULL
);
