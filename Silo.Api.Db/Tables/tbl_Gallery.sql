CREATE TABLE [dbo].[tbl_Gallery] (
    [fld_GalleryId]                 INT            IDENTITY (1, 1) NOT NULL,
    [fld_GalleryUserId]             NVARCHAR (128) NULL,
    [fld_GalleryMediaName]          NVARCHAR (128) NULL,
    [fld_GalleryMediaPath]          NVARCHAR (512) NULL,
    [fld_GalleryUsageType]          INT            DEFAULT ((0)) NULL,
    [fld_GalleryUploadDateTime]     DATETIME       DEFAULT (getdate()) NULL,
    [fld_GalleryUsageId]            NVARCHAR (128) NULL,
    [fld_GalleryMediaExtensionType] INT            NULL,
    PRIMARY KEY CLUSTERED ([fld_GalleryId] ASC)
);

