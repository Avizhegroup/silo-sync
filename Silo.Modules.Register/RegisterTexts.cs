namespace Silo.Modules.Register;

/// <summary>
/// Persian texts of the Register pages that have no key in <see cref="Silo.Shared.TextResources"/> yet. They are kept
/// in one place (not scattered through the pages) so each of them can be moved to a real TextResources key later by
/// replacing the member body only. Texts that do have a key are always read from TextResources, never from here.
/// </summary>
public static class RegisterTexts
{
    // ---- Shared interface texts --------------------------------------------------------------------------------

    public const string HowToUseTitle = "راهنمای کار با این صفحه";
    public const string HowToUseShow = "نمایش راهنما";
    public const string HowToUseHide = "پنهان کردن راهنما";
    public const string Optional = "اختیاری";
    public const string Required = "الزامی";
    public const string Captured = "دریافت شد";
    public const string Waiting = "در انتظار";
    public const string Running = "در حال کار";
    public const string Stopped = "متوقف";
    public const string Reading = "در حال خواندن";
    public const string Registered = "ثبت‌شده";
    public const string NotChosen = "انتخاب نشده";
    public const string PrinterDefault = "(چاپگر پیش‌فرض برنامه‌ی دستگاه)";
    public const string PrinterFromFile = "(چاپگر مشخص‌شده در فایل قالب؛ در غیر این صورت پیش‌فرض)";
    public const string PrinterLabel = "چاپگر";
    public const string LabelFormat = "قالب برچسب";
    public const string NoRows = "هنوز ردیفی وجود ندارد.";
    public const string Step1 = "مرحله‌ی ۱";

    // ---- Device host connection bar -----------------------------------------------------------------------------

    public const string DeviceTitle = "اتصال به برنامه‌ی دستگاه";
    public const string DeviceHint = "ریدر، چاپگر و اسکنر از طریق برنامه‌ی «Silo.Windows.Register» روی همین رایانه کار می‌کنند. آن برنامه را اجرا کنید و رمز دستگاه را یک‌بار وارد کنید؛ رمز در این مرورگر ذخیره می‌شود.";
    public const string DeviceConnected = "برنامه‌ی دستگاه متصل است";
    public const string DeviceDisconnected = "برنامه‌ی دستگاه متصل نیست";
    public const string DeviceToken = "رمز دستگاه";
    public const string DeviceTokenPlaceholder = "رمز نمایش‌داده‌شده در برنامه‌ی دستگاه";
    public const string DeviceConnect = "اتصال";
    public const string DeviceDisconnect = "قطع اتصال";
    public const string DeviceConnecting = "در حال اتصال...";
    public const string DeviceNeededHint = "برای استفاده از این بخش ابتدا به برنامه‌ی دستگاه وصل شوید.";
    public const string DeviceHostDisconnected = "ارتباط با برنامه‌ی دستگاه قطع شد.";
    public const string DeviceSocketFailed = "اتصال به برنامه‌ی دستگاه برقرار نشد. مطمئن شوید برنامه اجرا شده و رمز درست است.";

    // ---- Cancel the registration of a tag (CancelTag) -----------------------------------------------------------

    public const string CancelSubtitle = "ثبت یک برچسب را باطل می‌کند تا سریال و تگ دوباره قابل استفاده شوند.";
    public const string CancelHowTo1 = "سریال یا EPC محصول را تایپ کنید و Enter بزنید؛ یا برچسب را به ریدر نزدیک کنید.";
    public const string CancelHowTo2 = "مشخصات محصول را در جدول بررسی کنید.";
    public const string CancelHowTo3 = "دکمه‌ی «ابطال» را بزنید و در پیام تأیید، «تأیید» را انتخاب کنید.";
    public const string CancelStepFind = "یافتن محصول";
    public const string CancelStepFindHint = "سریال (بارکد) یا EPC ۲۴ کاراکتری را وارد کنید.";
    public const string CancelInputPlaceholder = "سریال یا EPC";
    public const string CancelReadOrType = "یک برچسب را بخوانید یا سریال یا EPC آن را تایپ کنید.";
    public const string CancelInvalidCode = "سریال یا EPC ۲۴ کاراکتری را وارد کنید (فقط حروف و ارقام انگلیسی).";
    public const string CancelProductNotLoaded = "اطلاعات محصول این برچسب بارگذاری نشد.";
    public const string CancelCheckAndConfirm = "محصول را بررسی کنید، سپس ابطال را تأیید کنید.";
    public static string CancelDone(string serial) => $"ثبت {serial} ابطال شد.";
    public const string CancelStepProduct = "مشخصات محصول";

