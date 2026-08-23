CREATE TABLE [dbo].[tbl_ModulSetting] (
    [Id]                   INT            NULL,
    [DeviceId]             INT            NULL,
    [Name]                 NVARCHAR (MAX) NULL,
    [Type]                 NVARCHAR (MAX) NULL,
    [CommunicationAddress] NVARCHAR (MAX) NULL,
    [Power]                INT            NULL,
    [isActive]             BIT            NOT NULL,
    [DeviceStationCode]    INT            NULL
);

