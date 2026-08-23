CREATE TABLE [dbo].[tbl_AppSetting] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [AppDescription]      NVARCHAR (50) NOT NULL,
    [AppType]             NVARCHAR (50) NOT NULL,
    [AppLastBuildVersion] INT           NOT NULL,
    [DeviceId]            INT           CONSTRAINT [DF_tbl_AppSetting_DeviceId] DEFAULT ((0)) NOT NULL
);

