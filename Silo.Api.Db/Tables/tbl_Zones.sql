CREATE TABLE [dbo].[tbl_Zones] (
    [id]                   INT             IDENTITY (1, 1) NOT NULL,
    [ZoneCode]             NVARCHAR (50)   NULL,
    [ZoneTitle]            NVARCHAR (50)   NULL,
    [ZoneCapacity]         DECIMAL (18, 2) NULL,
    [ZoneDimention]        NVARCHAR (50)   NULL,
    [ZoneParentCode]       NVARCHAR (50)   NULL,
    [ZoneParentLayer]      INT             NULL,
    [ZoneStoreCode]        NVARCHAR (50)   NULL,
    [ZoneCountPixle]       INT             NULL,
    [ZoneOccupiedCapacity] INT             CONSTRAINT [DF_tbl_Zones_ZoneOccupiedCapacity] DEFAULT ((0)) NULL,
    [MinZoneCapacity]      DECIMAL (18, 2) NULL,
    [MaxZoneCapacity]      DECIMAL (18, 2) NULL,
    [ZoneRowIndex]         INT             CONSTRAINT [DF_tbl_Zones_ZoneRowIndex] DEFAULT ((0)) NULL,
    [ZoneType]             INT             NULL,
    [ZoneAddress]          NVARCHAR (250)  NULL,
    [ZoneCorridorId]       INT             CONSTRAINT [DF_tbl_Zones_ZoneCorridorId] DEFAULT ((-1)) NULL, 
    [ZoneCoordinates] NVARCHAR(512) NULL
);

