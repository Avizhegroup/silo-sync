CREATE TABLE [dbo].[tbl_TruckCrossCause] (
    [fld_TruckCrossCauseId]          INT            IDENTITY (1, 1) NOT NULL,
    [fld_TruckCrossCauseTitle] NVARCHAR (256) NOT NULL, 
    [fld_TruckCrossCauseEnterActionTypeId] INT NULL, 
    [fld_TruckCrossCauseExitActionTypeId] INT NULL
);

