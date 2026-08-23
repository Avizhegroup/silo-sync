CREATE TABLE [dbo].[tbl_Tags] (
    [Id]                        INT             IDENTITY (1, 1) NOT NULL,
    [ProductSerial]             NVARCHAR (50)   NOT NULL,
    [ProductCode]               NVARCHAR (50)   NULL,
    [TagEpc]                    NVARCHAR (50)   NOT NULL,
    [ProjectCode]               NVARCHAR (50)   NULL,
    [ProductCount]              DECIMAL (18, 2) NULL,
    [ProductName]               NVARCHAR (MAX)  NULL,
    [ProductType]               NVARCHAR (256)  NULL,
    [ProductStatus]             NVARCHAR (256)  NULL,
    [TagStatus]                 NVARCHAR (50)   CONSTRAINT [DF_tbl_Tags_TagStatus] DEFAULT ((0)) NULL,
    [TagRegisterShamsiUnixDate] NVARCHAR (50)   NULL,
    [TagRegisterUser]           NVARCHAR (50)   NULL,
    [TagTreeParentId]           INT             NULL,
    [TagTreeSecondParentId]     INT             NULL,
    [TagTreeParentsId]          NVARCHAR (MAX)  NULL,
    [NewProductSerial]          NVARCHAR (50)   NULL,
    [ProductProperties]         NVARCHAR (MAX)  NULL,
    [Lock]                      BIT             CONSTRAINT [DF_tbl_Tags_Lock] DEFAULT ((0)) NULL,
    [Username]                  NVARCHAR (50)   NULL,
    [DeviceId]                  NVARCHAR (50)   NULL,
    [DeviceIp]                  NVARCHAR (50)   NULL,
    [Freeze]                    BIT             NULL,
    [Deactivate]                BIT             NULL,
    [TagInActionId]             INT             NULL,
    [TagInDestinationId]        NVARCHAR (50)   NULL,
    [TagInActionId2]            INT             NULL,
    [TagInDestinationId2]       NVARCHAR (50)   NULL,
    [fld_ProductPropertyAId]    NVARCHAR (50)   NULL,
    [fld_ProductPropertyBId]    NVARCHAR (50)   NULL,
    [fld_ProductPropertyCId]    NVARCHAR (50)   NULL,
    [RegCode]                   NVARCHAR (50)   NULL,
    [fld_LastModifierUser]      NVARCHAR (128)  NULL,
    [ContractStatus]            NVARCHAR (50)   NULL,
    [TagZone]                   NVARCHAR (50)   CONSTRAINT [DF_tbl_Tags_TagZone] DEFAULT ((0)) NULL,
    [TagRegisterDateTime]       DATETIME        CONSTRAINT [DF_tbl_Tags_TagRegisterDateTime] DEFAULT (getdate()) NULL,
    [ReProduct]                 BIT             CONSTRAINT [DF_tbl_Tags_ReProduct] DEFAULT ((0)) NULL,
    [fld_InspectActionId]       INT             CONSTRAINT [DF_tbl_Tags_fld_InspectActionId] DEFAULT ((0)) NULL,
    [fld_LastInspectResult]     NVARCHAR (MAX)  NULL,
    [fld_ProductGroup]          NVARCHAR (128)  NULL,
    [fld_ProductBrand]          NVARCHAR (128)  NULL,
    [fld_ProductSubGroup]       NVARCHAR (128)  NULL,
    [fld_ProductClass]          NVARCHAR (128)  NULL,
    [TagTreeParentsEpc]         NVARCHAR (128)  NULL,
    [TagEpc2] NVARCHAR(128) NULL, 
    [TagTreeParentSerial] NVARCHAR(50) NULL, 
    [ProductWeight] DECIMAL(18, 2) NULL, 
    [ProductVolume] DECIMAL(18, 2) NULL, 
    CONSTRAINT [PK_tbl_Tags_1] PRIMARY KEY CLUSTERED ([ProductSerial] ASC, [TagEpc] ASC)
);


