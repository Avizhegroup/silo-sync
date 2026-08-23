CREATE PROCEDURE [dbo].[GetRegisterStats]
    @Type INT 
AS   
SET NOCOUNT ON;  
DECLARE @FromDate datetime;
DECLARE @ToDate datetime;
DECLARE @TempDt table(RowValue  decimal(18,2),TypeIndex INT);

DECLARE db_cursor CURSOR FOR 
SELECT      dbo.JalaliDateToGeorgianDate(DateShamsi,'01:01'), case @Type 
when 1 then DATEADD(DAY ,1,dbo.JalaliDateToGeorgianDate(DateShamsi,'01:01'))
when 2 then  DATEADD(DAY ,7,dbo.JalaliDateToGeorgianDate(DateShamsi,'01:01')) 
when 3 then  DATEADD(month ,1,dbo.JalaliDateToGeorgianDate(DateShamsi,'01:01'))
when 4 then  DATEADD(month ,3,dbo.JalaliDateToGeorgianDate(DateShamsi,'01:01')) 
when 5 then  DATEADD(year ,1,dbo.JalaliDateToGeorgianDate(DateShamsi,'01:01')) end  as [EXP]
FROM            tbl_Calendar
WHERE      ( SELECT case @Type  when 2 then  FlagStartWeek 
 when 3 then FlagStartMonth   
 when 4 then FlagStartSeason  
 when 5 then  FlagStartYear  end  ) = 1 OR [Day] IS NOT NULL
OPEN db_cursor  
FETCH NEXT FROM db_cursor   
INTO @FromDate, @ToDate
WHILE @@FETCH_STATUS = 0  
BEGIN
 
insert into @TempDt   SELECT        Sum(ProductCount) ,@Type
FROM            tbl_Tags
WHERE   fld_ProductPropertyAId >0 AND   tbl_Tags.TagRegisterDateTime < @ToDate  

FETCH NEXT FROM db_cursor   
    INTO @FromDate, @ToDate 
END   
CLOSE db_cursor;  
DEALLOCATE db_cursor; 
declare @Json nvarchar(max) = (select AVG(RowValue) as [Avg],Max(RowValue) as [Max],@Type as [Type] from @TempDt for json Path,ROOT('Reg'));
INSERT INTO [dbo].[tbl_Item]
           ([fld_SaveDate]
           ,[fld_SaveUser]
           ,[fld_Data])
     VALUES
          (getdate()
           ,N'User'
           ,(select @json)
		   ) 
