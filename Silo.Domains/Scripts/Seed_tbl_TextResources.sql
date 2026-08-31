-- Manual seed script generated from TextResources.resx
-- Run this against the WMS database before first use.
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Account_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Account_Add', N'کاربر جدید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Account_Index')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Account_Index', N'تعریف کاربران');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Account_Login')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Account_Login', N'اطلاعات کاربری را وارد کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Add', N'افزودن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Alert_Fail')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Alert_Fail', N'در انجام عملیات مشکلی بوجود آمده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Alert_TryAgain')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Alert_TryAgain', N'لطفاً دوباره تلاش کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Alert_Success')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Alert_Success', N'عملیات با موفقیت به انجام شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_AppName')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_AppName', N'مدیریت انبار محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_AppName_Dashboard')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_AppName_Dashboard', N'داشبورد مدیریت انبار محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_AppVersion')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_AppVersion', N'1.0.0');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Attention')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Attention', N'توجه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_BackPage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_BackPage', N'صفحه قبل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity_Destination')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity_Destination', N'ظرفیت مقصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity_Free')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity_Free', N'ظرفیت آزاد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity_From')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity_From', N'ظرفیت مبدا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity_Occupied')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity_Occupied', N'ظرفیت اشغال شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_Pl')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_Pl', N'خط');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_ProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_ProductCode', N'کد محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_ProductDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_ProductDate', N'تاریخ تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_Qc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_Qc', N'درجه کیفیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Sort_Asc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Sort_Asc', N'صعودی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Sort_Desc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Sort_Desc', N'نزولی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Sorting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Sorting', N'جهت ترتیب');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_Regcode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_Regcode', N'کد فنی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_Shift')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_Shift', N'شیفت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Clear')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Clear', N'پاک کردن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Close')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Close', N'بستن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Code', N'کد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Compeletable_Edit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Compeletable_Edit', N'ویرایش {0}');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Compeletable_ReportOn')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Compeletable_ReportOn', N'گزارش تجمعی بر روی فیلد {0}');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Confirm')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Confirm', N'تائید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Contradiction')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Contradiction', N'لطفا مغایرت اطلاعات را رفع و دوباره امتحان کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Count_Rfid')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Count_Rfid', N'موجودی Rfid');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Date')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Date', N'تاریخ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Description')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Description', N'توضیحات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DetailInfo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DetailInfo', N'نمایش ریز اطلاعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Details')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Details', N'جزئیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Detination')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Detination', N'مقصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Disconfirm')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Disconfirm', N'انصراف');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Doc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Doc', N'سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocCode', N'شماره سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DriverName')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DriverName', N'نام راننده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Edit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Edit', N'ویرایش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter', N'ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_EnterDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_EnterDate', N'تاریخ ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_EnterTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_EnterTime', N'زمان ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExitTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExitTime', N'زمان خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExportChart')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExportChart', N'دریافت نمودار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExportExcel')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExportExcel', N'دریافت اکسل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Finished')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Finished', N'اتمام رسیده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_FirstEnterDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_FirstEnterDate', N'تاریخ اولین ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_From')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_From', N'مبدا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_FromDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_FromDate', N'از تاریخ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_FromTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_FromTime', N'از ساعت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GateOpCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GateOpCode', N'کد عملیات گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gates')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gates', N'گیت ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Security_Index')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Security_Index', N'ثبت جابجایی کالا از طریق گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GoToToday')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GoToToday', N'برو به امروز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_History')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_History', N'تاریخچه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Home_Index')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Home_Index', N'صفحه اصلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Home_Stats')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Home_Stats', N'داشبورد تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_IConfirm')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_IConfirm', N'تائید می کنم');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Ignore')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Ignore', N'لغو');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Ignored')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Ignored', N'لغو شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inquiry')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inquiry', N'استعلام');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_LastEnterDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_LastEnterDate', N'تاریخ آخرین ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Line')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Line', N'خط تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Loading')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Loading', N'در حال بارگذاری ..');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Account_Login')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Account_Login', N'ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Logout')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Logout', N'خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Modal_Inquiry')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Modal_Inquiry', N'استعلام موجودی کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Modal_Range_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Modal_Range_Title', N'انتخاب بازه زمانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Name')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Name', N'نام فارسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_NextPage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_NextPage', N'صفحه بعد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Operation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Operation', N'عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_OperationCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_OperationCode', N'کد عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Order_IgnoreMessage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Order_IgnoreMessage', N'آیا از لغو عملیات جایگذاری اطمینان دارید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Password')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Password', N'کلمه عبور');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Pdf')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Pdf', N'دریافت PDF');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Percent')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Percent', N'درصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Phone')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Phone', N'شماره تلفن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Placeholder_Password')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Placeholder_Password', N'[بدون تغییر]');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Plaque')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Plaque', N'پلاک');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Position')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Position', N'جانمایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Position_Index')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Position_Index', N'گزارش جانمایی محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Position_Order')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Position_Order', N'تعریف برنامه جانمایی کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Position_Place')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Position_Place', N'ثبت جابجایی مستقیم کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Print')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Print', N'چاپ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product', N'تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductCount', N'مقدار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductName')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductName', N'نام محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductSerial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductSerial', N'سریال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductType', N'نوع کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Size')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Size', N'سایز کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Recalculate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Recalculate', N'محاسبه مجدد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RecordsCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RecordsCount', N'تعداد رکورد های نمایش داده شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Reload')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Reload', N'بارگذاری مجدد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Remain')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Remain', N'باقی مانده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Remove')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Remove', N'حذف');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Reports')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Reports', N'گزارشات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_AverageExit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_AverageExit', N'میانگین فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_Destination')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_Destination', N'تفکیک مقصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_Pl')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_Pl', N'تفکیک خط تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_ProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_ProductCode', N'تفکیک کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_ProductDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_ProductDate', N'تفکیک تاریخ تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_ProductSize')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_ProductSize', N'تفکیک سایز محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_ProductType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_ProductType', N'تفکیک طرح محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_ProductType_6_Top')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_ProductType_6_Top', N'6 طرح با بیشترین موجودی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_Qc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_Qc', N'تفکیک درجه کیفی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_Regcode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_Regcode', N'تفکیک کدفنی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Diff_Shift')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Diff_Shift', N'تفکیک شیفت تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_Enter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_Enter', N'گزارش ورود کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_Exit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_Exit', N'گزارش عملیات های خروج کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Product')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Product', N'گزارش تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Product_AgeAnalysis')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Product_AgeAnalysis', N'آنالیز سنی کالاهای فروخته شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Sales')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Sales', N'گزارش فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_Store')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_Store', N'گزارش موجودی انبارها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Warehouse_10_Top')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Warehouse_10_Top', N'محصولات با بیشترین موجودی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Warehouse_AgeAnalysis')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Warehouse_AgeAnalysis', N'آنالیز سنی موجودی انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Warehouse_ZoneCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Warehouse_ZoneCode', N'سهم انبارک ها در موجودی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Role')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Role', N'سطح دسترسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Route_Nothing')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Route_Nothing', N'آدرس مورد نظر یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Row')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Row', N'ردیف');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Sales')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Sales', N'فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Sales_AvgRegisterDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Sales_AvgRegisterDate', N'میانگین سنی فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Save')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Save', N'ذخیره');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search', N'جستجو');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SearchFilter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SearchFilter', N'فیلتر های جستجو');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search_Product')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search_Product', N'جستجوی محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Security_GetDataByUhf')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Security_GetDataByUhf', N'دریافت اطلاعات ثبت شده قبلی با کد عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_See')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_See', N'مشاهده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Send')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Send', N'ارسال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings', N'تنظیمات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Status', N'وضعیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summary_Daily')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summary_Daily', N'روزانه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summary_Monthly')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summary_Monthly', N'ماهانه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summary_Seasonly')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summary_Seasonly', N'فصلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summary_Weekly')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summary_Weekly', N'هفتگی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summary_Yearly')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summary_Yearly', N'سالانه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_AverageAge')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_AverageAge', N'میانگین سنی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_Designs')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_Designs', N'تنوع طرح');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_Deviation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_Deviation', N'فاصله از میانگین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_Export')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_Export', N'سهم صادرات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_FreeLines')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_FreeLines', N'لاین های خالی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_InStore_Amount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_InStore_Amount', N'موجودی مقداری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_InStore_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_InStore_Count', N'موجودی تعدادی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_Lines')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_Lines', N'خطوط فعال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_LoadCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_LoadCount', N'تعداد بارگیری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_LowQualities')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_LowQualities', N'تولید درجه D و CG');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_MaxDistance')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_MaxDistance', N'فاصله از حداکثر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_Size')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_Size', N'تنوع سایز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Summery_SumCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Summery_SumCount', N'جمع متراژ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SumValue')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SumValue', N'جمع مقدار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Tabstrip_Bar')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Tabstrip_Bar', N'نمودار میله ای');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Tabstrip_Pie')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Tabstrip_Pie', N'نمودار دایره ای');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Tagzone')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Tagzone', N'لوکیشن جایگذاری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TDA')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TDA', N'تدبیر داده آویژه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Time')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Time', N'ساعت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Title', N'عنوان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ToDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ToDate', N'تا تاریخ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ToTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ToTime', N'تاساعت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Truck_Index')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Truck_Index', N'ماموریت جانمایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_Modal_Code_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_Modal_Code_Serial', N'کد یا سریال محصول موردنظر را وارد کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_Modal_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_Modal_Title', N'آیا محصول موردنظر در محل مشخص شده قرار گرفت؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_Name')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_Name', N'نام');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_TruckNumber')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_TruckNumber', N'لیفتراک');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_User')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_User', N'کاربر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Username')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Username', N'نام کاربری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Account_Edit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Account_Edit', N'ویرایش کاربران');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Empty')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Empty', N'فیلد موردنظر را پر کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SellType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SellType', N'نوع فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_EmptyRows')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_EmptyRows', N'ردیف های اطلاعاتی {0} خالی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_LoginFail')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_LoginFail', N'مشخصات کاربری یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Product_NotFound')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Product_NotFound', N'محصول مورد نظر یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Remote')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Remote', N'اطلاعات فیلد {0} تکراری است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Required')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Required', N'اطلاعات فیلد {0} الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_TimeRangeRequired')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_TimeRangeRequired', N'انتخاب بازه زمانی الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Value')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Value', N'مقدار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Wait')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Wait', N'لطفا چند لحظه صبر کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse', N'انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Year')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Year', N'سال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Zone')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Zone', N'لوکیشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Zones')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Zones', N'لوکیشن ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Zones_Search_Pure')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Zones_Search_Pure', N'جستجوی لوکیشن ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Zone_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Zone_Code', N'کد لوکیشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Zone_Search')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Zone_Search', N'جستجوی لوکیشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_RangeAge')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_RangeAge', N'بازه سنی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Store_Like')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Store_Like', N'یافتن همانند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Exit_Main')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Exit_Main', N'گزارش خروج ماشین های حمل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Exit_SumOnProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Exit_SumOnProductCode', N'تجمعی بر روی کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Exit_Full')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Exit_Full', N'گزارش ریز کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Choose')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Choose', N'انتخاب');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExitDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExitDate', N'تاریخ خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity_Between2040')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity_Between2040', N'بین 20 تا 40 درصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity_Between4060')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity_Between4060', N'بین 40 تا 60 درصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity_Between6080')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity_Between6080', N'بین 60 تا 80 درصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity_Empty')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity_Empty', N'خالی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity_Less20')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity_Less20', N'کمتر از 20 درصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity_More80')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity_More80', N'بیش از 80 درصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductCode', N'کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductSumCountInPack')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductSumCountInPack', N'مقدار واحد دوم');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductENTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductENTitle', N'عنوان لاتین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductPackVolume')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductPackVolume', N'حجم محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductPackWeight')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductPackWeight', N'وزن محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductProperties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductProperties', N'مشخصات محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductRegDateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductRegDateTime', N'تاریخ ثبت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductTitle', N'عنوان کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductUnit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductUnit', N'واحد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductValue')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductValue', N'مقدار کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Product_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Product_Add', N'افزودن کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RegUser')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RegUser', N'کاربر ثبت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductTypeCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductTypeCode', N'کد نوع کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductTypeTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductTypeTitle', N'عنوان نوع کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Product_AddProductType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Product_AddProductType', N'افزودن نوع کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search_ProductType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search_ProductType', N'جستجو نوع کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search_Product_Cus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search_Product_Cus', N'جستجوی کالا در نرم افزار انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductPropertyA')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductPropertyA', N'خط تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductPropertyB')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductPropertyB', N'شیفت تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductStatus', N'درجه کیفیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TagEPC')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TagEPC', N'شناسه RFID');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TagStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TagStatus', N'وضعیت محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_OperationType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_OperationType', N'نوع عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_FromZone')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_FromZone', N'مبدأ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GateType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GateType', N'نوع گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ToZone')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ToZone', N'مقصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TagAge')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TagAge', N'سن محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TagAgeAnalyse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TagAgeAnalyse', N'آنالیز سنی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_InventoryInfo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_InventoryInfo', N'انبارگردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PlacementInfo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PlacementInfo', N'جانمایی و جابجایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReadByGateLog')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReadByGateLog', N'شناسایی گیت ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_StoreTransactions')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_StoreTransactions', N'تراکنش های انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_QC')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_QC', N'کیفیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Like')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Like', N'همانند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Zones_Max')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Zones_Max', N'حداکثر ظرفیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Zones_Min')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Zones_Min', N'حداقل ظرفیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Ok')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Ok', N'متوجه شدم');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ParentCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ParentCode', N'کد والد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Resolution')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Resolution', N'رزولوشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Treeview')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Treeview', N'درختواره');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Zone_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Zone_Add', N'افزودن لوکیشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductHistory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductHistory', N'سابقه تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_TagHistory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_TagHistory', N'گزارش سابقه محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SalesHistory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SalesHistory', N'سابقه فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TagProperties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TagProperties', N'مشخصات محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Notif_Manage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Notif_Manage', N'تعریف دستور اطلاع رسانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_SendClock')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_SendClock', N'ساعت ارسال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_SendDay')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_SendDay', N'روز ارسال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_SendType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_SendType', N'روش اطلاع رسانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_TimePeriod')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_TimePeriod', N'دوره زمانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_Type', N'نوع گزارش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_ReportElements')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_ReportElements', N'المان های گزارش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_Contacts')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_Contacts', N'مخاطبین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_Content')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_Content', N'محتوا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Account_Biometric')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Account_Biometric', N'ثبت اثر انگشت برای ورود به برنامه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Login_Biometric')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Login_Biometric', N'ورود با اثر انگشت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Prefrences_Biometric')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Prefrences_Biometric', N'Biometric_User');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Biometric')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Biometric', N'اثر انگشت شناسایی نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Biometric_Registered')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Biometric_Registered', N'ابتدا اثر انگشت خود در برنامه ثبت کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Biometeric_Start')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Biometeric_Start', N'شروع شناسایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Load_Data')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Load_Data', N'دریافت اطلاعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DataList')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DataList', N'لیست اطلاعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_No')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_No', N'خیر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Remove_CheckBeforeDelete')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Remove_CheckBeforeDelete', N'آیا از حذف مورد، مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Yes')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Yes', N'بله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CheckingRules')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CheckingRules', N'قواعد کنترلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Rule_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Rule_Add', N'تعریف قواعد کنترلی ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CheckingRulesCommand')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CheckingRulesCommand', N'دستور کنترلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CheckingRulesId')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CheckingRulesId', N'کد قاعده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CheckingRulesStationCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CheckingRulesStationCode', N'ایستگاه اجرایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CheckingRulesStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CheckingRulesStatus', N'وضعیت قاعده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CheckingRulesTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CheckingRulesTitle', N'عنوان قاعده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CheckingRulesType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CheckingRulesType', N'نوع قاعده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Data_Elements')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Data_Elements', N'المان های داده ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CheckingRulesReturnResultFalse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CheckingRulesReturnResultFalse', N'پیغام در حالت خطا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CheckingRulesReturnResultTrue')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CheckingRulesReturnResultTrue', N'پیغام در حالت صحیح');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_AddElement')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_AddElement', N'افزودن المان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Rule_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Rule_Add', N'مشخصات قاعده کنترلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ResultType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ResultType', N'نوع خروجی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_StoreLocation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_StoreLocation', N'گزارش موجودی لوکیشن های انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Zones_Capacity_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Zones_Capacity_Status', N'وضعیت ظرفیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Zones_VarietyOfProduct')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Zones_VarietyOfProduct', N'تنوع کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Position_Collect')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Position_Collect', N'عملیات جمع آوری کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Remove_DeleteSuccess')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Remove_DeleteSuccess', N'اطلاعات با موفقیت حذف شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Recheck')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Recheck', N'لطفا موارد اعتبار سنجی را برطرف کنید و سپس دوباره تلاش کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_FirstDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_FirstDate', N'اولین تاریخ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_LastDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_LastDate', N'دومین تاریخ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_EmptinessCheck')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_EmptinessCheck', N'اطلاعات فرم را پر کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Capacity')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Capacity', N'ظرفیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_AppName_Silo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_AppName_Silo', N'سیلو');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Delete')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Delete', N'حذف');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Position_Collect_Contradiction_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Position_Collect_Contradiction_Count', N'مغایرت در مقدار {0} متر مربع');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Position_Collect_Contradiction_NotInDoc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Position_Collect_Contradiction_NotInDoc', N'عدم وجود در سند جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Position_Collect_Contradiction_NotInPlan')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Position_Collect_Contradiction_NotInPlan', N'عدم وجود در برنامه جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Position_Collect_CountRequest')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Position_Collect_CountRequest', N'تعداد درخواستی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Position_Collect_GetPlan')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Position_Collect_GetPlan', N'دریافت برنامه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Position_Collect_List2')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Position_Collect_List2', N'اطلاعات سند جمع آوری کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Position_Collect_PlanRequest')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Position_Collect_PlanRequest', N'برنامه پیشنهادی جمع آوری کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Position_Collect_SumValueRequest')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Position_Collect_SumValueRequest', N'متراژ درخواستی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Serials')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Serials', N'سریال ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_Choose')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_Choose', N'انتخاب لیفتراک');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Collect_Dock')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Collect_Dock', N'بارانداز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_AddPlan')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_AddPlan', N'افزودن برنامه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_ChooseDock')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_ChooseDock', N'انتخاب بارانداز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Collect_CollectAgent')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Collect_CollectAgent', N'مأمور جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Collect_EmptyCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Collect_EmptyCode', N'ابتدا با کد سند، اقدام به دریافت اطلاعات سند و برنامه جمع آوری اقدام کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Collect_Save')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Collect_Save', N'ثبت مأموریت جمع آوری کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Collect_SaveFailureOnCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Collect_SaveFailureOnCode', N'در ثبت اطلاعات با کد {0} مشکلی بوجود آمده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Remove_CheckBeforeClear')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Remove_CheckBeforeClear', N'آیا از پاک شدن صفحه مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Account_Logoff')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Account_Logoff', N'خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ActivationStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ActivationStatus', N'وضعیت فعال بودن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Active')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Active', N'فعال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_AllocationHistory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_AllocationHistory', N'تاریخچه اختصاص');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_AppName_Brief_Fa')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_AppName_Brief_Fa', N'ساده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_AppName_Fa')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_AppName_Fa', N'سامانه انبارداری هوشمند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Asset')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Asset', N'اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_AssetSerial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_AssetSerial', N'شناسه اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Asset_Assign')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Asset_Assign', N'اختصاص اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Asset_Changes')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Asset_Changes', N'تغییرات اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Asset_CreateExists')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Asset_CreateExists', N'چاپ برچسب اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_asset_details')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_asset_details', N'مشخصات و سوابق اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Asset_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Asset_Title', N'عنوان اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Asset_Verify')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Asset_Verify', N'تائید اختصاص');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Asset_VerifyActions')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Asset_VerifyActions', N'لیست اختصاص های تائید نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Back')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Back', N'بازگشت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Basic_AddField')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Basic_AddField', N'افزودن اقلام اطلاعاتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Basic_Print')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Basic_Print', N'چاپ برچسب');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Both')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Both', N'هر دو');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CcModal_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CcModal_Title', N'انتخاب مرکز هزینه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Changes_FromTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Changes_FromTime', N'از ساعت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Changes_ToTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Changes_ToTime', N'تا ساعت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Choose_Compeletable')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Choose_Compeletable', N'انتخاب {0}');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CostCenter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CostCenter', N'مرکز هزینه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CostCenterCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CostCenterCode', N'کد مرکزهزینه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Deactive')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Deactive', N'غیرفعال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DeleteModal_SubTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DeleteModal_SubTitle', N'اطلاعات حذف شده غیر قابل بازگشت است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DeleteModal_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DeleteModal_Title', N'توجه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_English')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_English', N'English');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enterable_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enterable_Title', N'پس از وارد کردن اطلاعات،دکمه Enter را بفشارید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_EnterWithExcel')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_EnterWithExcel', N'ورود با اکسل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_EnterWithoutProperties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_EnterWithoutProperties', N'ورود بدون مشخصات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error', N'خطا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExcelFile')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExcelFile', N'فایل اکسل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Farsi')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Farsi', N'فارسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_ChooseAsset')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_ChooseAsset', N'انتخاب اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Code', N'کد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Count', N'تعداد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_CurrentLocation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_CurrentLocation', N'لوکیشن فعلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Description')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Description', N'توضیحات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Details')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Details', N'جزئیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_DetectedZone')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_DetectedZone', N'لوکیشن های شناسایی شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Epc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Epc', N'شناسه تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_HDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_HDate', N'تاریخ عدم اختصاص');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_HTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_HTime', N'زمان عدم اختصاص');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Id')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Id', N'شناسه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_InventoryAlExistTgCunt')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_InventoryAlExistTgCunt', N'تعداد کل اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_InventoryCntCunt')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_InventoryCntCunt', N'اموال شناسایی شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_InventoryDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_InventoryDate', N'تاریخ اموال گردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_InventoryErrCunt')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_InventoryErrCunt', N'تگ های خطا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_InventoryProductDesc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_InventoryProductDesc', N'توضیحات اموال گردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_InventoryStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_InventoryStatus', N'وضعیت اموال گردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_InventoryUnCntCunt')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_InventoryUnCntCunt', N'اموال شناسایی نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_InventoryZoneCunt')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_InventoryZoneCunt', N'اماکن اتمام رسیده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Ip')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Ip', N'آی پی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_LocationCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_LocationCode', N'کد لوکیشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_LocationType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_LocationType', N'نوع لوکیشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Name')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Name', N'نام');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_OperationId')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_OperationId', N'شماره عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Operations')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Operations', N'عملیات ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_PersonelCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_PersonelCode', N'کد پرسنل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Personel_ParentId')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Personel_ParentId', N'سرپرست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_PowerPercent')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_PowerPercent', N'درصد قدرت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_RDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_RDate', N'تاریخ اختصاص');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_RememberMe')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_RememberMe', N'مرا بخاطر بسپار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Role')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Role', N'نوع کاربری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_RTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_RTime', N'زمان اختصاص');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_SaveType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_SaveType', N'نوع ثبت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Status', N'وضعیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Title', N'عنوان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_TwoLevelMove')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_TwoLevelMove', N'ثبت دو مرحله ای اختصاص اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_UnDetectedAsset')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_UnDetectedAsset', N'اموال شناسایی نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Username')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Username', N'نام کاربری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_FileName')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_FileName', N'عنوان فایل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GateNumber')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GateNumber', N'شماره گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GetEpc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GetEpc', N'دریافت شناسه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_HolderPersonel')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_HolderPersonel', N'پرسنل نگهدارنده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Id')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Id', N'شناسه یکتا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventory_HeaderDetail')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventory_HeaderDetail', N'اماکن عملیات شماره {0}');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventory_InventoryStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventory_InventoryStatus', N'وضعیت شمارش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventory_LocationReport')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventory_LocationReport', N'گزارش لوکیشن های کنترل شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventory_Search')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventory_Search', N'جستجوی اموال گردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventory_ZoneDetail')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventory_ZoneDetail', N'اموال اختصاص یافته به لوکیشن {0}');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_LearnMore')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_LearnMore', N'بیشتر بدانید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ListOf_Compeletable')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ListOf_Compeletable', N'لیست {0}');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Location')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Location', N'لوکیشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Location_Assign')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Location_Assign', N'اختصاص به لوکیشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Location_TypeIn')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Location_TypeIn', N'داخل سازمان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Location_TypeOut')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Location_TypeOut', N'خارج سازمان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_LocModal_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_LocModal_Title', N'انتخاب لوکیشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_MainPage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_MainPage', N'صفحه اصلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ManageFields')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ManageFields', N'مدیریت فیلد ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_MenuTitle_AboutHelp')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_MenuTitle_AboutHelp', N'معرفی و راهنما');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_MenuTitle_BasicInformation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_MenuTitle_BasicInformation', N'اطلاعات پایه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_MenuTitle_ManageAssets')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_MenuTitle_ManageAssets', N'مدیریت اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_MenuTitle_Reports')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_MenuTitle_Reports', N'گزارشات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_About')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_About', N'درباره ما');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_AboutApp')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_AboutApp', N'درباره سامانه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_Help')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_Help', N'راهنما');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_NewAsset')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_NewAsset', N'تعریف اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_NewCostCenter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_NewCostCenter', N'مرکز هزینه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_NewLocation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_NewLocation', N'اماکن استقرار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_NewOrganization')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_NewOrganization', N'تعریف سازمان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_NewPersonnel')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_NewPersonnel', N'مدیریت پرسنل سازمان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_NewStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_NewStatus', N'وضعیت اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_NewType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_NewType', N'انواع اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu_NewUser')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu_NewUser', N'مدیریت کاربر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_CascadeError')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_CascadeError', N'به علت اختصاص حداقل یک اموال به آن و یا داشتن زیرمجموعه قابل حذف نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_ConnectionError')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_ConnectionError', N'اتصال با سرور ممکن نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_DeleteText_Part1')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_DeleteText_Part1', N'از حذف اطلاعات مربوط به');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_DeleteText_Part2')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_DeleteText_Part2', N'مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_ExcelDuplicate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_ExcelDuplicate', N'اطلاعات فایل با عنوان موردنظر قبلا وارد شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_Failure')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_Failure', N'در انجام عملیات مورد مشکلی بوجود آمده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_FailureTags')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_FailureTags', N'اطلاعات تگ های زیر ثبت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_NodeCannotEdit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_NodeCannotEdit', N'گره موردنظر قابل ویرایش نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_SerialValidation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_SerialValidation', N'لطفا ابتدا سریال موردنظر را وارد کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_Success')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_Success', N'عملیات با موفقیت انجام شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_TagNotFound')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_TagNotFound', N'اطلاعات تگ مورد نظر یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_MovModal_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_MovModal_Title', N'انتخاب عملیات اختصاص');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_MovTagsModal_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_MovTagsModal_Title', N'اموال اختصاص یافته با شناسه ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_NewObject')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_NewObject', N'{0} جدید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_NoExcelModal_SubTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_NoExcelModal_SubTitle', N'تعداد اموال موردنظر را وارد کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_NoExcelModal_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_NoExcelModal_Title', N'ورود اموال بدون مشخصات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Nullable')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Nullable', N'[قابل خالی ماندن]');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PerModal_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PerModal_Title', N'انتخاب پرسنل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Personel')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Personel', N'پرسنل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Personel_Assign')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Personel_Assign', N'اختصاص به پرسنل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PreviousPage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PreviousPage', N'صفحه قبلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PrintAssetLabel')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PrintAssetLabel', N'چاپ برچسب اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PrinterModal_SubTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PrinterModal_SubTitle', N'پرینتر مورد نظر را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PrinterModal_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PrinterModal_Title', N'انتخاب چاپگر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PrintFlag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PrintFlag', N'وضعیت چاپ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Print_PrintAllSelected')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Print_PrintAllSelected', N'چاپ موارد انتخاب شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Print_Reprint')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Print_Reprint', N'چاپ شده / بازچاپ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Print_SelectAll')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Print_SelectAll', N'انتخاب همه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductStatusCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductStatusCode', N'کد وضعیت اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReaderSetting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReaderSetting', N'تنظیمات دستگاه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReadTag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReadTag', N'قرائت تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReadTagFromdb')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReadTagFromdb', N'دریافت تگ از دیتابیس');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RegisterFlag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RegisterFlag', N'وضعیت رجیستر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportAlarm_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportAlarm_Title', N'گزارش هشدار های گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportAsset_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportAsset_Title', N'گزارش اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportAssign_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportAssign_Title', N'گزارش اختصاص اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportDirectory_Directory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportDirectory_Directory', N'مسیر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportDirectory_IsChoosed')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportDirectory_IsChoosed', N'انتخاب');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportDirectory_Size')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportDirectory_Size', N'اندازه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportDirectory_SubTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportDirectory_SubTitle', N'فایل گزارش موردنظر خود را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportDirectory_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportDirectory_Title', N'انتخاب گزارش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportInventory_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportInventory_Title', N'گزارش اموال گردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Required')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Required', N'اجباری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Return')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Return', N'بازگشت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Refresh')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Refresh', N'بارگذاری مجدد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Rfid_DeviceType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Rfid_DeviceType', N'نوع دستگاه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search_FromDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search_FromDate', N'از تاریخ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search_ToDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search_ToDate', N'تا تاریخ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_StModal_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_StModal_Title', N'انتخاب وضعیت اموال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_STRINGFORMAT_Edit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_STRINGFORMAT_Edit', N'ویرایش {0} ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_STRINGFORMAT_Save')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_STRINGFORMAT_Save', N'ثبت {0}');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_STRINGFORMAT_Treeview')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_STRINGFORMAT_Treeview', N'درختواره {0}');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_STRINGFORMAT_Version')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_STRINGFORMAT_Version', N'نسخه {0}');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TagInputPlaceHolder')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TagInputPlaceHolder', N'[برای قرائت تگ کلیک کنید]');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Type', N'نوع');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Type_Compeletable')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Type_Compeletable', N'نوع {0}');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Unchanged')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Unchanged', N'[بدون تغییر]');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Unchoosed')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Unchoosed', N'مشخص نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_BadFormat')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_BadFormat', N'مقدار وارد شده در برای فیلد {0} در فرمت مناسب نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_OneFieldRequired')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_OneFieldRequired', N'حداقل یکی از دو فیلد {0} و {1} می بایست پر شود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Range')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Range', N'مقدار وارد شده در فیلد {0} باید بین {1} و {2} باشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Regex')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Regex', N'مقدار وارد شده در فرمت درست نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Remote_Uniqueness')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Remote_Uniqueness', N'اطلاعات فیلد {0}  قبلا وارد شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Required1')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Required1', N'پر کردن فیلد {0} الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Stringlength')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Stringlength', N'طول فیلد {0} باید میان {1} و {2} باشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Version')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Version', N'نسخه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_StoreReproduct')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_StoreReproduct', N'گزارش تولید نیمه آماده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_NotChoosed')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_NotChoosed', N'مشخص نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_ChooseCom')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_ChooseCom', N'انتخاب پورت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_Dollar')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_Dollar', N'نرخ دلار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_GoldPrice')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_GoldPrice', N'نرخ طلا گرمی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_GoldWeight')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_GoldWeight', N'وزن طلا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_InfoProduct')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_InfoProduct', N'اطلاعات محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_InfoStones')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_InfoStones', N'اطلاعات سنگ ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_Price')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_Price', N'قیمت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_PriceFormula')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_PriceFormula', N'فرمول قیمت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_PriceProduct')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_PriceProduct', N'قیمت محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_Recommend')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_Recommend', N'محصولات پیشنهادی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_TotalWeight')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_TotalWeight', N'وزن کل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_Type', N'جنس');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_TypeStone')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_TypeStone', N'جنس سنگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_WeightStone')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_WeightStone', N'وزن سنگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gold_Recommend_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gold_Recommend_Title', N'مشتریان دیگر انتخاب کرده اند..');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_Register')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_Register', N'گزارش تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_NotEnterCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_NotEnterCount', N'تعداد وارد نشده به انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_ErrorRegister')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_ErrorRegister', N'گزارش خطاهای رجیستر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_ErrorRegister_Title1')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_ErrorRegister_Title1', N'کالاهای رجیستر شده که از بسته بندی خارج نشده اند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_ErrorRegister_Title2')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_ErrorRegister_Title2', N'کالاهای رجیسترشده که تگ ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExportPdf')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExportPdf', N'دریافت PDF');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_MissionCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_MissionCode', N'کدماموریت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_MissionDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_MissionDate', N'زمان ماموریت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_MissionStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_MissionStatus', N'وضعیت ماموریت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_MissionType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_MissionType', N'نوع ماموریت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_Mission')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_Mission', N'گزارش ماموریت لیفتراک');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Claim_AddTo_User')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Claim_AddTo_User', N'مدیریت دسترسی های کاربر "{0}"');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Claim_ListViews')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Claim_ListViews', N'صفحات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Account_UserClaim')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Account_UserClaim', N'مدیریت دسترسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Home_Index')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Home_Index', N'خانه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_HomePage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_HomePage', N'صفحه اصلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_Access_NotChangable')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_Access_NotChangable', N'دسترسی های مربوط به کاربر/سطح دسترسی موردنظر، قابل تغییر نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductRevokeDateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductRevokeDateTime', N'تاریخ ابطال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_UserRevoke')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_UserRevoke', N'کاربر ابطال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_Revoke')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_Revoke', N'گزارش تگ های ابطال شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_Back')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_Back', N'گزارش کالاهای بازگشتی از انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExitDesc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExitDesc', N'توضیحات خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_InventoryConflicts')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_InventoryConflicts', N'گزارش مغایرت های انبارگردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GetData_Acounting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GetData_Acounting', N'بارگذاری اطلاعات از سیستم حسابداری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Count_Accounting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Count_Accounting', N'موجودی حسابداری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Count_Conflicts_Shown')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Count_Conflicts_Shown', N'فقط نمایش مغایرت ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Placement')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Placement', N'جانمایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SearchFilter_OperationInfo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SearchFilter_OperationInfo', N'فیلترهای اطلاعات عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SearchFilter_ProductInfo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SearchFilter_ProductInfo', N'فیلترهای اطلاعات کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SumCount_Conflicts')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SumCount_Conflicts', N'مقدار مغایرت Rfid');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Conflict_Accounting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Conflict_Accounting', N'مغایرت حسابداری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Count', N'تعداد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Exit_Tab2')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Exit_Tab2', N'تجمعی بر روی کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Exit_Tab3')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Exit_Tab3', N'گزارش ریز کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search_Accounting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search_Accounting', N'جستجو در سیستم حسابداری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search_InLocal')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search_InLocal', N'جستجو در محصولات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_SelectProduct')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_SelectProduct', N'ابتدا یک محصول را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Add_Image')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Add_Image', N'تصویر کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Add_Properties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Add_Properties', N'مشخصات کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Add_Tags_Properties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Add_Tags_Properties', N'مشخصات محموله کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Security_Tags')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Security_Tags', N'کالاهای شناسایی شده توسط RFID');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Security_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Security_Title', N'ثبت خروج ماشین حمل (خروج محصول)');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DownloadSample')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DownloadSample', N'دریافت نمونه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_RegisteredProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_RegisteredProductCode', N'محصول مورد نظر دارای تگ رجیستر شده است و الوکیشن حذف آن وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Upload')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Upload', N'بارگذاری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_UploadExcel')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_UploadExcel', N'بارگذاری اکسل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Choose')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Choose', N'ابتدا یک مورد را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report', N'گزارش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Destination')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Destination', N'مقصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Capacity')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Capacity', N'مورد انتخاب شده دارای ظرفیت است. ابتدا ظرفیت را خالی کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_HaveChild')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_HaveChild', N'مورد انتخاب شده دارای موارد زیرمجموعه است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Views_Account_Access')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Views_Account_Access', N'دسترسی های کاربران');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_User_Image')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_User_Image', N'تصویر کاربری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Add_TechnicalInfo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Add_TechnicalInfo', N'اطلاعات فنی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Add_TechnicalInfoUpload')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Add_TechnicalInfoUpload', N'بارگذاری اکسل اطلاعات فنی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Add_AddProductUpload')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Add_AddProductUpload', N'افزودن کالا با اکسل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect', N'بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Elements')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Elements', N'تعریف المان های بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_ElementType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_ElementType', N'نوع المان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Max')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Max', N'حداکثر مقدار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Min')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Min', N'حداقل مقدار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Prevent')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Prevent', N'از عبور گیت جلوگیری شود؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Type_Checkbox')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Type_Checkbox', N'چندگزینه ای');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Type_Combobox')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Type_Combobox', N'تک گزینه ای');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Type_Int')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Type_Int', N'مقدار عددی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Type_String')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Type_String', N'مقدار حرف و عدد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Default_value')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Default_value', N'مقدار پیش فرض');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Options')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Options', N'گزینه ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Option_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Option_Add', N'افزودن گزینه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductType_Choose')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductType_Choose', N'انتخاب نوع کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Options_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Options_Add', N'هیچ گزینه ای افزوده نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_ProductType_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_ProductType_Add', N'کد کالای مورد نظر انتخاب نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_ElementType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_ElementType', N'یک نوع المان را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Unverified')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Unverified', N'مردود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Verified')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Verified', N'تائید شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Report')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Report', N'گزارش بازرسی - عملیات ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Result')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Result', N'نتیجه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Default_Value_Question')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Default_Value_Question', N'آیا مورد پیش فرض است؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Warehouse_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Warehouse_Code', N'کد انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Warehouse_Inventory_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Warehouse_Inventory_Type', N'نوع موجودی انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Warehouse_Operational_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Warehouse_Operational_Type', N'نوع عملیاتی انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_Warehouse_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_Warehouse_Title', N'عنوان انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Loading_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Loading_Warehouse', N'انبار بارگیری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Material_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Material_Warehouse', N'انبار مواد اولیه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Physical')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Physical', N'حقیقی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Production_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Production_Warehouse', N'انبار تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Warehouse', N'انبار محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Code_Uniqueness')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Code_Uniqueness', N'این کد قبلا ثبت شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_EmptyError')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_EmptyError', N'نمی تواند خالی باشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Inventory_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Inventory_Type', N'نوع موجودی را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Operational_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Operational_Type', N'نوع عملیاتی انبار را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Virtual')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Virtual', N'مجازی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse_Add', N'افزودن انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Waste_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Waste_Warehouse', N'انبار ضایعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Default')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Default', N'پیش فرض');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Delete_Default_Forbidden')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Delete_Default_Forbidden', N'الوکیشن حذف پیش فرض وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Edit_Default_Forbidden')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Edit_Default_Forbidden', N'الوکیشن ویرایش پیش فرض وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Date')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Date', N'تاریخ بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Place_PlaceType_Direct')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Place_PlaceType_Direct', N'ثبت مستقیم');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Place_PlaceType_Mission')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Place_PlaceType_Mission', N'ماموریت جابه جایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Place_PlaceProductChooseType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Place_PlaceProductChooseType', N'نوع انتخاب کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Place_PlaceProductChooseType_CodeAndCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Place_PlaceProductChooseType_CodeAndCount', N'انتخاب کالا از طریق مشخص کردن کدکالا و تعداد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Place_PlaceProductChooseType_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Place_PlaceProductChooseType_Serial', N'انتخاب کالا از طریق مشخص کردن سریال کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Place_PlaceType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Place_PlaceType', N'نوع ثبت جابه جایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Place_ProductChoose_Direct')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Place_ProductChoose_Direct', N'انتخاب کالا در حالت جابه جایی مستقیم تنها با انتخاب سریال الوکیشن پذیر است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Count_InDestination')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Count_InDestination', N'موجودی در مقصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Count_InFrom')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Count_InFrom', N'موجودی در مبدا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Capacity_Destination')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Capacity_Destination', N'لوکیشن مقصد گنجایش مقدار درخواستی را ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Capacity_From')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Capacity_From', N'در لوکیشن مبدا، از محصول درخواستی، موجودی درخواستی وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Capacity_NotFound')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Capacity_NotFound', N'محصول درخواستی موجودی درخواستی را ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_Duplicated_Serials')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_Duplicated_Serials', N'سریال های زیر تکراری است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_Notfound_Serials')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_Notfound_Serials', N'سریال های زیر یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Field_ProductType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Field_ProductType', N'نوع کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Login_Timeout')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Login_Timeout', N'اعتبار لاگین شما اتمام یافته است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Compeletable_StoreLocations')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Compeletable_StoreLocations', N'موجودی انبار "{0}" لوکیشن "{1}"');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Compeletable_StoreLocations_ProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Compeletable_StoreLocations_ProductCode', N'موجودی انبار "{0}" لوکیشن "{1}" کدکالا "{2}"');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Compeletable_StoreLocations_ProductCode_Date')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Compeletable_StoreLocations_ProductCode_Date', N'موجودی انبار "{0}" لوکیشن "{1}" کدکالا "{2}" تاریخ "{3}"');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_History')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_History', N'تاریخچه بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Address')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Address', N'آدرس');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_Number')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_Number', N'شماره لیفتراک');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_TruckNumber_Required')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_TruckNumber_Required', N'وارد کردن شماره لیفتراک الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_Confirm_First')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_Confirm_First', N'آیا قرارگرفتن محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Truck_Confirm_Second')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Truck_Confirm_Second', N'را در لوکیشن زیر تائید می کنید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Number_Truck')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Number_Truck', N'لیفتراک شماره ی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Cargo_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Cargo_Status', N'ابتدا وضعیت محموله فعلی را تعیین کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gate_TruckCross')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gate_TruckCross', N'ثبت تردد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Steps_Enter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Steps_Enter', N'ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Steps_Exit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Steps_Exit', N'خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Steps_Present')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Steps_Present', N'پذیرش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_NationalCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_NationalCode', N'کدملی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Present_Cause')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Present_Cause', N'علت مراجعه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Turn')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Turn', N'نوبت پذیرش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_TypeTruck')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_TypeTruck', N'نوع ماشین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Validation_Step')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Validation_Step', N'اطلاعات مرحله {0} هنوز ثبت نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_NotFound')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_NotFound', N'اطلاعات مورد نظر یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Present_Date')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Present_Date', N'تاریخ پذیرش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Enter_WeightTonage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Enter_WeightTonage', N'وزن ورود(تن)');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Enter_DateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Enter_DateTime', N'تاریخ و ساعت ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_DateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_DateTime', N'تاریخ و ساعت خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_WeightTonage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_WeightTonage', N'وزن خروج(تن)');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_PresentCause_Guest')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_PresentCause_Guest', N'مهمان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_PresentCause_RecieveMaterial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_PresentCause_RecieveMaterial', N'تحویل مواد اولیه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_PresentCause_SendProductToCustomer')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_PresentCause_SendProductToCustomer', N'ارسال کالا به مشتری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_PresentCause_SendProductToShip')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_PresentCause_SendProductToShip', N'ارسال کالا به باربری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Company')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Company', N'شرکت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Stringlength_Max')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Stringlength_Max', N'طول فیلد {0} باید حداکثر {1} باشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Download')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Download', N'دریافت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gallery_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gallery_Title', N'گالری اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Upload_Docs')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Upload_Docs', N'بارگذاری اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Upload_MultiMedia')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Upload_MultiMedia', N'بارگذاری چندرسانه ای');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Upload_Photos')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Upload_Photos', N'بارگذاری تصاویر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_LicenseCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_LicenseCode', N'شماره گواهینامه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_DocUpload')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_DocUpload', N'بارگذاری اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Enter_Date')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Enter_Date', N'تاریخ ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Enter_OtherTags')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Enter_OtherTags', N'سایر تگ های ماشین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Enter_Time')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Enter_Time', N'ساعت ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_Date')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_Date', N'تاریخ خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_Time')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_Time', N'ساعت خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Present_Time')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Present_Time', N'ساعت پذیرش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Present_User')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Present_User', N'کاربر ثبت مراجعه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DateTime', N'تاریخ و ساعت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Add_Dynamic_fields')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Add_Dynamic_fields', N'تعریف فیلدهای اطلاعاتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Dynamic_Field')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Dynamic_Field', N'فیلد داینامیک');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_OperationInfo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_OperationInfo', N'اطلاعات عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Field')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Field', N'اطلاعات اقلام کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RelatedTitles')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RelatedTitles', N'عناوین مرتبط');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Systematic_Element')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Systematic_Element', N'المان سیستمی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Header_Key')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Header_Key', N'کلید سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_FileName_IsExist')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_FileName_IsExist', N'یک فایل با این نام قبلا آپلود شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Upload_DynamicExce')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Upload_DynamicExce', N'بارگذاری اکسل داینامیک');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RelatedTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RelatedTitle', N'عنوان مرتبط');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Upload_Operational_Docs')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Upload_Operational_Docs', N'بارگذاری اسناد عملیات های انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Iran')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Iran', N'ایران');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_ChoosePlaque')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_ChoosePlaque', N'از این راننده چند ماشین ثبت شده، لطفا یک مورد را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ContractStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ContractStatus', N'سند تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Brand')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Brand', N'برند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Group')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Group', N'گروه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Sales_Permit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Sales_Permit', N'مجوز فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Type', N'نوع سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Document_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Document_Type', N'یک نوع سند انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_CascadeDelete')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_CascadeDelete', N'این مورد دارای چند اطلاعات مرتبط است و قابل حذف نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Brand')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Brand', N'افزودن برند کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Group')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Group', N'افزودن گروه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Excel_Format')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Excel_Format', N'اکسل وارد شده در فرمت مناسب نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductBrand')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductBrand', N'برند کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductGroup')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductGroup', N'گروه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_AddProducts_UploadAndPreview')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_AddProducts_UploadAndPreview', N'بارگذاری فایل و پیش نمایش اطلاعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_AnyData')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_AnyData', N'اطلاعاتی برای بارگذاری وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_EnterAction_Tab1')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_EnterAction_Tab1', N'گزارش عملیات های ورود انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_EnterAction')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_EnterAction', N'گزارش عملیات های ورود کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Show_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Show_Count', N'تعداد نمایش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_TruckCross')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_TruckCross', N'گزارش تردد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Present_DateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Present_DateTime', N'تاریخ و ساعت پذیرش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_DescTruck')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_DescTruck', N'شرح ماشین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Present_Desc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Present_Desc', N'شرح مراجعه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Enter_User')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Enter_User', N'کاربر ثبت ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_User')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_User', N'کاربر ثبت خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_LastResult')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_LastResult', N'آخرین بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Enter_Doc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Enter_Doc', N'اسناد ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_Doc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_Doc', N'اسناد خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Present_CarDoc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Present_CarDoc', N'اسناد ماشین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Present_DirverDoc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Present_DirverDoc', N'اسناد راننده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Present_Doc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Present_Doc', N'اسناد پذیرش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Products')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Products', N'محموله ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GateCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GateCode', N'کد گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_UHF_Log_Id')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_UHF_Log_Id', N'کد عملیات شناسایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Handheld')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Handheld', N'هندهلد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Kiosk')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Kiosk', N'کیوسک');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_Device')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_Device', N'دستگاه رجیستر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_Inventory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_Inventory', N'گزارش عملیات انبارگردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RegisterDevice')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RegisterDevice', N'دستگاه رجیستر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Add_Product_Size')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Add_Product_Size', N'افزودن سایز کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Qc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Qc', N'افزودن درجه کیفیت کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Product_ApiUnique')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Product_ApiUnique', N'اطلاعات وارد شده منحصر به فرد نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Qc_Validation_Choose')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Qc_Validation_Choose', N'لطفا حداقل یک درجه کیفی را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Freeze_Tag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Freeze_Tag', N'ثبت و رفع فریز محصولات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Status_Freezed')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Status_Freezed', N'فریز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Status_Not_Freezed')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Status_Not_Freezed', N'عدم فریز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Freeze_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Freeze_Status', N'وضعیت فریز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_From_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_From_Serial', N'از سریال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_To_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_To_Serial', N'تا سریال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Serials_Add')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Serials_Add', N'هیچ سریالی افزوده نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Count_Conflicts')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Count_Conflicts', N'تعداد مغایرت Rfid');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SumCount_Rfid')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SumCount_Rfid', N'موجودی Rfid(متراژ)');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Choose_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Choose_Serial', N'انتخاب محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Metrage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Metrage', N'متراژ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_And_Freeze')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_And_Freeze', N'بازرسی و فریز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Freeze')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Freeze', N'گزارش عملیات های فریز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Line_Code_Salon')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Line_Code_Salon', N'کد سالن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Line_Title_Salon')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Line_Title_Salon', N'عنوان سالن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PackValue')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PackValue', N'مقدار محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductSecondValue')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductSecondValue', N'مقدار واحد دوم در محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SeconValueInPack')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SeconValueInPack', N'تعداد واحد دوم در محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_ReportProduct')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_ReportProduct', N'گزارشی بازرسی - طرح و درجه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Accept_Inspect_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Accept_Inspect_Count', N'تعداد تأیید شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Accept_Inspect_Value')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Accept_Inspect_Value', N'مقدار تأیید شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Reject_Inspect_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Reject_Inspect_Count', N'تعداد تأیید نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Reject_Inspect_Value')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Reject_Inspect_Value', N'مقدار تأیید نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Verify_Percent')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Verify_Percent', N' تائید شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Setting_ApiSync')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Setting_ApiSync', N'ارسال عملیات جابجایی به API');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Basic_Document')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Basic_Document', N'سند مبنا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Update_ProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Update_ProductCode', N'اصلاح کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Required_ProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Required_ProductCode', N'وارد کردن کد کالا الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Aggregate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Aggregate', N'سند تجمیعی جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Count', N'تعداد اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Item_Sum')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Item_Sum', N'مقدار اقلام');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Item_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Item_Count', N'تعداد اقلام');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Properties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Properties', N'مشخصات سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Aggregate_Suggests')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Aggregate_Suggests', N'پیشنهاد تجمیع اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Add_Document_Aggregate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Add_Document_Aggregate', N'افزودن تجمیع جدید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_SingleDoc_Remove')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_SingleDoc_Remove', N'الوکیشن حذف تک سند وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_Notfound_DocKey')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_Notfound_DocKey', N'کد سند وارد شده یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocKey')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocKey', N'کد سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search_Doc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search_Doc', N'جستجوی سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Approve_Aggregate_Docs')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Approve_Aggregate_Docs', N'تائید تجمیع اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Aggregate_CheckBeforeApprove')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Aggregate_CheckBeforeApprove', N'آیا از تجمیع اسناد مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Print_Aggregated_Document')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Print_Aggregated_Document', N'آیا مایل به چاپ اطلاعات سند هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Aggregatable')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Aggregatable', N'قابل تجمیع');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Aggregated')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Aggregated', N'تجمیع شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Aggregate_SingleDoc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Aggregate_SingleDoc', N'انتخاب حداقل دو سند برای تجمیع الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Item_Properties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Item_Properties', N'مشخصات اقلام');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Security_Index_Doc_required')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Security_Index_Doc_required', N'الزام کنترل کالاها با سند عطف');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Security_Index_Doc_required')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Security_Index_Doc_required', N'جابجایی از طریق گیت و سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Alert_MovementAction_Submit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Alert_MovementAction_Submit', N'این عملیات با کد جابجایی : {0} ثبت شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Required_OperationCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Required_OperationCode', N'اطلاعات کد عملیات را وارد کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Doc_Code_Required')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Doc_Code_Required', N'اطلاعات کد سند را وارد کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Prevent_On_Errors')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Prevent_On_Errors', N'لطفا خطا ها را رفع و دوباره امتحان کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Doc_Cus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Doc_Cus', N'اطلاعات سند مجوز خروج کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Gate_Required')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Gate_Required', N'ابتدا کد گیت را مشخص کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_TruckCross_Property_Required')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_TruckCross_Property_Required', N'مشخصات ماشین حمل خالی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Property')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Property', N'مشخصات ماشین حمل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Status_Accept')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Status_Accept', N'بازرسی تایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Status_Failed')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Status_Failed', N'بازرسی مردود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Status_Not')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Status_Not', N'بازرسی نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Status', N'وضعیت بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Account_Profile')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Account_Profile', N'پروفایل کاربری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Password_New')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Password_New', N'کلمه عبور جدید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RePassword')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RePassword', N'تکرار کلمه عبور');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RePassword_New')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RePassword_New', N'تکرار کلمه عبور جدید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Equals')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Equals', N'مقادیر فیلد {0} و {1} برابر نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Stringlength_Min')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Stringlength_Min', N'طول فیلد {0} باید حداقل {1} باشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Notif_EventBase')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Notif_EventBase', N'رخداد محور');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Notif_ScheduleBase')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Notif_ScheduleBase', N'برنامه محور');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Event_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Event_Type', N'نوع رخداد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_Title', N'عنوان اطلاع رسانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Element')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Element', N'المان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_Plan_Properties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_Plan_Properties', N'مشخصات برنامه اطلاع رسانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Contact_Duplicate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Contact_Duplicate', N'مشخصات تماس قبلا به لیست اضافه شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Contact_Required')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Contact_Required', N'لطفا مشخصات تماس را وارد کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_Elements_Contacts')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_Elements_Contacts', N'المان های مخاطبین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Elements')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Elements', N'المان ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_Contacts_Props')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_Contacts_Props', N'مشخصات ارسال و مخاطبین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif_Type_Props')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif_Type_Props', N'مشخصات نوع اطلاع رسانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Next_Operation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Next_Operation', N'عملیات بعدی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Previous_Operation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Previous_Operation', N'عملیات قبلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gate_Operations')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gate_Operations', N'عملیات های گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_Notfound_Operation_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_Notfound_Operation_Code', N'کد عملیات یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Ope')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Ope', N'سند عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Register')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Register', N'سند تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_OldSerial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_OldSerial', N'سریال قبلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Manage_Docs')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Manage_Docs', N'مدیریت اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Choosed')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Choosed', N'انتخاب شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Approve')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Approve', N'تایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Customer_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Customer_Code', N'کد مشتری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Customer_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Customer_Title', N'عنوان مشتری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Item_Variety')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Item_Variety', N'تنوع اقلام');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Choose_One_Document')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Choose_One_Document', N'انتخاب حداقل یک سند الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Cartable')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Cartable', N'کارتابل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Cartable_Approve_Final')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Cartable_Approve_Final', N'تأیید نهایی سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Cartable_Approve_First')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Cartable_Approve_First', N'تأیید اولیه سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Cartable_Collect_End')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Cartable_Collect_End', N'ثبت اتمام جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Cartable_Collect_Start')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Cartable_Collect_Start', N'ثبت شروع جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Cartable_Submit_Send_Doc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Cartable_Submit_Send_Doc', N'ثبت مستندات ارسال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Docs')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Docs', N'گزارش اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Collecting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Collecting', N'در حال جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Loading')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Loading', N'در حال بارگیری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Sended')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Sended', N'ارسال شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Wating_Approve_Final')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Wating_Approve_Final', N'در انتظار تایید نهایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Wating_Approve_First')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Wating_Approve_First', N'در انتظار تایید اول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Wating_Collect')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Wating_Collect', N'در انتظار جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Wating_Load')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Wating_Load', N'در انتظار بارگیری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Wating_Send')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Wating_Send', N'در انتظار ارسال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Get_Send_Doc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Get_Send_Doc', N'دریافت مستندات ارسال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Revoke')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Revoke', N'باطل شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_HaveRows')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_HaveRows', N'ابتدا ردیف های اطلاعاتی را حذف کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_International_Plaque')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_International_Plaque', N'پلاک بین المللی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Customer')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Customer', N'فرستنده / گیرنده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Enter_Acceptor')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Enter_Acceptor', N'شخص مراجعه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Enter_AcceptPlace')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Enter_AcceptPlace', N'محل مراجعه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_CargoOwnerName')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_CargoOwnerName', N'نام صاحب بار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_CargoOwnerPhone')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_CargoOwnerPhone', N'شماره تماس صاحب بار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_DeliveryAddress')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_DeliveryAddress', N'آدرس تحویل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_PaymentType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_PaymentType', N'نوع پرداخت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_PureWeightCargo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_PureWeightCargo', N'وزن خالص محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_ShipmentCost')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_ShipmentCost', N'کرایه حمل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_TotalCost')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_TotalCost', N'مبلغ نهایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_UnitPrice')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_UnitPrice', N'قیمت واحد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_WeightBridgeReceiptNumber')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_WeightBridgeReceiptNumber', N'شماره قبض باسکول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Operation_Destination')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Operation_Destination', N'مقصد عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Operation_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Operation_Type', N'نوع عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Shipment')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Shipment', N'باربری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Shipment_Number')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Shipment_Number', N'شماره بارنامه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Discharge_Place')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Discharge_Place', N'محل تخلیه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Loading_Place')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Loading_Place', N'محل بارگیری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Payment_ByCompany')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Payment_ByCompany', N'با شرکت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Payment_ByReciever')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Payment_ByReciever', N'با گیرنده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Payment_BySender')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Payment_BySender', N'با فرستنده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Entered')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Entered', N'وارد شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exited')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exited', N'خارج شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_NotExitedCrosses')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_NotExitedCrosses', N'ماشین های خارج نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Presented')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Presented', N'پذیرش شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Config')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Config', N'اطلاعات پایه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Info_Person')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Info_Person', N'اطلاعات شخص');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Info_Present')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Info_Present', N'اطلاعات پذیرش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Info_Truck')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Info_Truck', N'اطلاعات ماشین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Security')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Security', N'حراست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gate_Product_Detection')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gate_Product_Detection', N'شناسایی کالا از طریق گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Rfid')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Rfid', N'RFID');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_NameAndFamily')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_NameAndFamily', N'نام و نام خانوادگی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Phone_Number')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Phone_Number', N'شماره تماس');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PassportCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PassportCode', N'شماره گذرنامه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_Distance')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_Distance', N'مسافت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_InspectElement_Row')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_InspectElement_Row', N'ترتیب');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ActionType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ActionType', N'نوع عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ApiSync_Correct')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ApiSync_Correct', N'اصلاح اطلاعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ApiSync_Send')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ApiSync_Send', N'تأیید و ارسال عملیات به API');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Present_Causes')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Present_Causes', N'علل مراجعه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Filters')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Filters', N'فیلترها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_ChooseOneFieldRequired')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_ChooseOneFieldRequired', N'انتخاب حداقل یکی از دو فیلد {0} و {1} الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_ExitAction_Tab1')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_ExitAction_Tab1', N'گزارش عملیات های خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_ExitAction_Tab2')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_ExitAction_Tab2', N'تجمعی بر روی کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_ExitAction_Tab3')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_ExitAction_Tab3', N'گزارش ریز کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_Report_ExitAction')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_Report_ExitAction', N'گزارش عملیات های خروج کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_From_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_From_Warehouse', N'از انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_To_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_To_Warehouse', N'به انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Description')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Description', N'شرح کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_Delete')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_Delete', N'از حذف اطلاعات مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_EnterAgg')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_EnterAgg', N'گزارش تجمعی ورود کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_FieldType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_FieldType', N'نوع فیلد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Filters_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Filters_Type', N'انتخاب فیلتر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Filters_AddType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Filters_AddType', N'عملگر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Filters_AddType_And')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Filters_AddType_And', N'AND (&&)');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Filters_AddType_Or')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Filters_AddType_Or', N'OR (||)');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Filters_Results')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Filters_Results', N'نتایج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Filters_Dynamic')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Filters_Dynamic', N'فیلترهای داینامیک');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Filters_TechnicalInfo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Filters_TechnicalInfo', N'فیلترهای اطلاعات فنی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ActionType_HasChosen_Validation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ActionType_HasChosen_Validation', N'یک نوع عملیات مشخص شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ActionType_OnlyOne_Validation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ActionType_OnlyOne_Validation', N'اجازه ثبت هم زمان دو نوع عملیات وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Movement_Property')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Movement_Property', N'مشخصات جابجایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Average')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Average', N'مقدار میانگین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Max_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Max_Count', N'بیشترین مقدار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Min_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Min_Count', N'کمترین مقدار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Brand_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Brand_Code', N'کد برند کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Brand_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Brand_Title', N'عنوان برند کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Columns')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Columns', N'ستون ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Columns_Calculating')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Columns_Calculating', N'ستون های محاسباتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Columns_Data')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Columns_Data', N'ستون های اطلاعاتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GregorianDate_Full')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GregorianDate_Full', N'تاریخ میلادی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GregorianDate_Month')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GregorianDate_Month', N'ماه میلادی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GregorianDate_Week')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GregorianDate_Week', N'هفته میلادی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GregorianDate_Year')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GregorianDate_Year', N'سال میلادی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Group_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Group_Code', N'کد گروه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Group_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Group_Title', N'عنوان گروه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PersianDate_Full')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PersianDate_Full', N'تاریخ شمسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PersianDate_Month')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PersianDate_Month', N'ماه شمسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PersianDate_Week')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PersianDate_Week', N'هفته شمسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PersianDate_Year')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PersianDate_Year', N'سال شمسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductType_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductType_Code', N'کد نوع کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductType_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductType_Title', N'عنوان نوع کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Qc_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Qc_Code', N'کد درجه کیفیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Qc_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Qc_Title', N'عنوان درجه کیفیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Size_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Size_Code', N'کد سایز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Size_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Size_Title', N'عنوان سایز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Dynamic_Column')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Dynamic_Column', N'ابتدا یک ستون اطلاعاتی انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Product_Notfound')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Product_Notfound', N'اطلاعات کالاهای ماشین حمل یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_MovementActionId_Submited')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_MovementActionId_Submited', N'این کد عملیات قبلا ثبت شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Destination_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Destination_Warehouse', N'انبار مقصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Source_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Source_Warehouse', N'انبار مبدا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chosen_Serials')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chosen_Serials', N'سریال های انتخاب شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_HandHeld_Operations')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_HandHeld_Operations', N'عملیات های هندهلد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_HandHeld_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_HandHeld_Code', N'کد هندهلد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_HandHeld_Operation_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_HandHeld_Operation_Code', N'کد عملیات هندهلد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Barcode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Barcode', N'بارکد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Barcode_Reader')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Barcode_Reader', N'بارکدخوان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Detection_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Detection_Type', N'نوع شناسایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gate', N'گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Columns_Pivot')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Columns_Pivot', N'ستون های Pivot');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_DynamicReport_Register')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_DynamicReport_Register', N'گزارش‌ساز تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Dashboard')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Dashboard', N'داشبورد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Freeze')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Freeze', N'عملیات فریز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notif')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notif', N'اطلاع رسانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse_Actions')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse_Actions', N'عملیات های انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse_Collect')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse_Collect', N'جمع آوری کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse_Enter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse_Enter', N'گزارشات ورود کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse_Inventory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse_Inventory', N'گزارشات موجودی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse_Order')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse_Order', N'جانمایی کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse_Reports')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse_Reports', N'گزارشات انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse_Place')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse_Place', N'جانمایی کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Revoke')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Revoke', N'عملیات ابطال تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Revoke')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Revoke', N'ابطال تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Revoke_Confirm')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Revoke_Confirm', N'آیا ابطال تگ با سریال {0} را تائید می کنید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_MovementAction')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_MovementAction', N'عملیات جابجایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_ApiSettings')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_ApiSettings', N'تنظیمات اتصال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Software')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Software', N'تنظیمات نرم افزاری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_ReportDynamic')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_ReportDynamic', N'گزارش‌ساز بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Date_Max')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Date_Max', N'آخرین تاریخ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Date_Min')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Date_Min', N'اولین تاریخ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Time')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Time', N'ساعت بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_User')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_User', N'کاربر بازرس');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_User')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_User', N'کاربر رجیستر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Location_SecControl')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Location_SecControl', N'کنترل و خروج کالا از درب حراست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Location_SecExit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Location_SecExit', N'خروج کالا از درب حراست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Expiration_Warranty')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Expiration_Warranty', N'انقضا-گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Movement')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Movement', N'جابجایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Tag_Cargo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Tag_Cargo', N'محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Result_Reported_By_Inspector')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Result_Reported_By_Inspector', N'نتایج گزارش شده توسط بازرس');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportFormat')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportFormat', N'فرمت گزارش‌سازی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportFormat_Save')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportFormat_Save', N'ثبت فرمت گزارش‌سازی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportFormat_Title_Warning')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportFormat_Title_Warning', N'ابتدا عنوان موردنظر برای فرمت را وارد کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Desc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Desc', N'شرح سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_ExcelExtractionException')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_ExcelExtractionException', N'استخراج اطلاعات از اکسل با مشکل مواجه شد، لطفا فایل را بررسی کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_MethodNotFoundException')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_MethodNotFoundException', N'متد با نام ''{0}'' در کلاس ''{1}'' یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_SerialsNotFound')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_SerialsNotFound', N'سریال های مشخص شده یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_SqlException')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_SqlException', N'خطا با کد {0} در اجرای کوئری sql server بوجود آمده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_SqliteException')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_SqliteException', N'خطایی در اجرای کوئری sqlite بوجود آمده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_TokenInvalid')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_TokenInvalid', N'توکن کاربری معتبر نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_TokenRequiredException')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_TokenRequiredException', N'توکن یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_Unexpected')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_Unexpected', N'در انجام عملیات مشکلی بوجود آمده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_UserNotFoundException')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_UserNotFoundException', N'کاربر با مشخصات ارسال شده یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Message_Token')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Message_Token', N'برای ارتباط نیاز به ارسال توکن هست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspect_Elements_Name')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspect_Elements_Name', N'المان های بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Info')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Info', N'اطلاعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Products')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Products', N'محموله ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Revoke_DocumentStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Revoke_DocumentStatus', N'ابطال وضعیت سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Cartable_Submited_Document')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Cartable_Submited_Document', N'تایید سند قبلا انجام شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Revoke_Word')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Revoke_Word', N'ابطال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus', N'وضعیت سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_DocumentStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_DocumentStatus', N'یک وضعیت سند انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Get_Aggregate_Suggests')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Get_Aggregate_Suggests', N'دریافت پیشنهاد تجمیع');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Get_Aggregated_Documents')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Get_Aggregated_Documents', N'نمایش اسناد تجمیع شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_AggSuggest_Recieved')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_AggSuggest_Recieved', N'پیشنهادات یک بار دریافت شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_GetAggSuggest_First')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_GetAggSuggest_First', N'ابتدا پیشنهادات تجمیع را دریافت کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_OtherInformation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_OtherInformation', N'سایر اطلاعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Android')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Android', N'اندروید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Web')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Web', N'وب');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Position_Collect_SumValueRequestDesc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Position_Collect_SumValueRequestDesc', N'شرح تعداد درخواستی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Location_Locate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Location_Locate', N'جانمایی محموله در انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Wating_Approve_Only')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Wating_Approve_Only', N'فقط نمایش اسناد تایید نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Report_Freeze_Products')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Report_Freeze_Products', N'گزارش کالاهای فریز شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Freeze_Cause')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Freeze_Cause', N'علت فریز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Freeze_User')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Freeze_User', N'کاربر فریز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Insert_Batch')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Insert_Batch', N'بارگذاری تجمعی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_ExpireStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_ExpireStatus', N'وضعیت انقضا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_GuaranteeStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_GuaranteeStatus', N'وضعیت گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_Type_Enter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_Type_Enter', N'لحظه ورود به انبار محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_Type_Exit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_Type_Exit', N'لحظه خروج از انبار محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_Type_Inspect')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_Type_Inspect', N'لحظه تأیید بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_Type_Factory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_Type_Factory', N'لحظه خروج از کارخانه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_ExpireAndGuarantee')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_ExpireAndGuarantee', N'تعیین انقضاء و گارانتی محصولات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_ExpireDays')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_ExpireDays', N'مدت اعتبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Print_Action')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Print_Action', N'آیا مایل به چاپ اطلاعات عملیات هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross', N'ماشین حمل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee', N'انقضاء و گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Empty_Completable')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Empty_Completable', N'اطلاعات فیلد {0} را پر کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_Action')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_Action', N'گزارش عملیات ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_Aggregate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_Aggregate', N'گزارش سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_ApiSync')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_ApiSync', N'ارسال عملیات جابه جایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_Collect')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_Collect', N'برنامه جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_Enter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_Enter', N'گزارش ورود کالا به انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_EnterAction')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_EnterAction', N'گزارش عملیات های ورود انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_EnterActionAgg')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_EnterActionAgg', N'گزارش تجمعی عملیات های ورود کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_ExitAction')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_ExitAction', N'گزارش عملیات خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_Inventory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_Inventory', N'گزارش انبارگردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_InventoryDetails')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_InventoryDetails', N'گزارش کالاهای انبارگردانی شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_Register')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_Register', N'گزارش تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_ReportFile')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_ReportFile', N'فایل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Print_ReportTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Print_ReportTitle', N'عنوان گزارش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventory_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventory_Code', N'کد انبار‌گردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventory_DateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventory_DateTime', N'تاریخ انبارگردانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_Convert')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_Convert', N'تبدیل داده ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_ConvertJsonDocument')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_ConvertJsonDocument', N'تبدیل Json سندها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_TruckCross_SaveExit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_TruckCross_SaveExit', N'اطلاعات مرحله خروج هنوز ثبت نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Division')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Division', N'تقسیم سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Max_Value')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Max_Value', N'حداکثر مقدار مجاز {0} است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Min_Value')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Min_Value', N'حداقل مقدار مجاز {0} است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Divided_Documents')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Divided_Documents', N'اسناد تقسیم شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_New_Document_Division')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_New_Document_Division', N'تقسیم جدید سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Remain_Document')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Remain_Document', N'باقیمانده سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Remain_Document')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Remain_Document', N'باقیمانده سند نمیتواند خالی باشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Revoked')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Revoked', N'ابطال شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Revoke_TruckCross_Level')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Revoke_TruckCross_Level', N'ابطال تردد در حالت "پذیرش ثبت شده" امکان پذیر است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Revoke_TruckCross_Present')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Revoke_TruckCross_Present', N'آیا از ابطال این تردد با مشخصات زیر اطمینان دارید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Set_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Set_Status', N'مقداردهی به فیلد وضعیت ماشین حمل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Notfound')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Notfound', N'ماشین حمل یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_TruckCross_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_TruckCross_Status', N'امکان تغییر وضعیت تردد با وضعیت "ابطال شده" وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Division_Suggest')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Division_Suggest', N'پیشنهاد تقسیم سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_InventoryConflicts_OneStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_InventoryConflicts_OneStatus', N'سریال های انتخاب شده حتما باید در یک وضعیت باشند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Conflicts_Fix')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Conflicts_Fix', N'رفع مغایرت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventory_Filter_EnterExit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventory_Filter_EnterExit', N'فیلتر کالاهای وارد و خارج شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Count', N'تعداد وارد شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_SumCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_SumCount', N'مقدار وارد شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Exit_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Exit_Count', N'تعداد خارج شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Exit_SumCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Exit_SumCount', N'مقدار خارج شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GetData_Acounting_Brief')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GetData_Acounting_Brief', N'بارگذاری اکسل حسابداری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Remove_Cartable')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Remove_Cartable', N'کارتابل حذف سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gate_Alert')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gate_Alert', N'آلارم گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Alert_Desc')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Alert_Desc', N'شرح آلارم');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Sales_Info')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Sales_Info', N'اطلاعات فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Info')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Info', N'اطلاعات ماشین حمل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCrossId')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCrossId', N'کد پذیرش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PureWeight')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PureWeight', N'وزن خالص');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Divide_Document_Required')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Divide_Document_Required', N'حداقل یک سند تقسیم کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RemainDocument_Prevent_Print')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RemainDocument_Prevent_Print', N'امکان چاپ سند باقیمانده وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PlacementOrder_IgnoreMessage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PlacementOrder_IgnoreMessage', N'آیا از لغو عملیات جمع آوری اطمینان دارید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CollectOrder_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CollectOrder_Count', N'تعداد دستور جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Pallet_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Pallet_Count', N'تعداد پالت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Aggregate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Aggregate', N'تجمیع');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Collect')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Collect', N'جمع آوری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Divide')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Divide', N'تقسیم');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentItem_Weight')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentItem_Weight', N'وزن اقلام سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Approve_Question')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Approve_Question', N'آیا از تایید موارد زیر مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Check_Before_Remove')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Check_Before_Remove', N'آیا از حذف موارد زیر مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Check_Before_Revoke_Aggregate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Check_Before_Revoke_Aggregate', N'آیا از ابطال تجمیع سند مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Check_Before_Revoke_Division')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Check_Before_Revoke_Division', N'آیا از ابطال سند تقسیم شده مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Count_All_Documents')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Count_All_Documents', N'تعداد کل اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SumValue_Items')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SumValue_Items', N'جمع مقادیر اقلام');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Current_ProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Current_ProductCode', N'کد کالای فعلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_New_ProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_New_ProductCode', N'کد کالای جدید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Check_Before_UpdateProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Check_Before_UpdateProductCode', N'آیا از اصلاح کدکالا با اطلاعات زیر مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Current_ProductCode_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Current_ProductCode_Count', N'تعداد کالا با کدکالای فعلی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentItem_Volume')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentItem_Volume', N'حجم اقلام سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gate_Identify_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gate_Identify_Status', N'وضعیت شناسایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_Report')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_Report', N'گزارش سابقه تغییر وضعیت اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentEvent_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentEvent_Type', N'نوع تغییر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Description')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Description', N'توضیحات سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentEvent_DateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentEvent_DateTime', N'تاریخ و ساعت تغییر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_ImportDateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_ImportDateTime', N'تاریخ درج سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentEvent_User')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentEvent_User', N'کاربر تغییر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentEvent_Description')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentEvent_Description', N'توضیحات تغییر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_ChangeStatusBackward')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_ChangeStatusBackward', N'ابطال در کارتابل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_ChangeStatusForward')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_ChangeStatusForward', N'تایید در کارتابل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_InsertAggregate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_InsertAggregate', N'افزودن سند تجمیعی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_InsertDivide')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_InsertDivide', N'افزودن سند تقسیمی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_InsertDocument')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_InsertDocument', N'افزودن سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_RemoveAggregate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_RemoveAggregate', N'حذف سند تجمیعی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_RemoveDivide')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_RemoveDivide', N'حذف سند تقسیمی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_RemoveDocument')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_RemoveDocument', N'حذف سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_RevokeAggregate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_RevokeAggregate', N'ابطال تجمیع');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentLog_RevokeDivide')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentLog_RevokeDivide', N'ابطال تقسیم');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Documents')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Documents', N'اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Log')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Log', N'سوابق سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_AddEdit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_AddEdit', N'ثبت و ویرایش سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Check_Before_Submit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Check_Before_Submit', N'آیا از ثبت اطلاعات مطمئن هستید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Edit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Edit', N'افزودن و ویرایش اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_ActionType_AllowUpdate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_ActionType_AllowUpdate', N'از دسترسی به افزودن/ویرایش برای نوع سند مورد نظر جلوگیری شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_ActionType_Any')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_ActionType_Any', N'برای این نوع سند، هیچ فیلد اطلاعاتی تعریف نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductCountInPack')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductCountInPack', N'تعداد واحد دوم');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_ChangeTruckCross')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_ChangeTruckCross', N'تغییر ماشین حمل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Change_To_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Change_To_Status', N'تغییر به وضعیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_MinutesUntilNext')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_MinutesUntilNext', N'مدت زمان اجرا (دقیقه)');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notification_Order_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notification_Order_Type', N'نوع برنامه ارسال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Contact')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Contact', N'مخاطب');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notification_Send_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notification_Send_Status', N'وضعیت ارسال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notification_Report')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notification_Report', N'گزارش اطلاع رسانی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notification_Status_Sended')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notification_Status_Sended', N'ارسال شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Notification_Status_WatingSend')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Notification_Status_WatingSend', N'در انتظار ارسال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Create_ProductClass')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Create_ProductClass', N'تعریف طبقه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SubTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SubTitle', N'عنوان فرعی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Create_Product_SubGroup')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Create_Product_SubGroup', N'تعریف زیر گروه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SubGroup')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SubGroup', N'زیر گروه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductClass')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductClass', N'طبقه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_SubGroup')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_SubGroup', N'زیرگروه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Calculate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Calculate', N'محاسبه کرایه حمل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Aggregated_Document')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Aggregated_Document', N'سند تجمیع شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Divided_Document')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Divided_Document', N'سند تقسیم شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentEdit_Reset_First')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentEdit_Reset_First', N'برای ویرایش باید سند را به حالت اولیه بازگردانید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_DocumentEdit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_DocumentEdit', N'امکان ویرایش {0} وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Cartable_ProductGuarantee')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Cartable_ProductGuarantee', N'کارتابل وضعیت گارانتی سریال ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_NotStarted')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_NotStarted', N'شروع نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExitAction_Date')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExitAction_Date', N'تاریخ عملیات خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExitAction_DocumentCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExitAction_DocumentCode', N'کد سند عملیات خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_EndDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_EndDate', N'تاریخ پایان گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_StartDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_StartDate', N'تاریخ شروع گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Copy_Tags_To_ProductGuarantee')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Copy_Tags_To_ProductGuarantee', N'جابه جایی اطلاعات فعلی سریال ها به بخش گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Guarantee_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Guarantee_Type', N'نوع گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Data_NotFound_Print')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Data_NotFound_Print', N'اطلاعاتی برای چاپ یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Guarantee')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Guarantee', N'گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GuaranteeDuration')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GuaranteeDuration', N'مدت زمان گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RemainingDay')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RemainingDay', N'مدت روز باقی مانده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_Type_Customer')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_Type_Customer', N'لحظه کنترل اصالت کالا توسط مصرف کننده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_Type_Date')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_Type_Date', N'تاریخ مشخص');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_Type_Install')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_Type_Install', N'لحظه نصب کالا توسط مأمور نصب');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_Type_Sell')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_Type_Sell', N'لحظه ثبت فروش توسط نمایندگی فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Expire_Duration')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Expire_Duration', N'مدت انقضا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Expire_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Expire_Type', N'نوع انقضا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Guarantee_Duration')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Guarantee_Duration', N'مدت گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_UHFLog_Report')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_UHFLog_Report', N'گزارش شناسایی تگ ها بدون عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Device_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Device_Code', N'کد دستگاه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_EndDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_EndDate', N'تاریخ پایان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Expire_Start_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Expire_Start_Type', N'نوع آغاز انقضا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Guarantee_Start_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Guarantee_Start_Type', N'نوع آغاز گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_StartDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_StartDate', N'تاریخ شروع');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Expire')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Expire', N'انقضا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExpireAndGuarantee_Check')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExpireAndGuarantee_Check', N'بررسی اطلاعات انقضا و گارانتی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_City')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_City', N'شهر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Model')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Model', N'مدل کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Serial', N'سریال کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Province')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Province', N'استان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Customer')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Customer', N'مصرف کننده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Stringlength_Exact')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Stringlength_Exact', N'طول فیلد {0} باید {1} باشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Customer_GuaranteeCheck_AnnounceStatus_WithDays')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Customer_GuaranteeCheck_AnnounceStatus_WithDays', N'گارانتی کالای شما {0} بوده و تا {1} روز دیگر ({2}) اعتبار دارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Customer_GuaranteeCheck_Exist')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Customer_GuaranteeCheck_Exist', N'کالای شما نزد شرکت {0} دارای اصالت می باشد و این شرکت پیوستن شما به خانواده بزرگ مشتریان خود را مغتنم می شمارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Customer_GuaranteeCheck_NotExist')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Customer_GuaranteeCheck_NotExist', N'اطلاعات وارد شده نامعتبر می باشد، لطفا از صحت اطلاعات اطمینان پیدا کرده و مجدد اقدام نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductAuthenticity')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductAuthenticity', N'اصالت کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search_ProductModel')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search_ProductModel', N'جستجوی مدل کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Customer_GuaranteeCheck_CheckedBefore')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Customer_GuaranteeCheck_CheckedBefore', N'کالای موردنظر قبلا توسط فرد دیگری اصالت سنجی شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_AllCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_AllCount', N'مقدار کلی سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_CheckContradictions_DocHeaderRemining')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_CheckContradictions_DocHeaderRemining', N'مغایرت: جمع مقدار ردیف های کالا از باقی مانده ردیف های سند بیشتر است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Unused')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Unused', N'مقدار قابل استفاده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Used')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Used', N'مقدار استفاده شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_UploadExcel_SaleShop')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_UploadExcel_SaleShop', N'ورود اطلاعات نمایندگان با اکسل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Direction')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Direction', N'مسیر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Manager_Name')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Manager_Name', N'نام مدیر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SalesShop_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SalesShop_Code', N'کد نمایندگی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SalesShop_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SalesShop_Title', N'عنوان نمایندگی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Customer_GuaranteeCheck_DateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Customer_GuaranteeCheck_DateTime', N'تاریخ و ساعت ثبت اصالت سنجی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ProductAuthenticity_Check')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ProductAuthenticity_Check', N'بررسی اصالت سنجی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Customer_GuaranteeCheck_AnnounceStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Customer_GuaranteeCheck_AnnounceStatus', N'گارانتی کالای شما {0} است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_Exit_TotalShipingCost')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_Exit_TotalShipingCost', N'مبلغ نهایی کرایه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCross_FeeConfig')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCross_FeeConfig', N'تعریف فی کرایه حمل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Weight')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Weight', N'وزن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_UploadExcel_SalesInstaller')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_UploadExcel_SalesInstaller', N'ورود اطلاعات نصب کنندگان با اکسل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExcelSample_Install')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExcelSample_Install', N'نمونه اکسل اطلاعات نصب کنندگان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExcelSample_Shop')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExcelSample_Shop', N'نمونه اکسل اطلاعات نمایندگان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Customer_GuaranteeCheck_ActivedNow_WithDays')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Customer_GuaranteeCheck_ActivedNow_WithDays', N'گارانتی کالای شما هم اکنون فعال شد و تا {0} روز دیگر ({1}) اعتبار دارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SalesInstaller_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SalesInstaller_Code', N'کد نصب کننده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SalesInstaller_Name')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SalesInstaller_Name', N'نام نصب کننده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Serial_ExpireAndGuarantee_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Serial_ExpireAndGuarantee_Status', N'وضعیت انقضا و گارانتی سریال ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Day')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Day', N'روز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Month')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Month', N'ماه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RemainingMonth')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RemainingMonth', N'ماه باقیمانده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_RemainCheck')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_RemainCheck', N'نحوه بررسی مقدار سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Check_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Check_Type', N'نوع بررسی سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_CheckType_DocumentRemain')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_CheckType_DocumentRemain', N'کنترل با باقیمانده کل سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_CheckType_Exact')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_CheckType_Exact', N'کنترل کامل با سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_CheckType_ProductCodeRemain')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_CheckType_ProductCodeRemain', N'کنترل با باقیمانده کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Driver')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Driver', N'راننده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_View_DynamicReport_ExitAction')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_View_DynamicReport_ExitAction', N'گزارش ساز عملیات های خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Class_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Class_Code', N'کد طبقه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Class_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Class_Title', N'عنوان طبقه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_GregorianDate_Day')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_GregorianDate_Day', N'روز میلادی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Operation_DocumentCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Operation_DocumentCode', N'کد سند عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Operation_Time')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Operation_Time', N'ساعت عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_PersianDate_Day')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_PersianDate_Day', N'روز شمسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SubGroup_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SubGroup_Code', N'کد زیر گروه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SubGroup_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SubGroup_Title', N'عنوان زیر گروه کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Sum_ProductCountInPack')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Sum_ProductCountInPack', N'جمع مقدار واحد دوم');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Type1')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Type1', N'اولین نوع فرعی سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Type2')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Type2', N'دومین نوع فرعی سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Type_Validation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Type_Validation', N'مقدار {0} و {1} نمیتواند برابر باشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CreateDateTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CreateDateTime', N'تاریخ و ساعت ایجاد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_CreateUser')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_CreateUser', N'کاربر ثبت کننده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_ProductCodeNotFound')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_ProductCodeNotFound', N'کد کالا یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Station')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Station', N'ایستگاه شناسایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Station_Alert')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Station_Alert', N'آلارم ایستگاه شناسایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Convert_Create_UHFRearLogHeader')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Convert_Create_UHFRearLogHeader', N'ساخت رکورد های جدول اطلاعات عملیات شناسایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_Bar')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_Bar', N'نمودار میله ای');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_Pie')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_Pie', N'نمودار دایره ای');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ExitAction_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ExitAction_Code', N'کد عملیات خروج');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Settings_ReportLinks')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Settings_ReportLinks', N'تعریف دسترسی های گزارش سازها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Url')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Url', N'URL');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Menu')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Menu', N'منو');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportLink_ChooseCategory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportLink_ChooseCategory', N'محل افزوده شدن لینک در منو را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_ChooseCategory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_ChooseCategory', N'ابتدا یک دسته بندی در منو را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_ChooseUser')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_ChooseUser', N'ابتدا یک کاربر را انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ReportFormat_MenuLink')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ReportFormat_MenuLink', N'تعیین دسترسی به گزارش ساز برای کاربران');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ValueType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ValueType', N'نوع مقدار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Dropdown')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Dropdown', N'Dropdown');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Textbox')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Textbox', N'Textbox');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Search_Document_First')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Search_Document_First', N'لطفا ابتدا سند را جستجو کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_HeaderData')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_HeaderData', N'اطلاعات داینامیک هدر سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_ItemData')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_ItemData', N'اطلاعات داینامیک اقلام سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Choose_One_ValueOption')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Choose_One_ValueOption', N'انتخاب حداقل یک مقدار الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_Type', N'نوع چارت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_Pivot')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_Pivot', N'نمودار pivot');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Pivot_Data_NotFound')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Pivot_Data_NotFound', N'اطلاعات pivot یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chart_Line')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chart_Line', N'نمودار خطی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Shift_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Shift_Code', N'کد شیفت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Shift_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Shift_Title', N'عنوان شیفت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Aggregation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Aggregation', N'تجمعی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Degree')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Degree', N'درجه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Identification')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Identification', N'شناسایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_First')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_First', N'ابتدا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Last')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Last', N'انتها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_New')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_New', N'جدید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Offline')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Offline', N'آفلاین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Online')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Online', N'آنلاین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Pause')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Pause', N'توقف');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Question')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Question', N'سوال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register', N'ثبت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Start')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Start', N'شروع');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Today')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Today', N'امروز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warning')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warning', N'هشدار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Farvardin')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Farvardin', N'فروردين');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Ordibehesht')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Ordibehesht', N'ارديبهشت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Khordad')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Khordad', N'خرداد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Tir')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Tir', N'تير');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Mordad')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Mordad', N'مرداد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Shahrivar')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Shahrivar', N'شهريور');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Mehr')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Mehr', N'مهر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Aban')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Aban', N'آبان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Azar')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Azar', N'آذر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Dey')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Dey', N'دي');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Bahman')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Bahman', N'بهمن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Esfand')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Esfand', N'اسفند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Sunday')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Sunday', N'يکشنبه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Monday')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Monday', N'دوشنبه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Tuesday')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Tuesday', N'سه‌شنبه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Wednesday')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Wednesday', N'چهارشنبه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Thursday')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Thursday', N'پنج‌شنبه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Friday')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Friday', N'جمعه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Saturday')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Saturday', N'شنبه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Aggregation_Filter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Aggregation_Filter', N'فیلتر تجمعی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Apply_Filter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Apply_Filter', N'اعمال فیلتر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Authorization_Document_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Authorization_Document_Code', N'کد سند مجوز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Barcode_Scanner')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Barcode_Scanner', N'بارکد اسکنر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Cancel_Filter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Cancel_Filter', N'لغو فیلتر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Cargo_Properties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Cargo_Properties', N'سایر مشخصات محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Cargo_Sales_History')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Cargo_Sales_History', N'سابقه فروش محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Change_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Change_Serial', N'تغییر سریال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Chars_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Chars_Count', N'تعداد ارقام');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Check_Document_In_Movement')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Check_Document_In_Movement', N'چک سند در جابجایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Destination_Title')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Destination_Title', N'عنوان مقصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Destination_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Destination_Type', N'نوع مقصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Device_Power_Setting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Device_Power_Setting', N'تنظیمات دستگاه و پاور دستگاه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Document_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Document_Code', N'سند ثبت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Duplicate_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Duplicate_Serial', N'سریال تکراری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Edit_Product')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Edit_Product', N'ویرایش محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Move_Shipment_Value')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Move_Shipment_Value', N'انتقال مقدار محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Edit_Tag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Edit_Tag', N'ویرایش تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Product_To_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Product_To_Warehouse', N'ورود محصول به انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Tag_List')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Tag_List', N'لیست تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_In_Tag_Length')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_In_Tag_Length', N'خطا در طول تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_In_Tag_Identification')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_In_Tag_Identification', N'خطا در شناسایی تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Existed_In_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Existed_In_Warehouse', N'موجود در انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Hide_Filter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Hide_Filter', N'بستن فیلتر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Show_Filter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Show_Filter', N'نمایش فیلتر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Re_Identify')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Re_Identify', N'شناسایی مجدد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_First_Tag_And_Second_Tag_Mismatch')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_First_Tag_And_Second_Tag_Mismatch', N'عدم تطابق مشخصات تگ اول و تگ دوم');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Prevent_Movement_Frozen_Goods')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Prevent_Movement_Frozen_Goods', N'جلوگیری از جابجایی کالای فریز شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Prevent_Movement_Uninspected_Or_Rejected_Goods')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Prevent_Movement_Uninspected_Or_Rejected_Goods', N'جلوگیری از جابجایی کالای بازرسی نشده یا بازرسی مردود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Control_Exists_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Control_Exists_Serial', N'کنترل وجود سریال در مبدأ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Not_Exists_At_Origin')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Not_Exists_At_Origin', N'عدم وجود در مبدأ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Registration_Relocation_Mission')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Registration_Relocation_Mission', N'ثبت مأموریت جابجایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Fill_In_Login_Information')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Fill_In_Login_Information', N'پر کردن اطلاعات لاگین');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Show_Product_Info')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Show_Product_Info', N'نمایش مشخصات محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Send_Production_Product_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Send_Production_Product_Warehouse', N'ارسال تولید به انبار محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Show_View_Product_History')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Show_View_Product_History', N'نمایش مشاهده سوابق محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Tag_Writing')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Tag_Writing', N'نوشتن تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Back_To_Production')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Back_To_Production', N'برگشت به تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Back_To_Product_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Back_To_Product_Warehouse', N'برگشت به انبار محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Send_For_Download')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Send_For_Download', N'ارسال برای بارگیری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Send_To_Sell')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Send_To_Sell', N'ارسال به فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Return_From_Sale')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Return_From_Sale', N'برگشت از فروش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Number_Warehouse_Inventory')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Number_Warehouse_Inventory', N'انبارگردانی تعدادی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Receive_Tags_On_Other_Devices')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Receive_Tags_On_Other_Devices', N'دریافت تگ‌های شناسایی شده در دستگاه‌های دیگر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Code_Or_Tag_Id')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Code_Or_Tag_Id', N'کد کالا / شناسه تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Commodity_Code_Discrepancy')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Commodity_Code_Discrepancy', N'مغایرت کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Value_Discrepancy')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Value_Discrepancy', N'مغایرت مقداری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_List_Read_Tags')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_List_Read_Tags', N'لیست تگ‌های‌ خوانده شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Select_Bt_Device')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Select_Bt_Device', N'انتخاب دستگاه بلوتوث');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Get_Log')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Get_Log', N'دریافت Log');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_Similar_Tag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_Similar_Tag', N'تعریف تگ معادل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Identification_Send_To_Web')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Identification_Send_To_Web', N'شناسایی محصول و ارسال به وب');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Identification_Inventory_Errors')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Identification_Inventory_Errors', N'شناسایی خطاهای انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Image_Description')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Image_Description', N'توضیحات تصویر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inspection_Data')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inspection_Data', N'اطلاعات بازرسی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventories_List')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventories_List', N'لیست انبارگردانی‌ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Establishment_Location')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Establishment_Location', N'مکان استقرار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Movement_Settings')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Movement_Settings', N'تنظیمات جابجایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_New_Rfid_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_New_Rfid_Code', N'شناسه RFID جدید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Number_Of_Finds')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Number_Of_Finds', N'تعداد یافت شده‌ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Place_Of_Establishment')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Place_Of_Establishment', N'مکان استقرار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Without_Placement')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Without_Placement', N'بدون جانمایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Placement_List_Of_Goods_In_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Placement_List_Of_Goods_In_Warehouse', N'لیست جانمایی کالا در انبار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Placement_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Placement_Status', N'وضعیت جانمایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Print_User')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Print_User', N'کاربر چاپ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Private_Setting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Private_Setting', N'تنظیمات خصوصی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Count', N'تعداد محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Count_In_Cargo')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Count_In_Cargo', N'تعداد محصول در محموله');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Document')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Document', N'سند کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_History')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_History', N'سوابق محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Properties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Properties', N'سایر مشخصات محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Status', N'وضعیت محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Production_Date_Time')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Production_Date_Time', N'تاریخ و ساعت تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Production_Time')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Production_Time', N'ساعت تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Production_Salon')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Production_Salon', N'سالن تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Quality_Check_List')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Quality_Check_List', N'لیست کنترل کیفیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Read_Tag_And_Save_To_File')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Read_Tag_And_Save_To_File', N'قرائت تگ و ذخیره در فایل');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_And_Update_Product')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_And_Update_Product', N'ثبت و ویرایش محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_By_Defined_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_By_Defined_Serial', N'ثبت با سریال تعریف شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_By_Product_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_By_Product_Code', N'با کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_By_Product_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_By_Product_Code', N'ثبت با کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_By_Product_Code_And_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_By_Product_Code_And_Serial', N'ثبت با کد کالا و سریال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_Enter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_Enter', N'ثبت ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_Locate_Product')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_Locate_Product', N'ثبت جانمایی محصولات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_Product')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_Product', N'ثبت محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_Product_Offline')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_Product_Offline', N'ثبت آفلاین محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_Setting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_Setting', N'تنظیمات رجیستر');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_With_Group_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_With_Group_Serial', N'ثبت با سریال گروهی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Save_Failed')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Save_Failed', N'عدم موفقیت در ثبت اطلاعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Send_Data')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Send_Data', N'ارسال اطلاعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Send_Product_To_Gate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Send_Product_To_Gate', N'ارسال کالا برای گیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Send_To_Web')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Send_To_Web', N'ارسال به وب');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Separator_Phrase')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Separator_Phrase', N'عبارت جداکننده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Serial_Barcode_Identification_Type')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Serial_Barcode_Identification_Type', N'نوع تشخیص بارکد سریال:');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Server_Address')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Server_Address', N'آدرس سرور');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Server_Setting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Server_Setting', N'تنظیمات سرور');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Show_Information')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Show_Information', N'نمایش مشخصات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Show_Information_Setting')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Show_Information_Setting', N'تنظیمات نمایش مشخصات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Show_Product_History')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Show_Product_History', N'مشاهده سوابق محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Show_Serial_List')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Show_Serial_List', N'نمایش لیست سریال‌ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Single_Power_Percent')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Single_Power_Percent', N'درصد پاور تکی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Tag_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Tag_Status', N'وضعیت تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Transfer_Data_With_Server')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Transfer_Data_With_Server', N'در حال تبادل اطلاعات با سرور');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Update_Product_Value')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Update_Product_Value', N'ویرایش مقدار محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Wait_To_Read_Barcode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Wait_To_Read_Barcode', N'منتظر قرائت بارکد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Wait_To_Read_Tag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Wait_To_Read_Tag', N'منتظر قرائت تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_With_Code_Value_Product_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_With_Code_Value_Product_Serial', N'با کد، مقدار و سریال محصول');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Without_Extra_Data')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Without_Extra_Data', N'بدون اطلاعات اضافه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Dynamic_Fields')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Dynamic_Fields', N'فیلدهای دینامیک');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Open_Location_Settings')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Open_Location_Settings', N'تنظیمات مکان');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Not_Entered')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Not_Entered', N'وارد نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_No_Register')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_No_Register', N'رجیستر نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gathering_Data')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gathering_Data', N'در حال جمع‌آوری داده‌ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Are_You_Sure')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Are_You_Sure', N'آیا اطمینان دارید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Do_You_Want_Exit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Do_You_Want_Exit', N'می‌خواهید از برنامه خارج شوید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Register_Duplicates')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Register_Duplicates', N'آیا تگ تکراری رجیستر شود؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Success_Definition_Equivalent_Tag_Intend_Another')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Success_Definition_Equivalent_Tag_Intend_Another', N'تعریف تگ معادل با موفقیت انجام شد. آیا قصد تعریف تگ معادل دیگری برای این محصول دارید؟');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Are_You_Sure_Filters_Will_Delete')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Are_You_Sure_Filters_Will_Delete', N'آیا اطمینان دارید؟ فیلترها حذف می‌شوند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_If_Approved_Tags_And_Filters_Delete')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_If_Approved_Tags_And_Filters_Delete', N'در صورت تائید تمام تگ‌ها و فیلترها پاک خواهند شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Data_Saved_On_Sd_Card')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Data_Saved_On_Sd_Card', N'داده‌های مورد نظر در SD Card ذخیره شدند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Data_Saved_Successfully')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Data_Saved_Successfully', N'اطلاعات ذخیره شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Entered_Data_Is_Incorrect')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Entered_Data_Is_Incorrect', N'اطلاعات وارد شده صحیح نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Entered_Value_More_Than_Product_Value')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Entered_Value_More_Than_Product_Value', N'مقدار وارد شده از مقدار کل بیشتر است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Finding_Stopped')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Finding_Stopped', N'شناسایی متوقف شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Not_Exists_Dynamic_Fields')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Not_Exists_Dynamic_Fields', N'هیچ فیلد دینامیکی وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Min_Filter_1_And_Max_Filter_2')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Min_Filter_1_And_Max_Filter_2', N'حداقل فیلتر ۱ و حداکثر فیلتر ۲ عدد است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_No_Data_To_Write')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_No_Data_To_Write', N'مقداری برای نوشتن پیدا نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_No_Permission_To_Register_Duplicate_Serial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_No_Permission_To_Register_Duplicate_Serial', N'اجازه ثبت سریال تکراری وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Prev_Data_Will_Be_Delete')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Prev_Data_Will_Be_Delete', N'داده‌های قبلی پاک می‌شوند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Problem_In_Saving_Data')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Problem_In_Saving_Data', N'مشکلی در ذخیره اطلاعات بوجود آمده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Product_Find')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Product_Find', N'کالا شناسایی شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Registered_Successfully')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Registered_Successfully', N'با موفقیت ثبت شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Rfid_Can_Use')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Rfid_Can_Use', N'شناسه RFID قابل استفاده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Error_Occurred_In_Server_Information')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Error_Occurred_In_Server_Information', N'در اطلاعات دریافتی از سرور خطایی رخ داده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_You_Dont_Have_Access_Permission')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_You_Dont_Have_Access_Permission', N'شما اجازه دسترسی به این بخش را ندارید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_You_Have_Permission_To_Transfer_Value_To_Registred_Tag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_You_Have_Permission_To_Transfer_Value_To_Registred_Tag', N'شما اجازه انتقال مقدار به تگ جدید را دارید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Revoke_Filter_Apply')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Revoke_Filter_Apply', N'لغو فیلتر اعمال شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Read_Tags_Remove_If_Exit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Read_Tags_Remove_If_Exit', N'در صورت خروج تگ‌های خوانده شده پاک خواهند شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Gps_Network_Not_Enabled')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Gps_Network_Not_Enabled', N'مجوز مکان فعال نیست');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_No_Tag_Read')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_No_Tag_Read', N'تگی خوانده نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Wrong_Production_Salon')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Wrong_Production_Salon', N'سالن تولید به‌درستی انتخاب نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Wrong_Production_Shift')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Wrong_Production_Shift', N'شیفت تولید به‌درستی انتخاب نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Tag_Not_Register')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Tag_Not_Register', N'تگ خوانده شده رجیستر نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_There_Is_No_Filter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_There_Is_No_Filter', N'فیلتری اعمال نشده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Read_Inspect_Or_Freeze_Conflict')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Read_Inspect_Or_Freeze_Conflict', N'محصولات فریز یا بازرسی نشده قرائت شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_For_Start_Find_Product_Push_Key')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_For_Start_Find_Product_Push_Key', N'برای شروع شناسایی کالا ماشه را بفشارید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_For_Stop_Find_Product_Push_Key')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_For_Stop_Find_Product_Push_Key', N'برای پایان شناسایی کالا ماشه را بفشارید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_To_Read_Tag_Press_The_Button')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_To_Read_Tag_Press_The_Button', N'برای خواندن تگ ماشه را بفشارید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Grant_All_Permissions')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Grant_All_Permissions', N'برای ادامه باید تمامی مجوزها را واگذار نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enable_Location')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enable_Location', N'برای ادامه باید مجوز مکان را واگذار نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_First_Read_Tag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_First_Read_Tag', N'ابتدا تگ مورد نظر را بخوانید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Hundred_Unregistered_Information')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Hundred_Unregistered_Information', N'اطلاعات ثبت نشده به 100 عدد رسید. برای ادامه، اطلاعات را ثبت نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Place_Equivalent_Tag_Front_Device')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Place_Equivalent_Tag_Front_Device', N'تگ معادل را چند لحظه مقابل دستگاه قرار دهید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Update_Data')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Update_Data', N'لطفا اطلاعات را بروزرسانی کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Check_Connection_To_Server')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Check_Connection_To_Server', N'ارتباط با سرور را بررسی کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Check_Sent_Data_To_Server')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Check_Sent_Data_To_Server', N'اطلاعات ارسالی برای سرور را بررسی کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Choose_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Choose_Warehouse', N'انبار را مشخص کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_First_Save_Data')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_First_Save_Data', N'ابتدا عملیات را ذخیره کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_First_Stop_Reading_Tag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_First_Stop_Reading_Tag', N'ابتدا خواندن تگ را متوقف کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_For_Stop_Find_Product_Click_Btn')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_For_Stop_Find_Product_Click_Btn', N'برای پایان شناسایی کالا روی دکمه بلوتوث کلیک کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Read_Main_Tag')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Read_Main_Tag', N'تگ اصلی را قرائت کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Address')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Address', N'آدرس را وارد نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Information')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Information', N'اطلاعات را وارد نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Location')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Location', N'مکان را وارد نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Description')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Description', N'توضیحات را وارد نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Password')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Password', N'پسورد را وارد نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Product_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Product_Code', N'کد کالا را وارد نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Product_Document')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Product_Document', N'سند کالا را وارد نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Product_Value')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Product_Value', N'مقدار کالا را وارد نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Rows_Count_To_Get_Log')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Rows_Count_To_Get_Log', N'تعداد لاگ را وارد نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Server_Address')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Server_Address', N'آدرس سرور را وارد نمایید:');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Value_For_Edit_Shipment')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Value_For_Edit_Shipment', N'مقدار مورد نظر برای ویرایش را وارد نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Please_Select_Operation')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Please_Select_Operation', N'لطفا یک عملیات را انتخاب نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Production_Salon')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Production_Salon', N'سالن تولید را انتخاب نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Enter_Production_Shift')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Enter_Production_Shift', N'شیفت تولید را انتخاب نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Please_Select_Document_Code')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Please_Select_Document_Code', N'لطفا یک کد سند را انتخاب نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Select_Warehouse')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Select_Warehouse', N'انبار را انتخاب نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Select_It')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Select_It', N'انتخاب نمایید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Done')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Done', N'انجام شد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Entry_Status')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Entry_Status', N'وضعیت ورود');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Entered')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Entered', N'وارد شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Placed')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Placed', N'جانمایی شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Not_Placed')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Not_Placed', N'جانمایی نشده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Select')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Select', N'انتخاب کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_FromRegisterDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_FromRegisterDate', N'از تاریخ تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_FromRegisterTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_FromRegisterTime', N'از ساعت تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ToRegisterDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ToRegisterDate', N'تا تاریخ تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ToRegisterTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ToRegisterTime', N'تا ساعت تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RegisterDate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RegisterDate', N'تاریخ تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RegisterTime')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RegisterTime', N'ساعت تولید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Software_Map')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Software_Map', N'نقشه نرم افزار');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_A_TAG')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_A_TAG', N'یک تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TAGS')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TAGS', N'چند تگ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TAGS_COUNT_IN_REGISTER')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TAGS_COUNT_IN_REGISTER', N'تعداد تگ در عملیات ثبت با کد کالا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Enter_DocumentCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Enter_DocumentCode', N'وارد کردن کد سند الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Fill_Uhf_ProductSerial')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Fill_Uhf_ProductSerial', N'فیلد سریال از جدول شناسایی تگ را به کمک جدول تگ پر کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Update_Uhf_Statuses')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Update_Uhf_Statuses', N'وضعیت عملیات های شناسایی تگ را بروزرسانی کنید');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Print_Format_NotFound')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Print_Format_NotFound', N'فرمت مناسب برای چاپ یافت نشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Data_Mining_Elements')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Data_Mining_Elements', N'المان های استخراج داده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Max_Size')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Max_Size', N'حداکثر سایز مجاز {0} است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_File_Extention_Error')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_File_Extention_Error', N'فرمت فایل وارد شده غیر مجاز است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Update_Datas')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Update_Datas', N'بروزرسانی اطلاعات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_MovementAction_Submit_Form')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_MovementAction_Submit_Form', N'فرم ثبت عملیات جابجایی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse_Quarentine')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse_Quarentine', N'انبار قرنطینه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Warehouse_Sales')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Warehouse_Sales', N'انبار فروش رفته');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_IsCartablePermitted')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_IsCartablePermitted', N'اجازه تغییر در کارتابل اسناد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_IsUpdatePermitted')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_IsUpdatePermitted', N'اجازه بروز رسانی ');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_NoLimit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_NoLimit', N'بدون محدودیت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ActionControls')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ActionControls', N'کنترل های عملیاتی فعال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DocumentStatus_Permitted')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DocumentStatus_Permitted', N'وضعیت اسناد مجاز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_UploadExcel_RealityCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_UploadExcel_RealityCount', N'بارگذاری اکسل شمارش فیزیکی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Upload_Inventory_Excels')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Upload_Inventory_Excels', N'وارد کردن فیلد کد عملیات اموال گردانی برای بارگذاری فایل الزامی است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Extra')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Extra', N'اضافی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Shortage')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Shortage', N'کسری');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Conflicts')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Conflicts', N'مغایرت ها');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Histories')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Histories', N'سوابق');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Conflict')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Conflict', N'مغایرت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Count_Reality')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Count_Reality', N'شمارش فیزیکی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Conflict_Reality')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Conflict_Reality', N'مغایرت شمارش فیزیکی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventory_Count')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventory_Count', N'تعداد شناسایی شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Inventory_SumCount')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Inventory_SumCount', N'مقدار شناسایی شده');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_DisplayOrder')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_DisplayOrder', N'ترتیب نمایش');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SectionTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SectionTitle', N'عنوان دسته');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCrossPresent')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCrossPresent', N'اطلاعات پذیرش تردد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCrossEnter')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCrossEnter', N'اطلاعات ورود تردد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_TruckCrossExit')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_TruckCrossExit', N'اطلاعات خروج تردد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Compare_Equals')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Compare_Equals', N'مقدار فیلد {0} باید برابر با فیلد {1} باشد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Validation_Title_Uniqueness')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Validation_Title_Uniqueness', N'این عنوان قبلا ثبت شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RfidPower')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RfidPower', N'قدرت Rfid');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ActionTypeTitle')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ActionTypeTitle', N'عنوان عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_fld_ActionTypeFromDestinationType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_fld_ActionTypeFromDestinationType', N'نوع انبار مبدا');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ActionTypeToTypeDestinationType')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ActionTypeToTypeDestinationType', N'نوع انبار مقصد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ActionTypePermitedDocStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ActionTypePermitedDocStatus', N'وضعیت سند مجاز');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ActionTypeChangeDocStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ActionTypeChangeDocStatus', N'وضعیت تغییر سند');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ParentField')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ParentField', N'کد والد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_SectionId')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_SectionId', N'قسمت');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_RichTextEditor')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_RichTextEditor', N'RichTextEditor');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Numeric')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Numeric', N'Numeric');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Alert_Fail_Delete')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Alert_Fail_Delete', N'امکان حذف این رکورد وجود ندارد');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Serial_Add_Properties')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Serial_Add_Properties', N'مشخصات سریال');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_ActionStatus')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_ActionStatus', N'وضعیت عملیات');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Device_Name')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Device_Name', N'نام ایستگاه');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Retrieving_technical_information')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Retrieving_technical_information', N'بازیابی اطلاعات فنی');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Location_Serials_Report')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Location_Serials_Report', N'گزارش سریال های لوکیشن');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Column_Duplicate')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Column_Duplicate', N'ستون انتخاب شده قبلاً اضافه شده است');
IF NOT EXISTS (SELECT 1 FROM tbl_TextResources WHERE fld_TextResourceKey = N'APP_StringKeys_Variety_ProductCode')
    INSERT INTO tbl_TextResources (fld_TextResourceKey, fld_TextResourceValue) VALUES (N'APP_StringKeys_Variety_ProductCode', N'تنوع کدکالا');