GO
CREATE TRIGGER   [dbo].[TAGS_UpdateZoneCapacity]
   ON   dbo.tbl_Tags
   AFTER  INSERT,UPDATE
AS 

IF (SELECT COUNT(*) FROM deleted) > 0
BEGIN
UPDATE tbl_Zones
SET ZoneOccupiedCapacity =   (SELECT        COUNT(DISTINCT ProductSerial) AS Expr1
                               FROM            tbl_Tags
                               WHERE        (TagInDestinationId = tbl_Zones.ZoneStoreCode) AND (TagZone = tbl_Zones.ZoneCode)) +
                             (SELECT        COUNT(DISTINCT fld_PPMProductSerial) AS Expr1
                               FROM            tbl_ProductPlacementMissions
                               WHERE        (fld_PPMToZoneId = tbl_Zones.ZoneCode) AND (fld_PPMStoreCode = tbl_Zones.ZoneStoreCode) AND (fld_PPMStatus IN (0, 1)))
							   where tbl_Zones.ZoneStoreCode=(SELECT Coalesce(i.TagInDestinationId,'0') from deleted i) and tbl_Zones.ZoneCode = (SELECT Coalesce( i.TagZone,'0') from deleted i) 
END
ELSE
BEGIN
UPDATE tbl_Zones
SET ZoneOccupiedCapacity =   (SELECT        COUNT(DISTINCT ProductSerial) AS Expr1
                               FROM            tbl_Tags
                               WHERE        (TagInDestinationId = tbl_Zones.ZoneStoreCode) AND (TagZone = tbl_Zones.ZoneCode)) +
                             (SELECT        COUNT(DISTINCT fld_PPMProductSerial) AS Expr1
                               FROM            tbl_ProductPlacementMissions
                               WHERE        (fld_PPMToZoneId = tbl_Zones.ZoneCode) AND (fld_PPMStoreCode = tbl_Zones.ZoneStoreCode) AND (fld_PPMStatus IN (0, 1)))
							   where tbl_Zones.ZoneStoreCode=(SELECT Coalesce(i.TagInDestinationId,'0') from inserted i) and  tbl_Zones.ZoneCode=(SELECT Coalesce( i.TagZone,'0') from inserted i) 
END

GO
DISABLE TRIGGER [dbo].[TAGS_UpdateZoneCapacity]
    ON [dbo].[tbl_Tags];

