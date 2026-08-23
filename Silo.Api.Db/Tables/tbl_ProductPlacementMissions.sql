CREATE TABLE [dbo].[tbl_ProductPlacementMissions] (
    [fld_PPMId]              INT           IDENTITY (1, 1) NOT NULL,
    [fld_PPMUserId]          NVARCHAR (50) NULL,
    [fld_PPMRegDateTime]     DATETIME      NULL,
    [fld_POCode]             INT           NULL,
    [fld_PPMProductCode]     NVARCHAR (50) NULL,
    [fld_PPMProductSerial]   NVARCHAR (50) NULL,
    [fld_PPMProductEPC]      NVARCHAR (50) NULL,
    [fld_PPMType]            INT           NOT NULL,
    [fld_PPMWMCode]          INT           NULL,
    [fld_PPMWMDriverUserId]  NVARCHAR (50) NULL,
    [fld_PPMFromStoreCode]   NVARCHAR (50) NULL,
    [fld_PPMFromZoneId]      NVARCHAR (50) NULL,
    [fld_PPMToZoneId]        NVARCHAR (50) NULL,
    [fld_PPMStoreCode]       NVARCHAR (50) NULL,
    [fld_PPMStatus]          INT           NULL,
    [fld_PPMToZoneId_Edited] NVARCHAR (50) NULL,
    [fld_PPMDateTime]        DATETIME      NULL,
    [fld_PPMActionId]        INT           NULL,
    [fld_PPMRegCode]         NVARCHAR (50) NULL
);


GO
CREATE TRIGGER  [dbo].[Mission_ZoneCapacity]
   ON  [dbo].[tbl_ProductPlacementMissions]
   AFTER INSERT,UPDATE
AS 

IF (SELECT COUNT(*) FROM deleted) > 0
BEGIN
UPDATE       tbl_Zones
SET                ZoneOccupiedCapacity =
                             (SELECT        COUNT(DISTINCT ProductSerial) AS Expr1
                               FROM            tbl_Tags
                               WHERE        (TagInDestinationId = tbl_Zones.ZoneStoreCode) AND (TagZone = tbl_Zones.ZoneCode)) +
                             (SELECT        COUNT(DISTINCT fld_PPMProductSerial) AS Expr1
                               FROM            tbl_ProductPlacementMissions
                               WHERE        (fld_PPMToZoneId = tbl_Zones.ZoneCode) AND (fld_PPMStoreCode = tbl_Zones.ZoneStoreCode) AND (fld_PPMStatus IN (0, 1)))
							   where tbl_Zones.ZoneStoreCode=(SELECT i.fld_PPMStoreCode from deleted i) and  tbl_Zones.ZoneCode=(SELECT i.fld_PPMToZoneId from deleted i) 
END
ELSE
BEGIN
UPDATE       tbl_Zones
SET                ZoneOccupiedCapacity =
                             (SELECT        COUNT(DISTINCT ProductSerial) AS Expr1
                               FROM            tbl_Tags
                               WHERE        (TagInDestinationId = tbl_Zones.ZoneStoreCode) AND (TagZone = tbl_Zones.ZoneCode)) +
                             (SELECT        COUNT(DISTINCT fld_PPMProductSerial) AS Expr1
                               FROM            tbl_ProductPlacementMissions
                               WHERE        (fld_PPMToZoneId = tbl_Zones.ZoneCode) AND (fld_PPMStoreCode = tbl_Zones.ZoneStoreCode) AND (fld_PPMStatus IN (0, 1)))
							   where tbl_Zones.ZoneStoreCode=(SELECT i.fld_PPMStoreCode from inserted i) and  tbl_Zones.ZoneCode=(SELECT i.fld_PPMToZoneId from inserted i) 
END

ALTER TABLE [dbo].[tbl_ProductPlacementMissions] ENABLE TRIGGER [Mission_ZoneCapacity]