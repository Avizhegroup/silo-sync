CREATE TABLE [dbo].[tbl_CheckingHistory] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Username]          NVARCHAR (50)  NOT NULL,
    [DeviceId]          NVARCHAR (50)  NOT NULL,
    [DeviceIp]          NVARCHAR (50)  NOT NULL,
    [TagEpc]            NVARCHAR (50)  NULL,
    [ProductProject]    NVARCHAR (50)  NULL,
    [ProductCode]       NVARCHAR (50)  NULL,
    [ProductSerial]     NVARCHAR (50)  NULL,
    [ProductType]       NVARCHAR (256) NULL,
    [ProductStatus]     INT            NULL,
    [ProductProperties] NVARCHAR (MAX) NULL,
    [ProductCount]      NVARCHAR (50)  NULL,
    [CheckingDate]      NVARCHAR (50)  NULL,
    [CheckingTime]      NVARCHAR (50)  NULL,
    CONSTRAINT [PK__tbl_Chec__3214EC077A3223E8] PRIMARY KEY CLUSTERED ([Id] ASC)
);