    // ---- Tag test (RegisterTest, GroupTest) ---------------------------------------------------------------------

    public const string TestTitle = "تست برچسب و تگ";
    public const string TestSubtitle = "بررسی می‌کند برچسب چاپ‌شده و تگ RFID برای یک سریال ثبت شده‌اند یا نه.";
    public const string TestHowTo1 = "بارکد روی برچسب (سریال) را اسکن یا تایپ کنید.";
    public const string TestHowTo2 = "تگ را به ریدر نزدیک کنید یا EPC را وارد کنید.";
    public const string TestHowTo3 = "نتیجه‌ی مقایسه بالای جدول نمایش داده می‌شود: سبز یعنی درست، قرمز یعنی ناهماهنگ.";
    public const string TestStepInput = "دریافت بارکد و تگ";
    public const string TestBarcodeLabel = "بارکد برچسب (سریال)";
    public const string TestBarcodeHint = "بارکد را اسکن کنید و Enter بزنید.";
    public const string TestEpcHint = "با ریدر خوانده می‌شود؛ یا ۲۴ کاراکتر را تایپ کنید.";
    public const string TestEmpty = "بارکد برچسب را اسکن کنید و تگ را بخوانید تا مقایسه انجام شود.";
    public const string TestStepCompare = "مقایسه‌ی برچسب و تگ";
    public const string TestColumnLabel = "برچسب";
    public const string TestColumnTag = "تگ";
    public const string TestGroupLink = "تست گروهی";
    public const string TestSingleLink = "تست تکی";
    public const string TestMatch = "ثبت برچسب درست است";
    public const string TestMismatch = "برچسب برای سریال دیگری ثبت شده است";

    public const string GroupTitle = "تست گروهی تگ‌ها";
    public const string GroupSubtitle = "چند تگ را پشت‌سرهم با ریدر می‌خواند و ثبت‌شده یا ثبت‌نشده بودن هر کدام را نشان می‌دهد.";
    public const string GroupHowTo1 = "دکمه‌ی «شروع خواندن» را بزنید.";
    public const string GroupHowTo2 = "تگ‌ها را از مقابل ریدر عبور دهید؛ هر تگ فقط یک‌بار در جدول می‌آید.";
    public const string GroupHowTo3 = "ردیف قرمز یعنی تگ ثبت نشده و ردیف سبز یعنی ثبت شده است.";
    public const string GroupHowTo4 = "برای شروع دوباره «پاک کردن» را بزنید.";
    public const string GroupStart = "شروع خواندن";
    public const string GroupAllTags = "همه‌ی تگ‌ها";
    public const string GroupWaitingLookup = "در انتظار بررسی";
    public const string GroupDeviceNotConnected = "برنامه‌ی دستگاه متصل نیست.";
    public const string GroupDeviceDisconnected = "ارتباط با برنامه‌ی دستگاه قطع شد. دوباره متصل شوید و «شروع خواندن» را بزنید.";

    // ---- Register by barcode and RFID tag (PairTag) -------------------------------------------------------------