GO    
CREATE TRIGGER [dbo].[TriggerAfterActionInTblTag]
ON [dbo].[tbl_Tags]
AFTER INSERT, DELETE, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS (SELECT 1 FROM deleted) RETURN;

    BEGIN TRY
        --------------------------------------------------------------------
        -- 1. INSERT (سریع و مستقیم)
        --------------------------------------------------------------------
        INSERT INTO dbo.tbl_TablesChangeLog (TableName, RecordKey, ChangeDescription, UserId)
        SELECT
            N'tbl_Tags',
            i.ProductSerial,
            (
                SELECT
                    'INSERT' AS [action],
                    i.Id, i.ProductSerial, i.ProductCode, i.TagEpc, i.ProjectCode,
                    i.ProductCount, i.ProductName, i.ProductType, i.ProductStatus,
                    i.TagStatus, i.TagRegisterShamsiUnixDate, i.TagRegisterUser,
                    i.TagTreeParentId, i.TagTreeSecondParentId, i.TagTreeParentsId,
                    i.NewProductSerial,
                    JSON_QUERY(CASE WHEN ISJSON(i.ProductProperties) = 1 THEN i.ProductProperties END) AS ProductProperties,
                    i.[Lock], i.Username, i.DeviceId, i.DeviceIp, i.Freeze, i.Deactivate,
                    i.TagInActionId, i.TagInDestinationId, i.TagInActionId2, i.TagInDestinationId2,
                    i.fld_ProductPropertyAId, i.fld_ProductPropertyBId, i.fld_ProductPropertyCId,
                    i.RegCode, i.fld_LastModifierUser, i.ContractStatus, i.TagZone,
                    i.TagRegisterDateTime, i.ReProduct,
                    JSON_QUERY(CASE WHEN ISJSON(i.fld_LastInspectResult) = 1 THEN i.fld_LastInspectResult END) AS fld_LastInspectResult,
                    i.fld_ProductGroup, i.fld_ProductBrand, i.fld_InspectActionId,
                    i.fld_ProductSubGroup, i.fld_ProductClass, i.Temp, i.TagTreeParentsEpc,
                    i.TagTreeParentSerial, i.TagEpc2
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            ),
            i.Username
        FROM inserted i
        WHERE NOT EXISTS (SELECT 1 FROM deleted d WHERE d.ProductSerial = i.ProductSerial AND d.TagEpc = i.TagEpc);

        --------------------------------------------------------------------
        -- 2. DELETE (سریع)
        --------------------------------------------------------------------
        INSERT INTO dbo.tbl_TablesChangeLog (TableName, RecordKey, ChangeDescription, UserId)
        SELECT
            N'tbl_Tags',
            d.ProductSerial,
            (SELECT 'DELETE' AS [action], d.TagEpc AS [tagEpc] FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            d.Username
        FROM deleted d
        WHERE NOT EXISTS (SELECT 1 FROM inserted i WHERE i.ProductSerial = d.ProductSerial AND i.TagEpc = d.TagEpc);

        --------------------------------------------------------------------
        -- 3. UPDATE (بهینه‌سازی شده با حذف Dynamic SQL)
        --------------------------------------------------------------------
        IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
        BEGIN
            INSERT INTO dbo.tbl_TablesChangeLog (TableName, RecordKey, ChangeDescription, UserId)
            SELECT 
                N'tbl_Tags',
                i.ProductSerial,
                N'{"action":"UPDATE","changes":{' + 
                    STRING_AGG(
                        CAST(QUOTENAME(v.ColName, '"') + ':{"oldValue":' + v.OldVal + ',"newValue":' + v.NewVal + '}' AS NVARCHAR(MAX)), 
                        ','
                    ) + N'}}',
                i.Username
            FROM inserted i
            JOIN deleted d ON i.ProductSerial = d.ProductSerial AND i.TagEpc = d.TagEpc
            CROSS APPLY (
                
                SELECT N'ProductName', 
                       QUOTENAME(STRING_ESCAPE(ISNULL(d.ProductName, ''), 'json'), '"'), 
                       QUOTENAME(STRING_ESCAPE(ISNULL(i.ProductName, ''), 'json'), '"')
                WHERE ISNULL(i.ProductName, '') <> ISNULL(d.ProductName, '')
                
                UNION ALL
                SELECT N'ProductCount', 
                       CAST(ISNULL(CAST(d.ProductCount AS NVARCHAR(MAX)), 'null') AS NVARCHAR(MAX)), 
                       CAST(ISNULL(CAST(i.ProductCount AS NVARCHAR(MAX)), 'null') AS NVARCHAR(MAX))
                WHERE ISNULL(i.ProductCount, 0) <> ISNULL(d.ProductCount, 0)

                UNION ALL
                SELECT N'Lock', 
                       CASE WHEN d.[Lock] = 1 THEN 'true' ELSE 'false' END, 
                       CASE WHEN i.[Lock] = 1 THEN 'true' ELSE 'false' END
                WHERE i.[Lock] <> d.[Lock]

                UNION ALL
                SELECT N'ProductProperties', 
                       ISNULL(d.ProductProperties, 'null'), 
                       ISNULL(i.ProductProperties, 'null')
                WHERE ISNULL(i.ProductProperties, '') <> ISNULL(d.ProductProperties, '')
                
            ) v(ColName, OldVal, NewVal)
            GROUP BY i.ProductSerial, i.Username;
        END

    END TRY
    BEGIN CATCH
    END CATCH
END