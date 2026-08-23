CREATE VIEW dbo.inventory_rep_ProductCode
AS
SELECT        dbo.tbl_Tags.ProductCode AS [کد کالا], dbo.tbl_Products.ProductTitle AS [عنوان کالا], dbo.tbl_Products.ProductTechnicalCode AS [کد فنی], SUM(dbo.tbl_Tags.ProductCount) AS [مقدار شناسایی شده], 
                         COUNT(DISTINCT dbo.tbl_Tags.ProductSerial) AS [تعداد شناسایی شده]
FROM            dbo.tbl_Products RIGHT OUTER JOIN
                         dbo.tbl_Tags ON dbo.tbl_Products.ProductCode = dbo.tbl_Tags.ProductCode RIGHT OUTER JOIN
                         dbo.tbl_InventoryTags ON dbo.tbl_Tags.TagEpc = dbo.tbl_InventoryTags.fld_InventoryTagEPC
WHERE        (dbo.tbl_InventoryTags.fld_InventoryDate >= '1401/08/19') AND (dbo.tbl_Tags.ProductCode IS NOT NULL)
GROUP BY dbo.tbl_Tags.ProductCode, dbo.tbl_Products.ProductTitle, dbo.tbl_Products.ProductTechnicalCode

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "tbl_Products"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 264
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_Tags"
            Begin Extent = 
               Top = 6
               Left = 302
               Bottom = 136
               Right = 554
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_InventoryTags"
            Begin Extent = 
               Top = 6
               Left = 592
               Bottom = 136
               Right = 860
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'inventory_rep_ProductCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'inventory_rep_ProductCode';