    public const string PairTitle = "ثبت با بارکد و تگ RFID";
    public const string PairSubtitle = "بارکد برچسب و تگ RFID را کنار هم می‌خواند و ثبت می‌کند.";
    public const string PairHowTo1 = "در صورت نیاز جداکننده‌ی بارکد را تنظیم کنید و «شروع» را بزنید.";
    public const string PairHowTo2 = "برگه‌ی محصول را با بارکد رو به اسکنر نگه دارید تا بارکد خوانده شود.";
    public const string PairHowTo3 = "تگ را به ریدر نزدیک کنید؛ با دریافت هر دو مقدار، ثبت خودکار انجام می‌شود.";
    public const string PairHowTo4 = "نتیجه‌ی هر ثبت در جدول پایین صفحه دیده می‌شود.";
    public const string PairStepSettings = "تنظیمات و شروع";
    public const string PairSeparatorHint = "نویسه‌ای که بارکد را از بقیه‌ی متن جدا می‌کند.";
    public const string PairAutoPrint = "پس از هر ثبت، رسید چاپ شود";
    public const string PairAutoPrintHint = "قالب رسید باید در برنامه‌ی دستگاه موجود باشد.";
    public const string PairReceiptFormat = "قالب رسید";
    public const string PairStepCapture = "دریافت بارکد و تگ";
    public const string PairBarcodeLabel = "بارکد (سریال محصول)";
    public const string PairEpcLabel = "EPC تگ";
    public const string PairResults = "نتیجه‌ی ثبت‌ها";
    public const string PairRegisteredCount = "ثبت‌شده";
    public const string PairFailedCount = "ناموفق";
    public const string PairLinkTest = "تست تگ";
    public const string PairPressStart = "برای شروع، دکمه‌ی «شروع» را بزنید.";
    public const string PairHoldSheet = "برگه‌ی محصول را با بارکد رو به اسکنر نگه دارید.";
    public const string PairCleared = "پاک شد. بارکد را اسکن و برچسب را بخوانید.";
    public const string PairBarcodeInvalid = "بارکد، سریال معتبری نیست.";
    public const string PairEpcInvalid = "EPC باید ۲۴ کاراکتر هگزادسیمال باشد.";
    public const string PairNext = "مرحله‌ی بعد: بارکد را اسکن و برچسب را بخوانید.";
    public const string PairTimeout = "زمان دریافت تمام شد و مقادیر پاک شدند. دوباره بارکد را اسکن و برچسب را بخوانید.";
    public static string PairTagBound(string boundSerial, string serial) => $"این تگ برای سریال {boundSerial} ثبت شده است. از آن سریال برداشته و به {serial} اختصاص داده شود؟";
    public const string PairTagOther = "تگ برای سریال دیگری ثبت است؛ تغییری داده نشد.";
    public const string PairNoInfo = "اطلاعاتی برای ثبت این سریال پیدا نشد.";
    public static string PairSerialRegistered(string serial) => $"سریال {serial} قبلاً ثبت شده است. تگ آن با این تگ جایگزین شود؟";
    public const string PairAlreadyLeft = "قبلاً ثبت شده بود؛ بدون تغییر ماند.";
    public const string PairUpdated = "ثبت تگ به‌روزرسانی شد.";
    public const string PairNoReceiptFormat = "چاپ خودکار روشن است، اما قالب رسید موجود نیست.";
    public static string PairReceiptFailed(string reason) => $"رسید چاپ نشد: {reason}";
    public const string PairHostDisconnected = "ارتباط با برنامه‌ی دستگاه قطع شد. دوباره متصل شوید و «شروع» را بزنید.";
    public const string PairHostNotConnected = "برنامه‌ی دستگاه متصل نیست.";

    // ---- Package of tags (ParentPackage) ------------------------------------------------------------------------

