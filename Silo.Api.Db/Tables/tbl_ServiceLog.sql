CREATE TABLE [dbo].[tbl_ServiceLog] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [PrintUser]          NVARCHAR (50)  NULL,
    [ProductSerial]      NVARCHAR (50)  NULL,
    [Seen]               BIT            NULL,
    [ShamsiUniXDateTime] NVARCHAR (50)  NULL,
    [ErrorType]          INT            NULL,
    [ErrorDesc]          NVARCHAR (500) NULL
);

