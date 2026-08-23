# UHFAPP – ReadEPC Module

**Target Framework:** .NET Framework 3.5  
**Solution:** `UHFAPP.sln`  
**Primary file:** `ReadEPCForm.cs`  
**Version:** 1.3.8

---

## Table of Contents

1. [Overview](#overview)
2. [Architecture Summary](#architecture-summary)
3. [Key Classes and Files](#key-classes-and-files)
4. [Data Models](#data-models)
5. [ReadEPC Full Flow](#readepc-full-flow)
   - [Form Initialization](#1-form-initialization)
   - [State Restoration on Load](#2-state-restoration-on-load)
   - [Filter Configuration](#3-filter-configuration)
   - [Starting a Scan](#4-starting-a-scan)
   - [Background Reading Loop](#5-background-reading-loop-test)
   - [UI Update via Delegate](#6-ui-update-via-delegate-updataepc)
   - [Stopping a Scan](#7-stopping-a-scan)
   - [Timed Auto-Stop Scan](#8-timed-auto-stop-scan-button3)
   - [Export to Excel](#9-export-to-excel)
   - [State Persistence on Close](#10-state-persistence-on-close)
6. [Method Reference](#method-reference)
7. [Event System](#event-system)
8. [Threading Model](#threading-model)
9. [Flow Diagram](#flow-diagram)

---

## Overview

`ReadEPCForm` is the main UHF tag inventory form in UHFAPP. It continuously reads EPC
(Electronic Product Code) data from UHF RFID tags in range using a connected UHF reader
device. The form supports:

- Continuous tag scanning with real-time display
- Configurable tag filtering by EPC, TID, or User memory bank
- Optional timed auto-stop scanning
- Sorted, de-duplicated tag list with read-count tracking
- Excel export of the tag list
- Bilingual UI (Chinese / English)
- State persistence across navigation

---

## Architecture Summary

```
MainForm
  └─ ReadEPCForm  (inherits BaseForm)
	   ├─ BaseForm.uhf  ──►  UHFAPI (singleton)
	   │                         └─► UHFAPI.dll  (native P/Invoke)
	   ├─ Background Thread  ──►  test() / ReadEPC()
	   ├─ SetTextCallback  ──►  UpdataEPC()  (UI thread via BeginInvoke)
	   ├─ EpcInfo List  ──►  CheckUtils (binary search insert)
	   └─ ReadEPCFormData  (static state cache)
```

`ReadEPCForm` extends `BaseForm`, which holds a reference to the `UHFAPI` singleton instance
(`uhf`). All hardware commands are issued through `uhf`.

---

## Key Classes and Files

| File | Class | Role |
|------|-------|------|
| `ReadEPCForm.cs` | `ReadEPCForm` | Main UI form for EPC scanning |
| `BaseForm.cs` | `BaseForm` | Base form; exposes `UHFAPI uhf` singleton |
| `UHFAPI.cs` | `UHFAPI` | C# wrapper around `UHFAPI.dll` via P/Invoke; implements `IUHF` |
| `interfaces/IUHF.cs` | `IUHF` | Interface defining all UHF reader hardware operations |
| `UHFTAGInfo.cs` | `UHFTAGInfo` | Data model for a single received tag from hardware |
| `utils/EpcInfo.cs` | `EpcInfo` | Application-level model for a unique EPC entry in the list |
| `ReadEPCFormData.cs` | `ReadEPCFormData` | Static state cache for form persistence between navigation |
| `utils/CheckUtils.cs` | `CheckUtils` | Binary search utilities for maintaining sorted EPC list |
| `utils/DataConvert.cs` | `DataConvert` | Hex string ↔ byte array conversion utilities |
| `utils/Common.cs` | `Common` | Global settings: language flag, tag string, form cache |
| `excel/ExcelUtils.cs` | `ExcelUtils` | Exports `ListView` data to `.xls` via Office Interop |

---

## Data Models

### `UHFTAGInfo`  *(namespace: BLEDeviceAPI)*
Returned directly by the hardware API for each received tag.

| Property | Type | Description |
|----------|------|-------------|
| `Pc` | `string` | Protocol Control word |
| `Epc` | `string` | EPC hex string |
| `Tid` | `string` | Tag Identifier hex string |
| `User` | `string` | User memory hex string |
| `Ant` | `string` | Antenna port number that read this tag |
| `Rssi` | `string` | Received Signal Strength Indicator |
| `Sensor` | `string` | Sensor data (special tags) |

### `EpcInfo`  *(namespace: UHFAPP.utils)*
Application-level wrapper stored in `epcList`. Enables binary-search ordering.

| Property | Type | Description |
|----------|------|-------------|
| `Epc` | `string` | EPC hex string |
| `Count` | `int` | Number of times this tag has been read |
| `EpcBytes` | `byte[]` | Raw EPC bytes |
| `TidBytes` | `byte[]` | Raw TID bytes |
| `EpcAndTidBytes` | `byte[]` | Concatenated EPC+TID bytes used as sort key |

### `ReadEPCFormData`  *(static state cache)*
Survives form close/reopen so data is not lost when switching tabs.

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `filter_Data` | `string` | `""` | Filter hex data |
| `filter_Ptr` | `string` | `"32"` | Filter start address (bits) |
| `filter_len` | `string` | `"0"` | Filter length (bits) |
| `filter_bank` | `int` | `1` | Filter bank: 1=EPC, 2=TID, 3=USER |
| `filter_save` | `bool` | `false` | Save filter to device flash |
| `Total` | `string` | `"0"` | Last total tag count |
| `Time` | `string` | `"0"` | Last elapsed time |
| `epcList` | `List<EpcInfo>` | empty | In-memory sorted tag list |
| `listviewdata` | `List<ListViewItem>` | empty | ListView rows |
| `selectedText` | `string` | `""` | Last selected EPC text |

---

## ReadEPC Full Flow

### 1. Form Initialization

**Constructors:** `ReadEPCForm()`, `ReadEPCForm(bool isOpen)`, `ReadEPCForm(bool isOpen, MainForm mainform)`

- `InitializeComponent()` builds all designer controls.
- `isOpen` controls whether `panel1` (the scan controls panel) is enabled or disabled
  (disabled when no device is connected).
- `mainform` reference is stored to call `disableControls()` / `enableControls()` and
  `ReadWriteTag(tag)` on the parent form.

---

### 2. State Restoration on Load

**Method:** `ScanEPCForm_Load(object sender, EventArgs e)`

Called once when the form is displayed. Performs:

1. **Event subscriptions:**
   - `MainForm.eventOpen += MainForm_eventOpen` — enables/disables the panel when the device
	 connects or disconnects.
   - `MainForm.eventSwitchUI += MainForm_eventSwitchUI` — switches all labels and button text
	 between English and Chinese.
   - `MainForm.eventMainSizeChanged += MainForm_SizeChanged` — repositions `panel1.Left` when
	 the main window resizes.

2. **Restore filter state** from `ReadEPCFormData`:
   - `filerLen.Text`, `txtData.Text`, `txtPtr.Text`, `cbSave.Checked`
   - Time and total count labels
   - Previously selected radio button (EPC / TID / USER)

3. **Restore ListView** (`lvEPC`) from `ReadEPCFormData.listviewdata`, re-selecting the
   previously selected row.

4. **Restore `epcList`** from `ReadEPCFormData.epcList`.

5. Calls `MainForm_eventSwitchUI()` immediately to apply the current language.

---

### 3. Filter Configuration

Users can optionally configure a hardware-level tag filter before scanning so only matching
tags are reported.

**Filter panel controls:** `rbEPC` / `rbTID` / `rbUser` (bank), `txtPtr` (start bit address),
`filerLen` (length in bits), `txtData` (hex filter data), `cbSave` (persist to device flash).

#### `btnSet_Click` — Apply Filter

```
User input → validate hex data and length → convert to byte[] → uhf.SetFilter(...)
```

Steps:
1. Parse `txtPtr` → `ptr` (int), `filerLen` → `leng` (int).
2. Remove spaces from `txtData` → `data`.
3. Validate `data` is valid hexadecimal using `StringUtils.IsHexNumber()`.
4. Validate the hex data is long enough to satisfy the requested bit-length:
   `(leng/8 + (leng%8==0?0:1)) * 2 <= data.Length`
5. Determine bank byte: EPC=`0x01`, TID=`0x02`, USER=`0x03`.
6. If `leng == 0`, set `data = "00"` (clear filter).
7. Convert hex string to `byte[]` via `DataConvert.HexStringToByteArray(data)`.
8. Call `uhf.SetFilter(save, bank, ptr, leng, buff)`.
   - Returns `true` → show success dialog.
   - Returns `false` → show failure dialog.

**Signature (IUHF):**
```csharp
bool SetFilter(byte saveflag, byte bank, int startaddr, int datalen, byte[] databuf);
```
- `saveflag`: `1` = write to flash, `0` = RAM only
- `bank`: `0x01` EPC, `0x02` TID, `0x03` USER
- `startaddr`: Start bit address
- `datalen`: Length in bits
- `databuf`: Filter data bytes

#### `button2_Click` — Reset All Filters

Calls `uhf.SetFilter` three times (banks 1, 2, 3) each with length=0 to clear all filters.

#### `txtData_TextChanged` / `FormatHex`

`txtData_TextChanged` calls `FormatHex(txtData)` which:
- Strips all non-hex characters.
- Reformats the string as space-separated byte pairs (e.g., `"AABB CC"` → `"AA BB CC "`).
- Updates `label5` with the byte count.

---

### 4. Starting a Scan

**Method:** `btnScanEPC_Click(object sender, EventArgs e)`

The "Start/Stop" button (`btnScanEPC`) is the scan toggle.

**If button text is "Stop" / "停止":** calls `StopEPC(true)` then opens a brief wait dialog.

**If button text is "Start" / "开始":**

```
Guard check: !isRuning && isComplete
	│
	├─ mainform.disableControls()         // lock other UI
	├─ isRuning = true
	├─ isComplete = false
	│
	└─ uhf.Inventory()
		   ├─ true  → clear label9 → StartEPC()
		   └─ false → show "Inventory failure!" → reset flags → mainform.enableControls()
```

**`uhf.Inventory()` (IUHF):**  
Sends the continuous inventory command to the UHF reader hardware. Returns `true` if the
command was accepted successfully.

---

### 5. Background Reading Loop — `test()`

Called from `ReadEPC()` which runs on a dedicated background thread spawned by `StartEPC()`.

#### `StartEPC()`
```csharp
private void StartEPC()
{
	groupBox8.Enabled = false;                         // disable filter controls
	btnScanEPC.Text = Common.isEnglish ? strStop : strStop2;
	new Thread(new ThreadStart(delegate { ReadEPC(); })).Start();
}
```

#### `ReadEPC()`
```csharp
private void ReadEPC()
{
	beginTime = System.Environment.TickCount;  // record start time
	test();                                    // blocking loop
	isComplete = true;                         // signal scan finished
}
```

#### `test()` — Core polling loop

```
Read txtTime (work duration in seconds; 0 → int.MaxValue)
Disable gbAuto group box (via UI thread Invoke)

while (true)
{
	UHFTAGInfo info = uhf.uhfGetReceived()
	│
	├─ info != null
	│     └─ BeginInvoke(setTextCallback, [epc, tid, rssi, "1", ant, user])
	│         (marshals UpdataEPC call to UI thread asynchronously)
	│
	└─ info == null (no tag in receive buffer)
		  ├─ isRuning == true  → Thread.Sleep(5)   (yield, avoid busy-wait)
		  └─ isRuning == false → break              (stop requested)

	// Timeout check
	if (elapsed >= configuredTime)
	{
		if (!isStop)
		{
			isStop = true
			uhf.StopGet()          // tell hardware to stop
			t1 = TickCount         // start 100 ms drain timer
		}
		else if (TickCount - t1 > 100)
		{
			isRuning = false       // signal loop to exit on next null receive
			break
		}
	}

	// Update elapsed time label (via UI thread Invoke)
	lblTime.Text = min(elapsed, configuredTime) + "ms"
}

// Restore UI (via UI thread Invoke)
gbAuto.Enabled = true
groupBox8.Enabled = true
btnScanEPC.Text = "Start"
mainform.enableControls()
```

**`uhf.uhfGetReceived()` (IUHF):**
```csharp
UHFTAGInfo uhfGetReceived();
```
Polls the internal receive buffer. Returns the next `UHFTAGInfo` if a tag report is available,
or `null` if the buffer is empty.

**`uhf.StopGet()` (IUHF):**
```csharp
bool StopGet();
```
Sends the stop-inventory command to the hardware.

---

### 6. UI Update via Delegate — `UpdataEPC()`

The delegate type is:
```csharp
delegate void SetTextCallback(string epc, string tid, string rssi,
							  string count, string ant, string user);
```

Bound at form load:
```csharp
setTextCallback = new SetTextCallback(UpdataEPC);
```

Called on the UI thread via `this.BeginInvoke(setTextCallback, ...)`.

#### `UpdataEPC(string epc, string tid, string rssi, string count, string ant, string user)`

```
1. Compute elapsed = TickCount - beginTime
2. Update lblTime.Text = elapsed + "ms"
3. Increment tempCount += int.Parse(count)
4. Update label6.Text = tempCount  (total reads including duplicates)

5. CheckUtils.getInsertIndex(epcList, epc, tid, exist[])
   │
   ├─ exist[0] == true  (EPC already in list)
   │     ├─ lvEPC.Items[index].SubItems["TID"].Text   = tid
   │     ├─ lvEPC.Items[index].SubItems["USER"].Text  = user
   │     ├─ lvEPC.Items[index].SubItems["RSSI"].Text  = rssi
   │     ├─ lvEPC.Items[index].SubItems["COUNT"].Text += count
   │     └─ lvEPC.Items[index].SubItems["ANT"].Text   = ant
   │
   └─ exist[0] == false  (new EPC)
		 ├─ total++
		 ├─ Build new ListViewItem with sub-items:
		 │     index+1, EPC, TID, USER, RSSI, COUNT, ANT
		 ├─ lvEPC.Items.Insert(index, lv)   (insert at sorted position)
		 ├─ epcList.Insert(index, new EpcInfo(epc, count, epcBytes, tidBytes))
		 └─ lblTotal.Text = epcList.Count

6. Update speed label:
   - elapsed < 1000ms → label9.Text = tempCount + "/s"
   - elapsed >= 1000ms → label9.Text = (tempCount / (elapsed/1000)) + "/s"
```

**`CheckUtils.getInsertIndex(List<EpcInfo>, string epc, string tid, bool[] exists)`:**  
Binary search on `epcList` using the `EpcAndTidBytes` sort key. Returns:
- The index where the EPC exists (if `exists[0] = true`).
- The index where the EPC should be inserted (if `exists[0] = false`).

---

### 7. Stopping a Scan

**Method:** `StopEPC(bool isStop)`

```csharp
private void StopEPC(bool isStop)
{
	bool result = uhf.StopGet();          // command hardware to stop
	if (!result) MessageBox.Show("停止失败");
	Thread.Sleep(100);                    // allow hardware ACK / drain
	isRuning = false;                     // signals background loop to exit
	groupBox8.Enabled = true;             // re-enable filter controls
	btnScanEPC.Text = isEnglish ? strStart : strStart2;
	mainform.enableControls();            // unlock other UI tabs
}
```

The background `test()` loop detects `isRuning == false` on the next `null` receive and
exits naturally. `ReadEPC()` then sets `isComplete = true`.

---

### 8. Timed Auto-Stop Scan — `button3`

**Method:** `button3_Click(object sender, EventArgs e)`

This hidden button (`button3`, only visible when `isPz == true`) implements a fixed-duration
scan that automatically stops after 2 seconds:

```
If currently running → StopEPC(true)
Else
	mainform.disableControls()
	isRuning = true, isComplete = false
	beginTime = TickCount
	uhf.Inventory()
		├─ success → StartEPC()
		│              then spawn second thread:
		│                  Thread.Sleep(2000) → Invoke(StopEPC(true))
		└─ failure → error dialog, reset flags
```

Unlike the general scan, the 2-second timeout is hard-coded here rather than read from
`txtTime`. The general scan uses `txtTime` with radio buttons (2s / 3s / 4s / 5s / 10s).

---

### 9. Export to Excel

**Method:** `btnExport_Click(object sender, EventArgs e)`

The Export button (`btnExport`) is hidden until the user presses F1–F4:

```csharp
private void ReadEPCForm_KeyDown(object sender, KeyEventArgs e)
{
	if (e.KeyCode == Keys.F1 || ... || e.KeyCode == Keys.F4)
		btnExport.Visible = true;
}
```

On click:
```csharp
string path = Environment.CurrentDirectory + "\\uhfData"
			  + DateTime.Now.ToString("yyyy_MM_dd_HHssmm") + ".xls";
ExcelUtils.ExportExcels(path, lvEPC);
```

`ExcelUtils.ExportExcels` uses Microsoft Office Interop to:
1. Create a new Excel workbook.
2. Write column headers from `lvEPC`.
3. Write each row's cell values.
4. Auto-fit columns.
5. Save the workbook to the generated path.

---

### 10. State Persistence on Close

**Method:** `ScanEPCForm_FormClosing(object sender, FormClosingEventArgs e)`

1. Unsubscribes all `MainForm` events.
2. If scan is running, calls `StopEPC(true)` first.
3. Writes to `ReadEPCFormData`:
   - `filter_len`, `filter_Data`, `filter_Ptr`, `filter_save`
   - `Time`, `Total`
   - `epcList`
   - `filter_bank` (from active radio button)
   - All `ListViewItem` rows from `lvEPC`
   - `selectedText` (selected row EPC)

---

## Method Reference

| Method | Location | Description |
|--------|----------|-------------|
| `ScanEPCForm_Load` | `ReadEPCForm` | Subscribe events, restore form state |
| `ScanEPCForm_FormClosing` | `ReadEPCForm` | Unsubscribe events, persist state |
| `btnScanEPC_Click` | `ReadEPCForm` | Toggle start/stop scanning |
| `StartEPC` | `ReadEPCForm` | Spawn background read thread, update button |
| `StopEPC` | `ReadEPCForm` | Send stop command, reset flags and UI |
| `ReadEPC` | `ReadEPCForm` | Background thread entry; records start time, calls `test()` |
| `test` | `ReadEPCForm` | Core polling loop: polls hardware, dispatches UI updates, handles timeout |
| `UpdataEPC` | `ReadEPCForm` | UI-thread callback; de-duplicates and inserts tag into ListView |
| `btnSet_Click` | `ReadEPCForm` | Apply tag filter to hardware |
| `button2_Click` | `ReadEPCForm` | Reset all hardware filters |
| `button3_Click` | `ReadEPCForm` | Timed 2-second auto-stop scan |
| `button1_Click` | `ReadEPCForm` | Clear all scan results |
| `btnExport_Click` | `ReadEPCForm` | Export `lvEPC` data to `.xls` |
| `FormatHex` | `ReadEPCForm` | Format `TextBox` content as spaced hex pairs |
| `lvEPC_DoubleClick` | `ReadEPCForm` | Open selected tag in Read/Write form |
| `contextMenuStrip1_Click` | `ReadEPCForm` | Copy selected EPC to clipboard |
| `MainForm_eventOpen` | `ReadEPCForm` | Handle device connect/disconnect |
| `MainForm_eventSwitchUI` | `ReadEPCForm` | Switch UI language (EN/CN) |
| `MainForm_SizeChanged` | `ReadEPCForm` | Reposition panel on window resize |
| `uhf.Inventory` | `UHFAPI` | Send continuous inventory command to reader hardware |
| `uhf.StopGet` | `UHFAPI` | Send stop-inventory command to reader hardware |
| `uhf.uhfGetReceived` | `UHFAPI` | Poll receive buffer; returns next `UHFTAGInfo` or null |
| `uhf.SetFilter` | `UHFAPI` | Configure select filter on reader hardware |
| `CheckUtils.getInsertIndex` | `CheckUtils` | Binary search; returns sorted insert index for EpcInfo list |
| `DataConvert.HexStringToByteArray` | `DataConvert` | Convert hex string to `byte[]` |
| `ExcelUtils.ExportExcels` | `ExcelUtils` | Export ListView to `.xls` via Office Interop |

---

## Event System

`MainForm` exposes three static events that `ReadEPCForm` subscribes to during its lifetime:

| Event | Trigger | Handler | Action |
|-------|---------|---------|--------|
| `MainForm.eventOpen` | Device connect / disconnect | `MainForm_eventOpen` | Enable or disable `panel1`; if disconnected and scan is running, calls `StopEPC(true)` |
| `MainForm.eventSwitchUI` | Language toggle | `MainForm_eventSwitchUI` | Switches all label and button text between English and Chinese |
| `MainForm.eventMainSizeChanged` | Main window resize | `MainForm_SizeChanged` | Sets `panel1.Left = 308` |

All three are unsubscribed in `ScanEPCForm_FormClosing` to prevent memory leaks.

---

## Threading Model

```
UI Thread (WinForms main thread)
  │
  ├─ All controls live here
  ├─ UpdataEPC() executes here (via BeginInvoke)
  ├─ StopEPC() executes here
  └─ btnScanEPC_Click, filter logic, etc.

Background Thread (one per scan session)
  │
  ├─ Started by: new Thread(new ThreadStart(delegate { ReadEPC(); })).Start()
  ├─ Entry: ReadEPC()  →  test()
  ├─ Polls: uhf.uhfGetReceived()  (non-blocking, returns null on empty buffer)
  ├─ Communicates to UI: this.BeginInvoke(setTextCallback, ...)
  ├─ UI access: txtTime.Invoke(...)  (blocking Invoke for reading work time)
  └─ Exits when: isRuning == false && receive buffer empty
```

**Thread safety mechanism:**
- `isRuning` is a plain `bool` field — writes from both UI thread (StopEPC) and background
  thread (test timeout). Reads are safe on x86/x64 for bool-sized values, but the field is
  not declared `volatile`.
- All `ListView` and label updates are marshalled to the UI thread via `BeginInvoke`
  (async) or `Invoke` (sync).

---

## Flow Diagram

```
User clicks "Start"
		│
		▼
btnScanEPC_Click()
  ├─ isRuning=true, isComplete=false
  ├─ mainform.disableControls()
  └─ uhf.Inventory()  ──► [Hardware starts RF emission + tag collection]
		│
		▼
StartEPC()
  ├─ Disable filter group box
  ├─ Button → "Stop"
  └─ new Thread → ReadEPC()
					 │
					 ▼
				 beginTime = TickCount
				 test()
					 │
			  ┌──────▼──────────────────────────────────────┐
			  │  while(true)                                  │
			  │    uhf.uhfGetReceived()                       │
			  │      ├─ UHFTAGInfo != null                    │
			  │      │    └─ BeginInvoke → UpdataEPC()        │◄──┐
			  │      └─ null                                  │   │
			  │           ├─ isRuning=true → Sleep(5) ────────┘   │
			  │           └─ isRuning=false → break               │
			  │    timeout check → uhf.StopGet() → isRuning=false │
			  │    update lblTime                                  │
			  └──────────────────────────────────────────────────-┘
					 │
				 Restore UI (gbAuto, btnScanEPC, mainform.enableControls)
				 isComplete = true

		─────────────────────────────────────────────────────
		UpdataEPC() [UI Thread]
		  ├─ Update time / total-reads labels
		  ├─ CheckUtils.getInsertIndex(epcList, epc, tid)
		  │    ├─ exists → update row in-place
		  │    └─ new   → Insert into lvEPC + epcList at sorted index
		  └─ Update speed label

		─────────────────────────────────────────────────────
		User clicks "Stop" (or timeout expires)
		  └─ StopEPC(true)
			   ├─ uhf.StopGet()  ──► [Hardware stops RF]
			   ├─ Sleep(100)
			   ├─ isRuning = false  (background thread exits on next null)
			   ├─ Re-enable filter group box
			   ├─ Button → "Start"
			   └─ mainform.enableControls()
```