    public const string PackageTitle = "ثبت بسته (تگ والد)";
    public const string PackageSubtitle = "چند محصول را زیر یک تگ بسته گروه‌بندی می‌کند.";
    public const string PackageHowTo1 = "سریال محصولات را یکی‌یکی وارد (یا اسکن) کنید و «افزودن» را بزنید.";
    public const string PackageHowTo2 = "کد بسته یا EPC بسته را وارد کنید؛ یا تگ بسته را با ریدر بخوانید.";
    public const string PackageHowTo3 = "در صورت نیاز قالب برچسب را انتخاب و «چاپ برچسب» را بزنید.";
    public const string PackageHowTo4 = "در پایان «ذخیره» را بزنید.";
    public const string PackageStepAdd = "افزودن محصول به بسته";
    public const string PackageStepAddHint = "سریال محصول را وارد کنید یا بارکد آن را اسکن کنید.";
    public const string PackageStepPackage = "مشخص کردن بسته";
    public const string PackageStepPackageHint = "کد بسته را بنویسید و جستجو کنید؛ یا تگ بسته را بخوانید.";
    public const string PackageStepItems = "اقلام بسته و ذخیره";
    public const string PackageEpcLabel = "EPC بسته";
    public const string PackageReadTag = "خواندن تگ بسته";
    public const string PackageWaitingTag = "در انتظار تگ...";
    public const string PackageReadTagHint = "تگ بسته را با ریدر بخوانید";
    public const string PackageReaderNotConnected = "برنامه‌ی دستگاه متصل نیست";
    public const string PackagePrintLabel = "چاپ برچسب";
    public const string PackageNeedSerial = "سریال محصول را وارد کنید.";
    public const string PackageNeedCodeOrEpc = "کد بسته یا EPC بسته را وارد کنید، یا تگ بسته را با ریدر بخوانید.";
    public const string PackageNeedEpcToSave = "EPC بسته را وارد کنید یا تگ بسته را با ریدر بخوانید.";
    public const string PackageSaveFirst = "ابتدا بسته را ذخیره کنید.";
    public const string PackageDefaultPrinterLabel = "(چاپگر پیش‌فرض برنامه‌ی دستگاه)";
    public const string PackageItemsEmpty = "هنوز محصولی به بسته افزوده نشده است. سریال را وارد کنید و «افزودن» را بزنید.";

    // ---- Print and register labels (PrintRegister) --------------------------------------------------------------

