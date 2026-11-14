# راهنمای اتصال به وب‌سرویس سیلو (تیم‌یار)

این سند برای تیمی نوشته شده که می‌خواهد سیستم خودش (مثلاً نرم‌افزار انبار یا ERP) را به سیلو وصل کند. هر شش اکشنی که در `TeamYarController` هست، همین‌جا با ورودی و خروجی‌شان توضیح داده شده. اگر چیزی مبهم بود، قبل از شروع کدنویسی با تیم سیلو هماهنگ کنید.

## فهرست

1. [اطلاعات کلی](#اطلاعات-کلی)
2. [قالب پاسخ‌ها](#قالب-پاسخها)
3. [فیلد `commandUniqueId`](#فیلد-commanduniqueid)
4. [اکشن‌ها](#اکشنها)
   - [SaveTag — ثبت اطلاعات سری ساخت](#۱-savetag--ثبت-اطلاعات-سری-ساخت)
   - [VerifyLocate — تایید ثبت نهایی جابه‌جایی](#۲-verifylocate--تایید-ثبت-نهایی-جابهجایی)
   - [SerialConsumption — ثبت مصرف سری ساخت](#۳-serialconsumption--ثبت-مصرف-سری-ساخت)
   - [DocumentForSales — ثبت حواله فروش](#۴-documentforsales--ثبت-حواله-فروش)
   - [RevokeTags — ابطال سری ساخت](#۵-revoketags--ابطال-سری-ساخت)
   - [GetTagsBySerials — استعلام وضعیت سری ساخت](#۶-gettagsbyserials--استعلام-وضعیت-سری-ساخت)
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
| `value` | بسته به اکشن | خروجی اصلی. برای ۵ اکشن اول `bool` است و برای استعلام یک شیء. |
| `messages` | `string[]` یا `null` | پیام‌های توضیحی (مثلاً علت خطا). ممکن است `null` باشد. |

کدهای HTTP که باید پوشش بدهید:

| کد | معنی |
|----|------|
| `200` | درخواست پردازش شد. حتماً `successful` را هم چک کنید. |
| `400` | بدنه‌ی درخواست ایراد دارد (فیلد اجباری خالی، JSON خراب، نوع اشتباه و...) |
| `401` | توکن ارسال نشده یا معتبر نیست |
| `5xx` | خطای داخلی سرور. بعد از کمی صبر و با **همان `commandUniqueId`** دوباره تلاش کنید. |

---

## فیلد `commandUniqueId`

ورودی تمام اکشن‌ها به‌جز `GetTagsBySerials` این فیلد را دارند و **اجباری** است.

این یک شناسه‌ی یکتا برای هر درخواست است که شما می‌سازید (مثلاً یک GUID). کارش این است که اگر به هر دلیلی (قطع شدن اینترنت، timeout و...) مطمئن نشدید درخواست ثبت شده یا نه، بتوانید همان درخواست را دوباره بفرستید و سند دوبار ثبت نشود.

قاعده‌ی کار:

- برای هر درخواست **جدید** یک مقدار تازه بسازید.
- برای **تلاش مجدد** همان درخواست، دقیقاً همان مقدار قبلی را بفرستید.
- یک `commandUniqueId` را برای دو درخواست متفاوت استفاده نکنید.

---

## اکشن‌ها

### ۱. SaveTag — ثبت اطلاعات سری ساخت

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
| `unit` | string | | واحد |
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
      "unit": "عدد",
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

### ۲. VerifyLocate — تایید ثبت نهایی جابه‌جایی

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

### ۳. SerialConsumption — ثبت مصرف سری ساخت

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

### ۴. DocumentForSales — ثبت حواله فروش

```
POST /RfidCore/v3/TeamYar/DocumentForSales
```

برای ثبت، ویرایش یا ابطال حواله‌های فروش. نوع عملیات با فیلد `opType` مشخص می‌شود و می‌توانید چند حواله را در یک درخواست بفرستید.

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
| `opType` | int | ✔ | نوع عملیات: `1` ذخیره، `2` بروزرسانی، `3` ابطال ([جدول](#optype-حواله-فروش)) |
| `refSalesCode` | string | | کد سند مرجع فروش |
| `customer` | string | | مشتری |

**نمونه درخواست**

```json
{
  "commandUniqueId": "c4d2f8a1-6e90-47b3-a1d5-0f7b93e2c655",
  "documents": [
    {
      "documentCode": "SAL-3042",
      "refSalesCode": "INV-9921",
      "customer": "داروخانه مرکزی",
      "date": "1405/07/10",
      "opType": 1
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

### ۵. RevokeTags — ابطال سری ساخت

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

### ۶. GetTagsBySerials — استعلام وضعیت سری ساخت

```
POST /RfidCore/v3/TeamYar/GetTagsBySerials
```

تنها اکشن «خواندنی» این مجموعه است و `commandUniqueId` نمی‌خواهد. با فهرست سری‌ها یا بازه‌ی تاریخ، وضعیت و محل فعلی برچسب‌ها را برمی‌گرداند.

**ورودی**

همه‌ی فیلدها اختیاری‌اند.

| فیلد | نوع | توضیح |
|------|-----|-------|
| `serials` | آرایه‌ای از string | فهرست سری‌های ساخت مورد استعلام |
| `fromDate` | string | از تاریخ، مثلاً `1405/07/01` |
| `toDate` | string | تا تاریخ، مثلاً `1405/07/30` |

**نمونه درخواست**

```json
{
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

### opType حواله فروش

(فیلد `opType` در `DocumentForSales`)

| مقدار | نام | معنی |
|:-----:|-----|------|
| 1 | `Save` | ذخیره |
| 2 | `Update` | بروزرسانی |
| 3 | `Revoke` | ابطال |

## نکات پیاده‌سازی

- **تاریخ‌ها** را شمسی و با قالب `yyyy/MM/dd` بفرستید (مثل `1405/07/01`).
