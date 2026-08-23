 CREATE Function [dbo].[GetPersianDatePart]
(
	@Input DateTime
)
Returns Table
As
Return	(
		Select	Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  1 , 4) As Int)	As [Year]
		,	Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  6 , 2) As Int)	As [Month]
		,	Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  9 , 2) As Int)	As [Day]
		,	Case	
				When	Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  6 , 2) As Int) Between 1 And 3	Then	N'بهار'
				When	Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  6 , 2) As Int) Between 4 And 6	Then	N'تابستان'
				When	Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  6 , 2) As Int) Between 7 And 9	Then	N'پاییز'
				Else														N'زمستان'
			End										As [Season]
		,	Case	
				When	Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  6 , 2) As Int) Between 1 And 3	Then	1
				When	Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  6 , 2) As Int) Between 4 And 6	Then	2
				When	Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  6 , 2) As Int) Between 7 And 9	Then	3
				Else														4
			End										As [SeasonNumber]
		,	
			Choose(Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  6 , 2) As Int) , N'فروردین', N'اردیبهشت', N'خرداد', N'تیر', N'مرداد', N'شهریور', N'مهر', N'آبان', N'آذر', N'دی', N'بهمن', N'اسفند')	As [MonthName]
		,	Case
				When Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  9 , 2) As Int) Between 1 And 10		Then	1
				When Cast(Substring( [dbo].GeorgianDateToJalaliDate(@Input) ,  9 , 2) As Int) Between 11 And 20	Then	2
				Else														3
			End										As [TenthNumber]
			
					
					
	)