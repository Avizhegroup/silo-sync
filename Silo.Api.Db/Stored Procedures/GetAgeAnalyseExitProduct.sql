-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAgeAnalyseExitProduct]
	-- Add the parameters for the stored procedure here
	 @Serial int,@ExitDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 SELECT        CASE WHEN (( tbl_Tags.TagRegisterDateTime < @ExitDate) AND 
                         (tbl_Tags.TagRegisterDateTime >= DATEADD(month, - 1, @ExitDate))) 
                         THEN N'تا یک ماه' WHEN ((tbl_Tags.TagRegisterDateTime < DATEADD(month, - 1, 
                         @ExitDate)) AND (tbl_Tags.TagRegisterDateTime >= DATEADD(month, - 3, 
                         @ExitDate))) THEN N'یک تا سه ماه' WHEN ((tbl_Tags.TagRegisterDateTime < DATEADD(month, - 3, 
                         @ExitDate)) AND (tbl_Tags.TagRegisterDateTime >= DATEADD(month, - 6, 
                         @ExitDate))) THEN N'سه تا شش ماه' WHEN ((tbl_Tags.TagRegisterDateTime < DATEADD(month, - 6, 
                         @ExitDate)) AND (tbl_Tags.TagRegisterDateTime >= DATEADD(month, - 12, 
                         @ExitDate))) THEN N'شش ماه تا یک سال' WHEN ((tbl_Tags.TagRegisterDateTime < DATEADD(month, - 12, 
                         @ExitDate))) THEN N'بالای یک سال' END AS AgeTitle
FROM            tbl_Tags
WHERE        (ProductSerial = @Serial)
GROUP BY CASE WHEN ((tbl_Tags.TagRegisterDateTime < @ExitDate) AND 
                         (tbl_Tags.TagRegisterDateTime >= DATEADD(month, - 1, @ExitDate))) 
                         THEN N'تا یک ماه' WHEN ((tbl_Tags.TagRegisterDateTime < DATEADD(month, - 1, 
                         @ExitDate)) AND (tbl_Tags.TagRegisterDateTime >= DATEADD(month, - 3, 
                         @ExitDate))) THEN N'یک تا سه ماه' WHEN ((tbl_Tags.TagRegisterDateTime < DATEADD(month, - 3, 
                         @ExitDate)) AND (tbl_Tags.TagRegisterDateTime >= DATEADD(month, - 6, 
                         @ExitDate))) THEN N'سه تا شش ماه' WHEN ((tbl_Tags.TagRegisterDateTime < DATEADD(month, - 6, 
                         @ExitDate)) AND (tbl_Tags.TagRegisterDateTime >= DATEADD(month, - 12, 
                         @ExitDate))) THEN N'شش ماه تا یک سال' WHEN ((tbl_Tags.TagRegisterDateTime < DATEADD(month, - 12, 
                         @ExitDate))) THEN N'بالای یک سال' END
 
                      
END
