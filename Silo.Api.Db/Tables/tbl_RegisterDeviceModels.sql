CREATE TABLE [dbo].[tbl_RegisterDeviceModels] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [ModelName]        NVARCHAR (50) NOT NULL,
    [ModelDeviceName]  NVARCHAR (50) NOT NULL,
    [ModelDeviceCount] INT           NOT NULL,
    [ConnectionType]   INT           NULL,
    CONSTRAINT [PK_tbl_RegisterDeviceModels] PRIMARY KEY CLUSTERED ([ModelName] ASC, [ModelDeviceName] ASC)
);

