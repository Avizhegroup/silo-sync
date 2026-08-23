CREATE TABLE [dbo].[tbl_NotificationOrders] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [fld_NOId]           INT            NULL,
    [fld_NOStatus]       INT            NULL,
    [fld_NODateTime]     DATETIME       NULL,
    [fld_NOUserId]       NVARCHAR (50)  NULL,
    [fld_NOType]         INT            NULL,
    [fld_NOTitle]        NVARCHAR (50)  NULL,
    [fld_NOEventType]    INT            NULL,
    [fld_NOTimePeriod]   INT            NULL,
    [fld_NOSendDay]      NVARCHAR (50)  NULL,
    [fld_NOSendClock]    NVARCHAR (5)   NULL,
    [fld_NOSendType]     NVARCHAR (50)  NULL,
    [fld_NOSendContacts] NVARCHAR (MAX) NULL,
    [fld_NOContent]      NVARCHAR (MAX) NULL
);

