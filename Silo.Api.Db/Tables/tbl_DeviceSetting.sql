CREATE TABLE [dbo].[tbl_DeviceSetting] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [DeviceId]   INT           NOT NULL,
    [DeviceType] NVARCHAR (50) NOT NULL,
    [DeviceName] NVARCHAR (50) NOT NULL,
    [FormName]   NVARCHAR (50) NOT NULL,
    [FormPower]  INT           NOT NULL,
    [Username]   NVARCHAR (50) NOT NULL,
    [CanAccess]  BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

