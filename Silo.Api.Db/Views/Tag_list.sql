CREATE VIEW dbo.Tag_list
AS
SELECT        dbo.tbl_Tags.ProductSerial AS [سریال محموله], dbo.tbl_Tags.ProductCode AS [کد کالا], dbo.tbl_Products.ProductTitle AS [عنوان کالا], dbo.tbl_Tags.RegCode AS [کد فنی], dbo.tbl_ProductType.ProductTypeTitle AS [طرح کالا], 
                         dbo.tbl_ProductPropertyC.fld_ProductPropertyCTitle AS [سایز کالا], dbo.tbl_ProductStatus.ProductStatusTitle AS [درجه کیفیت], dbo.tbl_Tags.ProductCount AS [مقدار محموله], 
                         CASE dbo.tbl_Tags.TagStatus WHEN 0 THEN N'بسته بندی' WHEN 1 THEN N'داخل انبار' WHEN 2 THEN N'خارج شده' END AS [وضعیت محموله], dbo.tbl_Tags.TagRegisterShamsiUnixDate AS [تاریخ و ساعت تولید], 
                         dbo.tbl_Tags.Username AS [کاربر ثبت], dbo.tbl_ProductPropertyA.fld_ProductPropertyATitle AS [خط تولید], dbo.tbl_ProductPropertyB.fld_ProductPropertyBTitle AS [شیفت تولید], dbo.tbl_Zones.ZoneTitle AS [لوکیشن انبار], 
                         dbo.tbl_Tags.TagEpc AS [شناسه RFID], dbo.GetProductAnalyseAge(dbo.tbl_Tags.TagRegisterShamsiUnixDate) AS [آنالیز سنی],
                             (SELECT        TOP (1) Name
                               FROM            dbo.splitstring(dbo.tbl_Products.ProductTitle, '-') AS STRING_SPLIT_1) AS برند, CASE dbo.tbl_Tags.ReProduct WHEN 0 THEN N'محصول کامل' WHEN 1 THEN N'محصول نیمه آماده' END AS [محصول نمیه آماده]
FROM            dbo.tbl_Tags LEFT OUTER JOIN
                         dbo.tbl_Zones ON dbo.tbl_Tags.TagZone = dbo.tbl_Zones.ZoneCode LEFT OUTER JOIN
                         dbo.tbl_ProductPropertyC ON dbo.tbl_Tags.fld_ProductPropertyCId = dbo.tbl_ProductPropertyC.fld_ProductPropertyCId LEFT OUTER JOIN
                         dbo.tbl_ProductPropertyB ON dbo.tbl_Tags.fld_ProductPropertyBId = dbo.tbl_ProductPropertyB.fld_ProductPropertyBId LEFT OUTER JOIN
                         dbo.tbl_ProductPropertyA ON dbo.tbl_Tags.fld_ProductPropertyAId = dbo.tbl_ProductPropertyA.fld_ProductPropertyAId LEFT OUTER JOIN
                         dbo.tbl_ProductStatus ON dbo.tbl_Tags.ProductStatus = dbo.tbl_ProductStatus.ProductStatusCode LEFT OUTER JOIN
                         dbo.tbl_ProductType ON dbo.tbl_Tags.ProductType = dbo.tbl_ProductType.ProductTypeCode LEFT OUTER JOIN
                         dbo.tbl_Products ON dbo.tbl_Tags.ProductCode = dbo.tbl_Products.ProductCode

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[51] 4[7] 2[35] 3) )"
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
         Begin Table = "tbl_Tags"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 210
               Right = 274
            End
            DisplayFlags = 280
            TopColumn = 28
         End
         Begin Table = "tbl_Zones"
            Begin Extent = 
               Top = 6
               Left = 312
               Bottom = 136
               Right = 525
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_ProductPropertyC"
            Begin Extent = 
               Top = 6
               Left = 563
               Bottom = 136
               Right = 791
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_ProductPropertyB"
            Begin Extent = 
               Top = 6
               Left = 829
               Bottom = 136
               Right = 1056
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_ProductPropertyA"
            Begin Extent = 
               Top = 138
               Left = 312
               Bottom = 268
               Right = 540
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_ProductStatus"
            Begin Extent = 
               Top = 6
               Left = 1094
               Bottom = 119
               Right = 1285
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_ProductType"
            Begin Extent = 
               Top = 138
               Left = 578
               Bottom = 268
               Ri', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'Tag_list';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'ght = 784
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_Products"
            Begin Extent = 
               Top = 138
               Left = 822
               Bottom = 268
               Right = 1032
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
      Begin ColumnWidths = 18
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1545
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'Tag_list';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'Tag_list';

