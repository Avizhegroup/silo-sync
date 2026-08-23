CREATE TABLE [dbo].[tbl_TruckCrossProductType] (
    [fld_TruckCrossProductTypeId]    INT            IDENTITY (1, 1) NOT NULL,
    [fld_TruckCrossProductTypeTitle] NVARCHAR (256) NOT NULL,
    [fld_TruckCrossCauseIdsArray] NVARCHAR (MAX) NOT NULL
);
