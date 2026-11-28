# راهنمای اتصال به وب‌سرویس سیلو (تیم‌یار)

این سند برای تیمی نوشته شده که می‌خواهد سیستم خودش (مثلاً نرم‌افزار انبار یا ERP) را به سیلو وصل کند. هر نه اکشنی که در `TeamYarController` هست، همین‌جا با ورودی و خروجی‌شان توضیح داده شده. اگر چیزی مبهم بود، قبل از شروع کدنویسی با تیم سیلو هماهنگ کنید.

## فهرست

1. [اطلاعات کلی](#اطلاعات-کلی)
2. [قالب پاسخ‌ها](#قالب-پاسخها)
3. [فیلد `commandUniqueId`](#فیلد-commanduniqueid)
4. [اکشن‌ها](#اکشنها)
   - [SaveTag — ثبت اطلاعات سری ساخت](#۱-savetag--ثبت-اطلاعات-سری-ساخت)
   - [UpdateTagCount — ویرایش مقادیر سری ساخت](#۲-updatetagcount--ویرایش-مقادیر-سری-ساخت)
   - [VerifyLocate — تایید ثبت نهایی جابه‌جایی](#۳-verifylocate--تایید-ثبت-نهایی-جابهجایی)
   - [SerialConsumption — ثبت مصرف سری ساخت](#۴-serialconsumption--ثبت-مصرف-سری-ساخت)
   - [DocumentForSales — ثبت حواله فروش](#۵-documentforsales--ثبت-حواله-فروش)
   - [RevokeTags — ابطال سری ساخت](#۶-revoketags--ابطال-سری-ساخت)
   - [GetTagsBySerials — استعلام وضعیت سری ساخت](#۷-gettagsbyserials--استعلام-وضعیت-سری-ساخت)
   - [DocumentForGeneral — ثبت اسناد عمومی](#۸-documentforgeneral--ثبت-اسناد-عمومی)
   - [GetRequestResponse — استعلام پاسخ درخواست](#۹-getrequestresponse--استعلام-پاسخ-درخواست)
5. [جدول وضعیت‌ها (Enum)](#جدول-وضعیتها-enum)
6. [نکات پیاده‌سازی](#نکات-پیادهسازی)

---

## اطلاعات کلی

| مورد | مقدار |
|------|-------|
| الگوی آدرس | `{BaseUrl}/RfidCore/v3/TeamYar/{ActionName}` |
| متد HTTP | همه‌ی اکشن‌ها `POST` هستند |
| Content-Type | `application/json` |
| انکودینگ | UTF-8 |
| نام فیلدها در JSON | camelCase (مثلاً `productSerial`). به بزرگی و کوچکی حروف حساس نیست، ولی همان camelCase را بفرستید. |
| مقدار Enum | به‌صورت **عدد** ارسال و دریافت می‌شود (نه رشته) |

`{BaseUrl}` آدرس سرور سیلو در محیط شماست و تیم سیلو آن را اعلام می‌کند.

مثال آدرس کامل:

```
POST /RfidCore/v3/TeamYar/SaveTag
```

## قالب پاسخ‌ها

همه‌ی پاسخ‌های موفق در یک پوشش یکسان برمی‌گردند:

```json
{
  "successful": true,
  "value": ...,
  "messages": null
}
```

| فیلد | نوع | توضیح |
|------|-----|-------|
| `successful` | `bool` | نتیجه‌ی کلی درخواست |
| `value` | بسته به اکشن |در هر اکشن متفاوت است |
| `messages` | `string[]` یا `null` | پیام‌های توضیحی (مثلاً علت خطا). ممکن است `null` باشد. |

کدهای HTTP که باید پوشش بدهید:

| کد | معنی |
|----|------|
| `200` | درخواست پردازش شد. حتماً `successful` را هم چک کنید. |
| `202` | فقط در `GetRequestResponse`: رکورد درخواست وجود دارد ولی پاسخ آن هنوز در دسترس نیست. |
| `400` | بدنه‌ی درخواست ایراد دارد (فیلد اجباری خالی، JSON خراب، نوع اشتباه و...) |
| `401` | توکن ارسال نشده یا معتبر نیست |
| `404` | اطلاعات هدف درخواست ارسالی یافت نشد |
| `409` | درخواست تکراری است |
| `5xx` | خطای داخلی سرور |

---

## Internal response audit

For handlers implementing `ITeamYarSecHandler`, `tbl_StructuredExternalConnection.fld_SecResponse` stores the final API response as `ApiResponseWithStatusCode<T>` JSON, including `successful`, `value`, `messages`, and `httpstatuscode`. The HTTP response sent to the caller is unchanged.

Capture runs outside `AppExceptionHandlerMiddleware`, so exception responses use the actual HTTP status selected by that middleware. Each attempt is correlated by its database record ID in the current HTTP context, not by `commandUniqueId`; retrying an ID does not overwrite an earlier attempt. Responses are captured before HTTP compression.

Failed handlers retain their audit records with `fld_SecStatus = -1`. Duplicate detection ignores these failed attempts, allowing retries with the same `commandUniqueId`; existing non-failed status values retain their meaning. Requests rejected before a marked handler starts do not create or overwrite a handler audit record. Audit-update errors are logged without replacing the original API response.

## فیلد `commandUniqueId`

ورودی تمام اکشن‌ها، این فیلد را دارند و **اجباری** است. در `GetRequestResponse` باید شناسه‌ی درخواست قبلی را بفرستید، نه یک شناسه‌ی جدید.

این یک شناسه‌ی یکتا برای هر درخواست است که شما می‌سازید (مثلاً یک GUID). کارش این است که اگر به هر دلیلی (قطع شدن اینترنت، timeout و...) مطمئن نشدید درخواست ثبت شده یا نه، بتوانید پاسخ ذخیره‌شده را استعلام کنید و سند دوبار ثبت نشود.

قاعده‌ی کار:

- برای هر درخواست **جدید** یک مقدار تازه بسازید.
- یک `commandUniqueId` را برای دو درخواست متفاوت استفاده نکنید.
- اگر درخواست قبلاً بدون خطا انجام شده باشد، ارسال مجدد آن `409` می‌دهد؛ برای دریافت پاسخ قبلی از `GetRequestResponse` استفاده کنید.
- استعلام هیچ عملیات ثبت یا ویرایشی را دوباره اجرا نمی‌کند و رکورد قبلی را تغییر نمی‌دهد.

---

## اکشن‌ها

### ۱. SaveTag — ثبت/ ویرایش اطلاعات سری ساخت

```
POST /RfidCore/v3/TeamYar/SaveTag
```

وقتی یک سری ساخت (Batch) در سیستم شما ایجاد می‌شود، با این اکشن اطلاعاتش را به سیلو می‌دهید تا برچسب RFID برایش ثبت و اطلاعات چاپ آماده شود.

**ورودی**

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `commandUniqueId` | string | ✔ | [شناسه‌ی یکتای درخواست](#فیلد-commanduniqueid) |
| `data` | آرایه‌ای از `SaveTagItem` | ✔ | فهرست سری‌ساخت‌هایی که باید ثبت شوند |

هر عضو `data` (`SaveTagItem`):

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `productCode` | string | ✔ | کد کالا |
| `documentCode` | string | ✔ | کد سند |
| `documentType` | int | ✔ | نوع سند (مقدار عددی؛ لیست مقادیر مجاز را تیم سیلو اعلام می‌کند) |
| `productSerial` | string | ✔ | سری ساخت |
| `warehouseCode` | string | ✔ | کد انبار |
| `productTypeCode` | string | ✔ | کد نوع کالا |
| `originalCount` | decimal | | مقدار اصلی |
| `subCount` | decimal | | مقدار فرعی |
| `date` | string | | تاریخ، مثلاً `1405/07/01` |
| `unitForOriginalCount` | string | | واحد مقدار اصلی |
| `unitForSubCount` | string | | واحد مقدار فرعی |
| `printData` | آرایه‌ای از `{ title, value }` | | فیلدهای اضافه که باید روی برچسب چاپ شوند. هر کدام `title` و `value` اجباری دارند. |

**نمونه درخواست**

```json
{
  "commandUniqueId": "5f0c2a8e-3b7d-4c1e-9a52-1d6e8f0b7c33",
  "data": [
    {
      "productCode": "P-1001",
      "documentCode": "DOC-5521",
      "documentType": 1,
      "productSerial": "3852489025529",
      "originalCount": 120,
      "subCount": 10,
      "date": "1405/07/01",
      "warehouseCode": "W-1",
      "productTypeCode": "T-02",
      "unitForOriginalCount": "کارتن",
      "unitForSubCount": "عدد",
      "printData": [
        { "title": "تاریخ انقضا", "value": "1407/07/01" },
        { "title": "شماره پروانه", "value": "88231" }
      ]
    }
  ]
}
```

**خروجی**

```json
{
  "successful": true,
  "value": true,
  "messages": null
}
```

`value` از نوع `bool` است؛ `true` یعنی ثبت انجام شد.

---

### ۲. UpdateTagCount — ویرایش مقادیر سری ساخت

```
POST /RfidCore/v3/TeamYar/UpdateTagCount
```

با این اکشن مقدار اصلی یا فرعی و واحدهای سری ساخت را ویرایش کنید. برای هر سری ساخت، فقط فیلدهایی را که تغییر کرده‌اند ارسال کنید؛ فیلدهای بدون تغییر را `null` بفرستید.

**ورودی**

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `commandUniqueId` | string | ✔ | [شناسه‌ی یکتای درخواست](#فیلد-commanduniqueid) |
| `list` | آرایه‌ای از `UpdateTagCountDto` | ✔ | فهرست سری‌ساخت‌هایی که باید ویرایش شوند |

هر عضو `list` (`UpdateTagCountDto`):

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `productSerial` | string | ✔ | سری ساخت |
| `originalCount` | decimal یا `null` | | مقدار اصلی جدید؛ اگر تغییر نکرده، `null` بفرستید |
| `subCount` | decimal یا `null` | | مقدار فرعی جدید؛ اگر تغییر نکرده، `null` بفرستید |
| `unitForOriginalCount` | string یا `null` | | واحد جدید مقدار اصلی؛ اگر تغییر نکرده، `null` بفرستید |
| `unitForSubCount` | string یا `null` | | واحد جدید مقدار فرعی؛ اگر تغییر نکرده، `null` بفرستید |

**نمونه درخواست**

```json
{
  "commandUniqueId": "8456c8ea-1fa4-4e25-b7e7-8cb39e1be571",
  "list": [
    {
      "productSerial": "3852489025529",
      "originalCount": 120,
      "subCount": 10,
      "unitForOriginalCount": "کارتن",
      "unitForSubCount": "عدد"
    }
  ]
}
```

**خروجی**

```json
{
  "successful": true,
  "value": true,
  "messages": null
}
```

---

### ۳. VerifyLocate — تایید ثبت نهایی جابه‌جایی

```
POST /RfidCore/v3/TeamYar/VerifyLocate
```

بعد از اینکه عملیات جابه‌جایی (جایگذاری) در RFID انجام شد، سیستم شما با این اکشن نهایی شدن آن را در سمت خودتان تایید می‌کند.

**ورودی**

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `commandUniqueId` | string | ✔ | [شناسه‌ی یکتای درخواست](#فیلد-commanduniqueid) |
| `serials` | آرایه‌ای از string | ✔ | فهرست سری‌های ساخت |
| `documentCode` | string | ✔ | کد سند |
| `warehouseCode` | string | ✔ | کد انبار |
| `operationCode` | string | ✔ | کد شناسه‌ی عملیات در سامانه‌ی RFID |

**نمونه درخواست**

```json
{
  "commandUniqueId": "a91d6c40-7e22-4f0a-8c1b-52e0d4c9b811",
  "serials": ["3852489025529", "3852489025530"],
  "documentCode": "DOC-5521",
  "warehouseCode": "W-1",
  "operationCode": "OP-90417"
}
```

**خروجی**

```json
{
  "successful": true,
  "value": true,
  "messages": null
}
```

---

### ۴. SerialConsumption — ثبت مصرف سری ساخت

```
POST /RfidCore/v3/TeamYar/SerialConsumption
```

وقتی سری‌های ساخت مصرف شدند (مثلاً در خط تولید)، با این اکشن مصرفشان را به سیلو اعلام کنید تا وضعیت‌شان به «مصرف شده» تغییر کند.

**ورودی**

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `commandUniqueId` | string | ✔ | [شناسه‌ی یکتای درخواست](#فیلد-commanduniqueid) |
| `serials` | آرایه‌ای از string | ✔ | فهرست سری‌های ساخت مصرف‌شده |
| `documentCode` | string | ✔ | کد سند مصرف |
| `date` | string | | تاریخ مصرف، مثلاً `1405/07/05` |

**نمونه درخواست**

```json
{
  "commandUniqueId": "0b7e3f19-52aa-4d63-bf70-9c3a1e6d2a44",
  "serials": ["3852489025529"],
  "documentCode": "CONS-778",
  "date": "1405/07/05"
}
```

**خروجی**

```json
{
  "successful": true,
  "value": true,
  "messages": null
}
```

---

### ۵. DocumentForSales — ثبت حواله فروش

```
POST /RfidCore/v3/TeamYar/DocumentForSales
```

برای ثبت، ویرایش یا ابطال حواله‌های فروش. نوع عملیات با فیلد `opType` مشخص می‌شود و می‌توانید چند حواله را در یک درخواست بفرستید. اقلام هر حواله در `items` می‌آید.

**ورودی**

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `commandUniqueId` | string | ✔ | [شناسه‌ی یکتای درخواست](#فیلد-commanduniqueid) |
| `documents` | آرایه‌ای از `SaleDocument` | ✔ | فهرست حواله‌ها |

هر عضو `documents` (`SaleDocument`):

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `documentCode` | string | ✔ | کد سند |
| `date` | string | ✔ | تاریخ، مثلاً `1405/07/10` |
| `opType` | int | ✔ | نوع عملیات: `1` ذخیره، `2` بروزرسانی، `3` ابطال ([جدول](#optype-اسناد)) |
| `refCode` | string | | کد سند مرجع |
| `customer` | string | | مشتری |
| `items` | آرایه‌ای از `SaleItem` | | اقلام حواله. برای ثبت و ویرایش حواله حتماً بفرستید. |

هر عضو `items` (`SaleItem`):

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `productCode` | string | ✔ | کد کالا |
| `count` | decimal | ✔ | مقدار |
| `unit` | string | | واحد |

**نمونه درخواست**

```json
{
  "commandUniqueId": "c4d2f8a1-6e90-47b3-a1d5-0f7b93e2c655",
  "documents": [
    {
      "documentCode": "SAL-3042",
      "refCode": "INV-9921",
      "customer": "داروخانه مرکزی",
      "date": "1405/07/10",
      "opType": 1,
      "items": [
        { "productCode": "P-1001", "count": 50, "unit": "کارتن" },
        { "productCode": "P-1002", "count": 12.5, "unit": "عدد" }
      ]
    }
  ]
}
```

**خروجی**

```json
{
  "successful": true,
  "value": true,
  "messages": null
}
```

---

### ۶. RevokeTags — ابطال سری ساخت

```
POST /RfidCore/v3/TeamYar/RevokeTags
```

برای باطل کردن سری‌های ساخت. بعد از ابطال، وضعیت سری در استعلام `Revoked` (مقدار `4`) برمی‌گردد.

**ورودی**

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `commandUniqueId` | string | ✔ | [شناسه‌ی یکتای درخواست](#فیلد-commanduniqueid) |
| `serials` | آرایه‌ای از string | ✔ | فهرست سری‌های ساخت برای ابطال |
| `documentCode` | string | ✔ | کد سند ابطال |
| `desc` | string | | توضیحات (مثلاً علت ابطال) |

**نمونه درخواست**

```json
{
  "commandUniqueId": "e18b05d7-93c4-4a2f-8d61-7a4c0b5f1e29",
  "serials": ["3852489025529"],
  "documentCode": "REV-120",
  "desc": "ثبت اشتباه سری ساخت"
}
```

**خروجی**

```json
{
  "successful": true,
  "value": true,
  "messages": null
}
```

---

### ۷. GetTagsBySerials — استعلام وضعیت سری ساخت

```
POST /RfidCore/v3/TeamYar/GetTagsBySerials
```

یکی از اکشن‌های «خواندنی» این مجموعه است و `commandUniqueId` می‌خواهد. با فهرست سری‌ها یا بازه‌ی تاریخ، وضعیت و محل فعلی برچسب‌ها را برمی‌گرداند.

**ورودی**

`commandUniqueId` اجباری است. فیلدهای فیلتر به‌تنهایی اختیاری‌اند، اما باید حداقل یک سری معتبر یا یکی از مرزهای تاریخ (`fromDate` یا `toDate`) را بفرستید؛ فقط ارسال شناسه کافی نیست. تاریخ‌ها باید شمسی و با قالب `yyyy/MM/dd` باشند و در صورت ارسال هر دو، `fromDate` نباید بعد از `toDate` باشد.

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `commandUniqueId` | string | ✔ | [شناسه‌ی یکتای درخواست](#فیلد-commanduniqueid)؛ برای هر استعلام جدید مقدار تازه بسازید. |
| `serials` | آرایه‌ای از string | | فهرست سری‌های ساخت مورد استعلام |
| `fromDate` | string | | از تاریخ، مثلاً `1405/07/01` |
| `toDate` | string | | تا تاریخ، مثلاً `1405/07/30` |

**نمونه درخواست**

```json
{
  "commandUniqueId": "a1c9d75f-8090-4826-9cb3-6c3e5b25d928",
  "serials": ["3852489025529"],
  "fromDate": "1405/07/01",
  "toDate": "1405/07/30"
}
```

**خروجی**

`value` یک شیء است که آرایه‌ی `result` را دارد.

| فیلد | نوع | توضیح |
|------|-----|-------|
| `result[].productSerial` | string | سری ساخت |
| `result[].epc` | string | سریال RFID (EPC) برچسب |
| `result[].status` | int | وضعیت برچسب؛ [جدول وضعیت‌ها](#وضعیت-برچسب) |
| `result[].warehouseCode` | string | کد انبار فعلی |
| `result[].locationCode` | string | کد لوکیشن فعلی |

**نمونه پاسخ**

```json
{
  "successful": true,
  "value": {
    "result": [
      {
        "productSerial": "3852489025529",
        "epc": "E20047027B606426C8D3010B",
        "status": 1,
        "warehouseCode": "W-1",
        "locationCode": "L-1"
      }
    ]
  },
  "messages": null
}
```

---

### ۸. DocumentForGeneral — ثبت اسناد عمومی

```
POST /RfidCore/v3/TeamYar/DocumentForGeneral
```

برای اسنادی که حواله‌ی فروش نیستند و بین دو انبار جابه‌جایی دارند (انبار مبدأ و مقصد). مثل حواله‌ی فروش، می‌توانید چند سند را در یک درخواست بفرستید و نوع عملیات را با `opType` مشخص کنید.

**ورودی**

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `commandUniqueId` | string | ✔ | [شناسه‌ی یکتای درخواست](#فیلد-commanduniqueid) |
| `list` | آرایه‌ای از `GeneralDocument` | ✔ | فهرست اسناد |

هر عضو `list` (`GeneralDocument`):

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `documentCode` | string | ✔ | کد سند |
| `date` | string | ✔ | تاریخ، مثلاً `1405/07/12` |
| `originWarehouseCode` | string | ✔ | کد انبار مبدأ |
| `destinationWarehouseCode` | string | ✔ | کد انبار مقصد |
| `documentType` | int | ✔ | نوع سند (مقادیر در جدول انواع سند) |
| `opType` | int | ✔ | نوع عملیات: `1` ذخیره، `2` بروزرسانی، `3` ابطال ([جدول](#optype-اسناد)) |
| `items` | آرایه‌ای از `GeneralItem` | | اقلام سند. برای ثبت و ویرایش حتماً بفرستید. |

هر عضو `items` (`GeneralItem`):

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `productCode` | string | ✔ | کد کالا |
| `count` | decimal | ✔ | مقدار |
| `unit` | string | | واحد |

**انواع سند**

| نوع سند | مقدار `documentType` |
|----------|:--------------------:|
| رسید خرید | `1` |
| حواله انتقال | `36` |
| حواله مصرف مرکز | `4` |

**نمونه درخواست**

```json
{
  "commandUniqueId": "7d3a9b52-1c48-4e07-a6f3-2b9e5d8c0a17",
  "list": [
    {
      "documentCode": "GEN-2210",
      "date": "1405/07/12",
      "originWarehouseCode": "W-1",
      "destinationWarehouseCode": "W-2",
      "documentType": 3,
      "opType": 1,
      "items": [
        { "productCode": "P-1001", "count": 30, "unit": "کارتن" }
      ]
    }
  ]
}
```

**خروجی**

```json
{
  "successful": true,
  "value": true,
  "messages": null
}
```

---

### ۹. GetRequestResponse — استعلام پاسخ درخواست

```
POST /RfidCore/v3/TeamYar/GetRequestResponse
```

پاسخ ذخیره‌شده‌ی آخرین تلاش برای `commandUniqueId` را برمی‌گرداند. 

**ورودی**

| فیلد | نوع | اجباری | توضیح |
|------|-----|:------:|-------|
| `commandUniqueId` | string | ✔ | دقیقاً شناسه‌ی درخواست قبلی؛ مقدار خالی، فاصله یا حذف فیلد معتبر نیست. |

**نمونه درخواست**

```json
{
  "commandUniqueId": "5f0c2a8e-3b7d-4c1e-9a52-1d6e8f0b7c33"
}
```

**نمونه پاسخ برای درخواست موفق قبلی — HTTP 200**

```json
{
  "successful": true,
  "value": {
    "successful": true,
    "value": true,
    "messages": null,
    "httpstatuscode": 200
  },
  "messages": null
}
```

**نمونه پاسخ برای درخواست ناموفق قبلی — HTTP 200**

```json
{
  "successful": true,
  "value": {
    "successful": false,
    "value": null,
    "messages": ["اطلاعات درخواست معتبر نیست."],
    "httpstatuscode": 400
  },
  "messages": null
}
```

پیام خطای بالا نمونه است؛ `value.messages` همان پیام‌های ذخیره‌شده‌ی درخواست اصلی است.

| فیلد | توضیح |
|------|-------|
| `successful` | موفقیت استعلام پاسخ؛ به معنی موفقیت عملیات اصلی نیست. |
| `value` | پاسخ ذخیره‌شده به‌صورت شیء JSON، نه رشته‌ی JSON. |
| `value.successful` | موفقیت یا شکست درخواست اصلی. |
| `value.value` | خروجی اصلی با همان نوع JSON ذخیره‌شده (`bool`، شیء، آرایه یا `null`). |
| `value.messages` | پیام‌های درخواست اصلی. |
| `value.httpstatuscode` | کد HTTP درخواست اصلی؛ با کد HTTP استعلام تفاوت دارد. |

**وضعیت‌های استعلام**

- `200`: پاسخ ذخیره‌شده پیدا شد؛ حتی اگر کد درخواست اصلی در `value.httpstatuscode` برابر `400`، `401` یا `500` باشد.
- `202`: رکورد آخرین تلاش وجود دارد ولی `Response` هنوز `null` است. ممکن است درخواست در حال پردازش باشد یا ذخیره‌ی پاسخ با خطا مواجه شده باشد؛ این وضعیت تضمین نمی‌کند که پردازش هنوز ادامه دارد. پاسخ شامل `successful: false`، `value: null` و پیام تلاش مجدد است. پس از کمی صبر دوباره استعلام کنید.
- `404`: هیچ رکوردی برای شناسه وجود ندارد؛ پاسخ شامل `successful: false`، `value: null` و پیام یافت‌نشدن است.
- `400`: ورودی نامعتبر است؛ مثلاً شناسه خالی است یا JSON درخواست معتبر نیست.
- `5xx`: خطا در استعلام یا خواندن پاسخ ذخیره‌شده؛ داده‌ی JSON خراب به‌عنوان پاسخ موفق یا وضعیت `202` بازگردانده نمی‌شود.

اگر چند تلاش با یک شناسه وجود داشته باشد، رکورد دارای بزرگ‌ترین `Id` انتخاب می‌شود؛ وجود پاسخ در تلاش قدیمی باعث نمی‌شود پاسخ آن به‌جای تلاش آخر بازگردانده شود. استعلام خواندنی است، مشمول بررسی تکراری‌بودن فرمان نیست و رکورد لاگ جدیدی ایجاد نمی‌کند.

---

## جدول وضعیت‌ها (Enum)

### وضعیت برچسب

(فیلد `status` در `GetTagsBySerials`)

| مقدار | نام | معنی |
|:-----:|-----|------|
| 1 | `Active` | فعال |
| 2 | `Freezed` | فریز شده |
| 3 | `Used` | مصرف شده |
| 4 | `Revoked` | باطل شده |
| 5 | `Exited` | خارج شده |

### opType اسناد

(فیلد `opType` در `DocumentForSales` و `DocumentForGeneral`)

| مقدار | نام | معنی |
|:-----:|-----|------|
| 1 | `Save` | ذخیره |
| 2 | `Update` | بروزرسانی |
| 3 | `Revoke` | ابطال |

## نکات پیاده‌سازی

- **تاریخ‌ها** را شمسی و با قالب `yyyy/MM/dd` بفرستید (مثل `1405/07/01`).
- بعد از قطع ارتباط یا دریافت خطای تکراری، ابتدا با همان `commandUniqueId` از `GetRequestResponse` استعلام بگیرید. کد HTTP استعلام را با `value.httpstatuscode` اشتباه نگیرید و برای تصمیم‌گیری نتیجه‌ی داخلی را هم بررسی کنید.
