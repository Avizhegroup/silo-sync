CREATE TABLE [dbo].[tbl_MovementActions] (
    [MovementActionId]            INT            NULL,
    [MovementActionTp]            INT            NULL,
    [MovementActionStore]         NVARCHAR (50)  NULL,
    [MovementActionUserId]        NVARCHAR (MAX) NULL,
    [MovementActionDate]          NVARCHAR (10)  NULL,
    [MovementActionTime]          NVARCHAR (5)   NULL,
    [MovementActionDateTime]      DATETIME       NULL,
    [MovementActionCountTags]     INT            NULL,
    [MovementActionDestinationId] NVARCHAR (50)  NULL,
    [MovementActionCarPlaque]     NVARCHAR (16)  NULL,
    [MovementActionDriverName]    NVARCHAR (50)  NULL,
    [MovementActionDriverMobile]  NVARCHAR (50)  NULL,
    [MovementActionData]          NVARCHAR (MAX) NULL,
    [MovementActionLinkId]        INT            NULL,
    [MovementActionLinkDestId]    NVARCHAR (50)  NULL,
    [MovementActionDocumentId]    NVARCHAR(MAX),
    [MovementActionDesc]          NVARCHAR(MAX),
    [MovementActionUHFLogId]      NVARCHAR(256)  NULL,
    [MovementActionUHFLogGate]    NVARCHAR(30)   NULL,
    [MovementActionTruckCrossId]  bigint         NULL

);

