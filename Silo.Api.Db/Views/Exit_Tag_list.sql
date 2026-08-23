CREATE VIEW dbo.Exit_Tag_list
AS
SELECT DISTINCT 
                         TOP (100) PERCENT dbo.tbl_TagsMovement.HMovementActionId AS [کد عملیات خروج], dbo.tbl_TagsMovement.HTagsMovementDate AS [تاریخ خروج], dbo.tbl_TagsMovement.HTagsMovementTime AS [ساعت خروج], 
                         dbo.tbl_TagsMovement.ProductSerial AS [سریال محموله], dbo.tbl_Tags.fld_ProductPropertyCId AS [سایز کالا], dbo.tbl_Tags.ProductStatus AS [کنترل کیفیت], dbo.tbl_Tags.ProductCode AS [کد کالا], 
                         dbo.tbl_Tags.ProductName AS [عنوان کالا], dbo.tbl_Tags.ProductCount AS [مقدار کالا], CASE dbo.tbl_Tags.TagStatus WHEN 0 THEN N'بسته بندی' WHEN 1 THEN N'داخل انبار' WHEN 2 THEN N'خارج شده' END AS [وضعیت محموله], 
                         dbo.tbl_MovementActions.MovementActionCarPlaque AS [پلاک ماشین], dbo.tbl_MovementActions.MovementActionDriverName AS [نام راننده], COALESCE (dbo.tbl_MovementActions.MovementActionDriverMobile, N'') 
                         AS [موبایل راننده], dbo.tbl_MovementActions.MovementActionData AS [اطلاعات عملیات خروج], COALESCE (dbo.tbl_MovementActions.MovementActionUHFLogId, 0) AS [عملیات گیت], 
                         dbo.GetProductAnalyseAge(dbo.tbl_Tags.TagRegisterShamsiUnixDate) AS [آنالیز سنی], CASE COALESCE (JSON_VALUE(dbo.tbl_MovementActions.MovementActionData, '$.DetinationType'), N'0') 
                         WHEN '2' THEN N'صادرات' WHEN '0' THEN N'نا مشخص' WHEN '1' THEN N'فروش داخل' END AS [نوع فروش], COALESCE (JSON_VALUE(dbo.tbl_MovementActions.MovementActionData, '$.Destination'), N'نامشخص') AS [مقصد فروش], 
                         COALESCE (dbo.GetExitProductAnalyseAge(dbo.tbl_Tags.TagRegisterShamsiUnixDate, dbo.tbl_TagsMovement.HTagsMovementDate), N'نامشخص') AS [آنالیز سنی خروج]
FROM            dbo.tbl_TagsMovement LEFT OUTER JOIN
                         dbo.tbl_MovementActions ON dbo.tbl_TagsMovement.HMovementActionId = dbo.tbl_MovementActions.MovementActionId LEFT OUTER JOIN
                         dbo.tbl_Tags ON dbo.tbl_TagsMovement.ProductSerial = dbo.tbl_Tags.ProductSerial
WHERE        (dbo.tbl_MovementActions.MovementActionTp = 2) AND (dbo.tbl_MovementActions.MovementActionCarPlaque <> '') AND (dbo.tbl_Tags.TagStatus = 2)
ORDER BY [تاریخ خروج], [ساعت خروج]

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[37] 4[27] 2[30] 3) )"
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
         Begin Table = "tbl_TagsMovement"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 274
               Right = 270
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_MovementActions"
            Begin Extent = 
               Top = 96
               Left = 325
               Bottom = 226
               Right = 577
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_Tags"
            Begin Extent = 
               Top = 6
               Left = 598
               Bottom = 279
               Right = 834
            End
            DisplayFlags = 280
            TopColumn = 20
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
         Width = 2400
         Width = 1500
         Width = 1500
         Width = 12435
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1545
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2625
         Alias = 1830
         Table = 1815
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'Exit_Tag_list';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'Exit_Tag_list';

