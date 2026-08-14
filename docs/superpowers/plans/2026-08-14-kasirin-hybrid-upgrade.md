# KasirIn Hybrid Portfolio Upgrade Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Enhance KasirIn into a top-tier portfolio project by implementing Live Camera Barcode Scanning (JS Interop + html5-qrcode), Professional Excel Report Export (`ClosedXML`), and RBAC Persona Switching (`CASHIER` vs `OWNER` Mode).

**Architecture:** Extend .NET 8 Clean Architecture with `ClosedXML` Excel generation in `KasirIn.Application`/`Infrastructure`, ASP.NET Core Web API endpoints, Blazor Server `KasirInUserSession` state management, and `html5-qrcode` JS Interop for real-time barcode scanning with Web Audio API feedback.

**Tech Stack:** C# .NET 8, Blazor WebApp InteractiveServer, ClosedXML 0.104.2, html5-qrcode 2.3.8, Playwright Python.

## Global Constraints
- Target Framework: `net8.0` across all 5 projects.
- Pure White Contrast: `#ffffff` text on `#004ac6` primary active backgrounds.
- Zero Warnings & Errors: All 18+ unit tests must pass.

---

### Task 1: KasirInUserSession State & Topbar RBAC Switcher

**Files:**
- Create: `src/KasirIn.Web/Services/KasirInUserSession.cs`
- Modify: `src/KasirIn.Web/Program.cs`
- Modify: `src/KasirIn.Web/Components/Layout/MainLayout.razor`
- Modify: `src/KasirIn.Web/Components/Layout/NavMenu.razor`
- Modify: `src/KasirIn.Web/Components/Pages/POSCashier.razor`
- Modify: `src/KasirIn.Web/Components/Pages/InventoryMaster.razor`
- Modify: `src/KasirIn.Web/Components/Pages/FinancialReports.razor`

**Interfaces:**
- Consumes: None
- Produces: `KasirInUserSession` with `CurrentRole` (`"OWNER"` | `"CASHIER"`), `IsOwner`, `IsCashier`, and `OnRoleChanged` event.

- [ ] **Step 1: Create KasirInUserSession.cs**

```csharp
namespace KasirIn.Web.Services;

public class KasirInUserSession
{
    public string CurrentRole { get; private set; } = "OWNER";

    public bool IsOwner => CurrentRole == "OWNER";
    public bool IsCashier => CurrentRole == "CASHIER";

    public event Action? OnRoleChanged;

    public void SwitchRole(string newRole)
    {
        if (CurrentRole != newRole)
        {
            CurrentRole = newRole;
            OnRoleChanged?.Invoke();
        }
    }
}
```

- [ ] **Step 2: Register KasirInUserSession in Program.cs**

In `src/KasirIn.Web/Program.cs`:
```csharp
builder.Services.AddScoped<KasirInUserSession>();
```

- [ ] **Step 3: Add Topbar Quick Switcher in MainLayout.razor & NavMenu.razor**

In `MainLayout.razor`, add a persona switcher badge in the top right header:
```razor
@inject KasirInUserSession UserSession
@implements IDisposable

<div class="flex items-center gap-2 bg-surface-container border border-outline-variant rounded-full px-3 py-1 text-xs">
    <span class="font-bold text-on-surface-variant">Mode:</span>
    <button type="button" class="px-2.5 py-0.5 rounded-full font-bold transition-all @(UserSession.IsOwner ? "bg-primary text-white shadow-xs" : "text-on-surface-variant hover:text-on-surface")"
            @onclick='() => UserSession.SwitchRole("OWNER")'>
        Pemilik (Owner)
    </button>
    <button type="button" class="px-2.5 py-0.5 rounded-full font-bold transition-all @(UserSession.IsCashier ? "bg-primary text-white shadow-xs" : "text-on-surface-variant hover:text-on-surface")"
            @onclick='() => UserSession.SwitchRole("CASHIER")'>
        Kasir
    </button>
</div>
```

In `NavMenu.razor`, hide Financial Reports & Analytics Dashboard when `UserSession.IsCashier` is true.

- [ ] **Step 4: Update InventoryMaster & POSCashier HPP privacy**

In `InventoryMaster.razor`, hide Cost Price (Harga Modal) column when `UserSession.IsCashier` is true.

- [ ] **Step 5: Verify build & commit**

```bash
dotnet build KasirIn.sln
git add .
git commit -m "feat: add KasirInUserSession state management and RBAC persona switcher"
```

---

### Task 2: Excel Report Exporter via ClosedXML

**Files:**
- Modify: `src/KasirIn.Infrastructure/KasirIn.Infrastructure.csproj`
- Create: `src/KasirIn.Application/Reports/Queries/ExportProfitReportToExcel/ExportProfitReportToExcelQuery.cs`
- Create: `src/KasirIn.Application/Reports/Queries/ExportProfitReportToExcel/ExportProfitReportToExcelQueryHandler.cs`
- Create: `tests/KasirIn.UnitTests/Application/ExportProfitReportToExcelQueryHandlerTests.cs`
- Modify: `src/KasirIn.Api/Controllers/ReportsController.cs`
- Modify: `src/KasirIn.Web/Services/KasirInApiService.cs`
- Modify: `src/KasirIn.Web/Components/Pages/FinancialReports.razor`

**Interfaces:**
- Consumes: `IKasirInDbContext`
- Produces: `byte[]` containing valid `.xlsx` Excel workbook.

- [ ] **Step 1: Add ClosedXML package to KasirIn.Infrastructure**

```bash
dotnet add src/KasirIn.Infrastructure/KasirIn.Infrastructure.csproj package ClosedXML --version 0.104.2
```

