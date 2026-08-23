CREATE TABLE [dbo].[tbl_UserActionLog] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [Username]           NVARCHAR (50)  NULL,
    [Form]               NVARCHAR (50)  NULL,
    [Action]             NVARCHAR (MAX) NULL,
    [DeviceIp]           NVARCHAR (50)  NULL,
    [DeviceId]           NVARCHAR (50)  NULL,
    [ShamsiUnixDateTime] NVARCHAR (50)  NULL,
    [ShamsiDate]         NVARCHAR (50)  NULL,
    [Time]               NVARCHAR (50)  NULL,
    CONSTRAINT [PK_tbl_UserActionLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