    public const string PrintTitle = "ثبت و چاپ برچسب";
    public const string PrintSubtitle = "سریال‌های یک عملیات را چاپ می‌کند و در صورت نیاز تگ RFID را همزمان ثبت می‌کند.";
    public const string PrintHowTo1 = "کد عملیات را وارد و «بارگذاری سریال‌ها» را بزنید.";
    public const string PrintHowTo2 = "حالت کار، قالب برچسب و چاپگر را انتخاب کنید.";
    public const string PrintHowTo3 = "سریال‌هایی را که باید چاپ شوند علامت بزنید.";
    public const string PrintHowTo4 = "دکمه‌ی چاپ را بزنید؛ نتیجه‌ی هر ردیف در ستون «وضعیت» دیده می‌شود.";
    public const string PrintStepOperation = "عملیات";
    public const string PrintStepOperationHint = "شماره‌ی عملیاتی که در صفحه‌ی ثبت ساخته شده است.";
    public const string PrintLoadSerials = "بارگذاری سریال‌ها";
    public const string PrintStepSettings = "حالت، قالب و چاپگر";
    public const string PrintMode = "حالت کار";
    public const string PrintModeRegister = "ثبت تگ، سپس چاپ (چاپگر RFID)";
    public const string PrintModePrint = "فقط چاپ (تگ‌ها بعداً با بارکد جفت می‌شوند)";
    public const string PrintModeAutoRegister = "چاپ بارکد، سپس ثبت خودکار";
    public const string PrintFilterBrand = "فقط قالب‌های FastReport هم‌برند با محصول";
    public const string PrintFrxOutput = "خروجی FastReport";
    public const string PrintFrxPrinter = "چاپگر";
    public const string PrintFrxPdf = "یک فایل PDF (روی همین رایانه باز می‌شود)";
    public const string PrintWriteRfid = "هنگام چاپ، تراشه‌ی RFID هم نوشته شود";
    public const string PrintEpcType = "نوع EPC (از سرور)";
    public const string PrintEpcType2 = "۲ - کد محصول + سریال جدید";
    public const string PrintEpcType1 = "۱ - کد محصول + عدد تصادفی + سریال";
    public const string PrintAllowReprint = "اجازه‌ی چاپ دوباره‌ی سریال‌های ثبت‌شده";
    public const string PrintCheckPrinter = "بررسی چاپگر";
    public const string PrintStepSerials = "سریال‌ها";
    public const string PrintPair = "جفت کردن برچسب‌های چاپ‌شده با تگ";
    public const string PrintPreview = "پیش‌نمایش (PDF)";
    public const string PrintSelectedFormat = "قالب انتخاب‌شده را انتخاب کنید";
    public const string PrintExportPdf = "خروجی PDF";
    public const string PrintAndRegister = "چاپ و ثبت";
    public const string PrintRegisterAndPrint = "ثبت و چاپ";
    public const string PrintNoFormat = "برنامه‌ی دستگاه هیچ قالب چاپی ندارد. قالب‌ها را در پوشه‌ی Formats آن قرار دهید.";
    public const string PrintPdfFailed = "فایل PDF ساخته نشد.";
    public const string PrintPdfOpened = "فایل PDF روی ایستگاه کاری باز شد.";
    public const string PrinterNotReady = "چاپگر آماده نیست.";
    public const string PrintRowsEmpty = "سریالی بارگذاری نشده است. کد عملیات را وارد و «بارگذاری سریال‌ها» را بزنید.";
    public static string PrintInvalidEpc(string type) => $"سرور برای نوع «{type}» EPC معتبری برنگرداند.";
    public static string PrintRegisteredButFailed(string reason) => $"ثبت شد، اما چاپ ناموفق بود: {reason}";
    public static string PrintFailed(string reason) => $"چاپ ناموفق بود: {reason}";
    public const string PrintedPair = "چاپ شد. تگ را در صفحه‌ی ثبت با بارکد جفت کنید.";
    public const string PrintedAlready = "چاپ شد (قبلاً ثبت شده بود).";
    public const string PrintedNoDigits = "چاپ شد، اما سریال رقمی برای شناسه‌ی تگ ندارد.";
    public const string PrintedNotRegistered = "چاپ شد، اما تگ ثبت نشد.";
    public const string PrintedAndRegistered = "چاپ و ثبت شد.";
    public const string PrintedRegisteredStatusNotUpdated = "چاپ و ثبت شد، اما وضعیت چاپ به‌روز نشد.";
    public const string RegisteredAndPrinted = "ثبت و چاپ شد.";
    public const string PrintedStatusNotUpdated = "چاپ شد، اما وضعیت چاپ به‌روز نشد.";
    public static string PrintBadDate(string value) => $"تاریخ چاپ «{value}» خوانده نشد.";

    // ---- Register matrix report (RegisterMatrix) ----------------------------------------------------------------

    public const string MatrixTitle = "گزارش ماتریس ثبت";
    public const string MatrixSubtitle = "تعداد ثبت‌ها را بر اساس خط، اندازه و وضعیت کنترل کیفیت نشان می‌دهد.";
    public const string MatrixHowTo1 = "بازه‌ی تاریخ و ساعت و فیلترهای دلخواه را مشخص کنید؛ همه‌ی فیلترها اختیاری‌اند.";
    public const string MatrixHowTo2 = "«جستجو» را بزنید.";
    public const string MatrixHowTo3 = "برای دریافت نتیجه «خروجی اکسل» را بزنید.";
    public const string MatrixDateHint = "مثال: ۱۴۰۴/۰۵/۱۲";
    public const string MatrixFromTime = "از ساعت";
    public const string MatrixToTime = "تا ساعت";
    public const string MatrixTimeHint = "مثال: 06:00";
    public const string MatrixInvalidFormat = "قالب نادرست";
    public const string MatrixBadInput = "تاریخ باید مانند 1404/05/12 و ساعت مانند 06:00 باشد.";
    public const string MatrixEmpty = "ابتدا فیلترها را مشخص کنید و «جستجو» را بزنید.";