- [ ] **Step 2: Create ExportProfitReportToExcelQuery & Handler**

Create `ExportProfitReportToExcelQuery.cs`:
```csharp
namespace KasirIn.Application.Reports.Queries.ExportProfitReportToExcel;

using MediatR;

public record ExportProfitReportToExcelQuery(Guid TenantId, DateTime? StartDate = null, DateTime? EndDate = null) : IRequest<byte[]>;
```

Create `ExportProfitReportToExcelQueryHandler.cs` using ClosedXML to build 3 sheets ("Rekap Keuangan", "Riwayat Penjualan", "Buku Kasbon Utang").

- [ ] **Step 3: Create Unit Test for Excel Exporter**

In `tests/KasirIn.UnitTests/Application/ExportProfitReportToExcelQueryHandlerTests.cs`:
```csharp
[Fact]
public async Task Handle_ShouldReturnValidExcelByteArray()
{
    // Verify non-null byte array with length > 0
}
```

- [ ] **Step 4: Add Controller Endpoint & Web Component Button**

In `src/KasirIn.Api/Controllers/ReportsController.cs`:
```csharp
[HttpGet("export-excel")]
public async Task<IActionResult> ExportExcel([FromQuery] Guid tenantId, CancellationToken cancellationToken)
{
    var query = new ExportProfitReportToExcelQuery(tenantId);
    var bytes = await _mediator.Send(query, cancellationToken);
    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "KasirIn_Laporan_Keuangan.xlsx");
}
```

In `FinancialReports.razor`, add **"Export Excel (.xlsx)"** button that triggers browser download.

- [ ] **Step 5: Run tests & commit**

```bash
dotnet test KasirIn.sln
git add .
git commit -m "feat: add ClosedXML Excel report generator query and API export endpoint"
```

---

### Task 3: Live Camera Barcode Scanner (JS Interop + html5-qrcode)

**Files:**
- Create: `src/KasirIn.Web/wwwroot/js/barcode-scanner.js`
- Modify: `src/KasirIn.Web/Components/App.razor`
- Modify: `src/KasirIn.Web/Components/Pages/POSCashier.razor`

**Interfaces:**
- Consumes: `html5-qrcode` CDN script in `App.razor`
- Produces: `window.KasirInScanner.startScan(dotNetRef, elementId)` & `stopScan()` with Web Audio API beep sound.

- [ ] **Step 1: Add html5-qrcode script to App.razor**

In `src/KasirIn.Web/Components/App.razor`:
```html
<script src="https://unpkg.com/html5-qrcode@2.3.8/html5-qrcode.min.js"></script>
<script src="js/barcode-scanner.js"></script>
```

- [ ] **Step 2: Create barcode-scanner.js**

In `src/KasirIn.Web/wwwroot/js/barcode-scanner.js`:
```javascript
window.KasirInScanner = {
    html5Qrcode: null,
    audioCtx: null,
    playBeep: function() {
        try {
            if (!this.audioCtx) this.audioCtx = new (window.AudioContext || window.webkitAudioContext)();
            var osc = this.audioCtx.createOscillator();
            var gain = this.audioCtx.createGain();
            osc.type = "sine";
            osc.frequency.value = 800;
            gain.gain.value = 0.1;
            osc.connect(gain);
            gain.connect(this.audioCtx.destination);
            osc.start();
            osc.stop(this.audioCtx.currentTime + 0.15);
        } catch (e) {}
    },
    startScan: function(dotNetHelper, elementId) {
        var self = this;
        if (self.html5Qrcode) self.stopScan();
        self.html5Qrcode = new Html5Qrcode(elementId);
        self.html5Qrcode.start(
            { facingMode: "environment" },
            { fps: 10, qrbox: { width: 250, height: 250 } },
            function(decodedText) {
                self.playBeep();
                dotNetHelper.invokeMethodAsync("OnBarcodeScanned", decodedText);
                self.stopScan();
            },
            function(errorMessage) {}
        ).catch(function(err) {
            console.error("Camera access error:", err);
        });
    },
    stopScan: function() {
        if (this.html5Qrcode) {
            this.html5Qrcode.stop().then(function() {
                this.html5Qrcode.clear();
                this.html5Qrcode = null;
            }).catch(function() {});
        }
    }
};
```

- [ ] **Step 3: Integrate Scanner Modal & Button in POSCashier.razor**

In `POSCashier.razor`:
- Add "Scan Kamera" button next to search bar.
- Add camera viewport container `<div id="reader"></div>` in scanner modal.
- Add `[JSInvokable] public void OnBarcodeScanned(string barcode)` C# callback to select matching product and add to cart.

- [ ] **Step 4: Verify build & commit**

```bash
dotnet build KasirIn.sln
git add .
git commit -m "feat: add real-time camera barcode scanner JS Interop with audio feedback"
```

---

### Task 4: End-to-End Verification & Automated Playwright Tests

**Files:**
- Modify: `scratch/test_kasirin_e2e.py`
- Test: Run full Playwright test suite

- [ ] **Step 1: Update Playwright E2E Test script**

In `scratch/test_kasirin_e2e.py`, add tests for persona switcher and Excel export API download verification.

- [ ] **Step 2: Run unit tests & E2E tests**

```bash
dotnet test KasirIn.sln
python -u "C:\Users\Deo\.gemini\antigravity-cli\brain\17d96f24-8870-4fd6-b5ff-b4bab788d04d\scratch\test_kasirin_e2e.py"
```

- [ ] **Step 3: Final Git Commit & Push**

```bash
git add .
git commit -m "test: verify KasirIn hybrid portfolio upgrade features with Playwright E2E"
git push origin main
```
