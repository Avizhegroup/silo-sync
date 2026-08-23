CREATE TABLE [dbo].[tbl_NotificationQueue] (
    [fld_Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [fld_Text]                NVARCHAR (MAX) NOT NULL,
    [fld_SendType]            INT            NOT NULL,
    [fld_Contact]             NVARCHAR (256) NOT NULL,
    [fld_SendDateTime]        DATETIME       NULL,
    [fld_SendDate]            NVARCHAR (10)  NULL,
    [fld_SendTime]            NVARCHAR (5)   NULL,
    [fld_SendStatus]          INT            NOT NULL,
    [fld_NotificationOrderId] INT            NOT NULL,
    [fld_QueueActionCode]     NVARCHAR (256) NULL,
    [fld_SaveDateTime]        DATETIME       NOT NULL
);