    // ---- Registration date (RegistrationDate) -------------------------------------------------------------------

    public const string DateTitle = "ویرایش تاریخ ثبت";
    public const string DateSubtitle = "تاریخ و ساعت ثبت یک محصول را در صورت مجاز بودن تغییر می‌دهد.";
    public const string DateHowTo1 = "سریال یا EPC محصول را وارد و «جستجو» را بزنید.";
    public const string DateHowTo2 = "اگر تغییر مجاز بود، تاریخ شمسی (مثلاً 1404/01/31) و ساعت را وارد کنید.";
    public const string DateHowTo3 = "«ذخیره» را بزنید.";
    public const string DateStepFind = "یافتن محصول";
    public const string DateStepProduct = "مشخصات محصول";
    public const string DateStepNew = "تاریخ و ساعت جدید";
    public const string DateFormat = "قالب: 1404/01/31";
    public const string DateTime = "ساعت";
    public const string DateTimeFormat = "قالب: 14:30";
    public const string DateInvalidInput = "تاریخ را مانند 1404/01/31 و ساعت را مانند 14:30 وارد کنید.";
    public const string DateFirst = "ابتدا سریال یا EPC محصول را وارد و «جستجو» را بزنید.";

    // ---- Batch (MultiProductBatch) ------------------------------------------------------------------------------

