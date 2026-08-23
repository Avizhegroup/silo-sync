CREATE TABLE [dbo].[tbl_Destination] (
    [DestinationId]        INT            IDENTITY (1, 1) NOT NULL,
    [DestinationTitle]     NVARCHAR (50)  NULL,
    [DestinationSt]        INT            NULL,
    [DestinationDesc]      NVARCHAR (MAX) NULL,
    [DestinationCode]      NVARCHAR (50)  NULL,
    [DestinationType]      INT            NULL,
    [DestinationParentId]  INT            NULL,
    [DestinationParentsId] NVARCHAR (MAX) NULL,
    [DestinationEpc]       NVARCHAR (50)  NULL, 
    [DestinationCoordinates] NVARCHAR(512) NULL
);

