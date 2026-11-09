-- Migration: AI product image pattern + image search page
-- Idempotent: safe to run more than once.

-- 1) New column used by "AddProduct" (image analysis) and by the image search page.
IF COL_LENGTH('dbo.tbl_Products', 'fld_ProductPattern') IS NULL
    ALTER TABLE dbo.tbl_Products ADD fld_ProductPattern NVARCHAR(MAX) NULL;
GO

-- 2) Sidebar link (shown for admins automatically; other roles get access through the Access page / tbl_UserClaim).
--    The link is created beside the existing "product/add" link and copies its URL style, parent and level.
IF NOT EXISTS (SELECT 1 FROM dbo.tbl_MenuLink WHERE fld_MenuLinkUrl LIKE N'%product/imagesearch')
BEGIN
    DECLARE @addUrl NVARCHAR(256), @parent INT, @level INT, @icon NVARCHAR(256);

    SELECT TOP 1 @addUrl = fld_MenuLinkUrl, @parent = fld_MenuLinkParentId, @level = fld_MenuLinkLevel
    FROM dbo.tbl_MenuLink
    WHERE fld_MenuLinkUrl LIKE N'%product/add';

    IF @addUrl IS NOT NULL
    BEGIN
        INSERT INTO dbo.tbl_MenuLink
            (fld_MenuLinkId, fld_MenuLinkTitle, fld_MenuLinkParentId, fld_MenuLinkLevel, fld_MenuLinkUrl, fld_MenuLinkShown, fld_MenuLinkIconName, fld_MenuLinkIsUserDedicated)
        VALUES
            ((SELECT ISNULL(MAX(fld_MenuLinkId), 0) + 1 FROM dbo.tbl_MenuLink),
             N'جستجوی کالا با تصویر', @parent, @level,
             REPLACE(@addUrl, N'product/add', N'product/imagesearch'),
             1, N'image_search', 0);
    END
END
GO