    public const string BatchTitle = "ثبت محصول و تعریف عملیات";
    public const string BatchSubtitle = "اطلاعات تولید، سند و محصول را برای یک عملیات ثبت می‌کند و سریال‌ها را می‌سازد.";
    public const string BatchHowTo1 = "شماره‌ی عملیات را ببینید؛ با دکمه‌های ‹ و › عملیات قبلی را باز کنید یا برای عملیات تازه «جدید» را بزنید.";
    public const string BatchHowTo2 = "خط، شیفت و انبار را انتخاب و در صورت نیاز سند تولید را برگزینید.";
    public const string BatchHowTo3 = "محصول را جستجو کنید، تعداد و مقدار هر بسته را بنویسید و «افزودن» را بزنید.";
    public const string BatchHowTo4 = "ردیف‌ها را در جدول پایین ببینید و «تأیید» را بزنید؛ سپس «ثبت و چاپ برچسب» را باز کنید.";
    public const string BatchShortcuts = "F5 پاک کردن · F6 ذخیره · F7 چاپ";
    public const string BatchStepOperation = "عملیات";
    public const string BatchOperationNumber = "شماره‌ی عملیات";
    public const string BatchOpenOperation = "باز کردن عملیات";
    public const string BatchPrevOperation = "عملیات قبلی";
    public const string BatchNextOperation = "عملیات بعدی";
    public const string BatchStepProduction = "اطلاعات تولید";
    public const string BatchStepDocument = "سند تولید";
    public const string BatchDocumentHint = "کد سند را بنویسید یا از فهرست انتخاب کنید.";
    public const string BatchDocumentList = "فهرست اسناد";
    public const string BatchDocumentItems = "اقلام سند";
    public const string BatchRequested = "درخواستی";
    public const string BatchRegisterAndPrint = "ثبت و چاپ برچسب‌ها";
    public const string BatchPrintManagement = "مدیریت چاپ";
    public const string BatchSingleMode = "تک‌محصولی (ذخیره‌ی مستقیم)";
    public const string BatchSingleModeHint = "هر بار یک محصول را مستقیماً ذخیره می‌کند.";
    public const string BatchCheckRemaining = "کنترل مانده‌ی سند";
    public const string BatchCheckRemainingHint = "از ثبت بیشتر از مانده‌ی سند جلوگیری می‌کند.";
    public const string BatchSerialList = "فهرست سریال‌ها";
    public const string BatchSerialListHint = "اختیاری است؛ اگر خالی بماند، سرور سریال‌ها را می‌سازد.";
    public const string BatchSerialSource = "منبع سریال‌ها";
    public const string BatchSerialRange = "بازه‌ی عددی";
    public const string BatchSerialExcel = "فایل اکسل";
    public const string BatchPrefixStart = "پیشوند ثابت: شروع";
    public const string BatchPrefixLength = "پیشوند ثابت: طول";
    public const string BatchExcelHeader = "ردیف اول عنوان ستون است";
    public const string BatchStepProduct = "محصول و تعداد";
    public const string BatchPackValueFromWeight = "مقدار بسته از روی وزن کل";
    public const string BatchTotalWeight = "وزن کل";
    public const string BatchNoPermission = "شما اجازه‌ی ذخیره‌ی این صفحه را ندارید؛ فقط می‌توانید اطلاعات را ببینید.";
    public const string BatchStepRows = "ردیف‌های این عملیات";
    public const string BatchRowsEmpty = "هنوز ردیفی افزوده نشده است. محصول را جستجو کنید و «افزودن» را بزنید.";
    public const string BatchSelectProductFirst = "ابتدا محصول را جستجو و انتخاب کنید.";
    public const string BatchProductNotInDocument = "محصول انتخاب‌شده در اقلام سند نیست.";
    public const string BatchDocumentInvalid = "کد سند شامل نویسه‌های غیرمجاز است.";
    public const string BatchQuantityOverRemain = "تعداد از مانده‌ی سند تولید بیشتر است.";
    public static string BatchConfirmSave(int count, string name, string code, string document) => $"{count} سریال از «{name}» ({code}) با سند {document} ثبت شود؟";
    public static string BatchSavingStopped(int saved, int total) => $"ثبت پس از {saved} از {total} سریال متوقف شد. برای ادامه «تأیید» را بزنید.";
    public const string BatchSaveOrRemoveFirst = "پیش از باز کردن عملیات دیگر، ردیف‌های ذخیره‌نشده را ثبت یا حذف کنید.";
    public const string BatchAverageNeeds = "ابتدا محصول را انتخاب کنید و تعداد و وزن کل را وارد کنید.";
    public const string BatchSerials = "سریال‌ها";
    public const string BatchLink = "مدیریت چاپ";

    // ---- Serial list builder / models (BatchModels, PrintLabelModels, TagStateInfo) -------------------------------

    public const string SerialPrefixNegative = "جایگاه و طول پیشوند نمی‌تواند منفی باشد.";
    public const string SerialPrefixTooLong = "پیشوند از شماره‌ی سریال‌ها بلندتر است.";
    public const string SerialRangeNotNumbers = "ابتدا و انتهای بازه باید عدد باشند.";
    public const string SerialRangeOrder = "انتهای بازه باید از ابتدای آن بزرگ‌تر باشد (خود انتها شامل نمی‌شود).";
    public static string SerialRangeTooMany(int max) => $"یک بازه حداکثر می‌تواند {max} سریال داشته باشد.";
    public const string SerialFileNoSheet = "فایل برگه‌ای ندارد.";
    public static string SerialFileTooMany(int max) => $"فایل بیش از {max} سریال دارد.";
    public static string SerialInvalid(string list) => $"سریال نامعتبر: {list} (فقط حروف، ارقام و _ - . / مجاز است).";
    public static string SerialDuplicate(string serial) => $"سریال «{serial}» بیش از یک‌بار آمده است.";
    public const string DateEditable = "تاریخ ثبت قابل تغییر است.";
    public const string DateNotEditable = "محصول بازرسی شده یا از انبارهای تولید خارج شده است؛ بنابراین تاریخ ثبت آن قابل تغییر نیست.";
}
