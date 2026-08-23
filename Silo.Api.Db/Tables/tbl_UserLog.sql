CREATE TABLE [dbo].[tbl_UserLog] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [Username]          NVARCHAR (50) NOT NULL,
    [Password]          NVARCHAR (50) NOT NULL,
    [DeviceIp]          NVARCHAR (50) NOT NULL,
    [DeviceId]          NVARCHAR (50) NOT NULL,
    [LoginUnixDateTime] BIGINT        NOT NULL,
    CONSTRAINT [PK__tbl_User__3214EC07239E4DCF] PRIMARY KEY CLUSTERED ([Id] ASC)
);

