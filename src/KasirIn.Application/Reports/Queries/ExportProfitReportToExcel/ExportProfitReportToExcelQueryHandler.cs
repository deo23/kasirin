namespace KasirIn.Application.Reports.Queries.ExportProfitReportToExcel;

using ClosedXML.Excel;
using KasirIn.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class ExportProfitReportToExcelQueryHandler : IRequestHandler<ExportProfitReportToExcelQuery, byte[]>
{
    private readonly IKasirInDbContext _context;

    public ExportProfitReportToExcelQueryHandler(IKasirInDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> Handle(ExportProfitReportToExcelQuery request, CancellationToken cancellationToken)
    {
        var transactionsQuery = _context.Transactions
            .Include(t => t.TransactionItems)
            .Where(t => t.TenantId == request.TenantId);

        if (request.StartDate.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(t => t.TransactionDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(t => t.TransactionDate <= request.EndDate.Value);
        }

        var transactions = await transactionsQuery
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);

        var debts = await _context.DebtRecords
            .Where(d => d.TenantId == request.TenantId)
            .OrderBy(d => d.CustomerName)
            .ToListAsync(cancellationToken);

        decimal totalRevenue = transactions.Sum(t => t.TotalAmount);
        decimal totalCost = transactions.Sum(t => t.TransactionItems.Sum(i => i.CostPrice * i.Quantity));
        decimal netProfit = totalRevenue - totalCost;

        using var workbook = new XLWorkbook();

        // -------------------------------------------------------------
        // Sheet 1: Ringkasan Keuangan
        // -------------------------------------------------------------
        var sheetSummary = workbook.Worksheets.Add("Rekap Keuangan");
        sheetSummary.Cell("A1").Value = "KASIRIN - LAPORAN REKAPITULASI KEUANGAN UMKM";
        sheetSummary.Cell("A1").Style.Font.Bold = true;
        sheetSummary.Cell("A1").Style.Font.FontSize = 14;

        sheetSummary.Cell("A3").Value = "Total Omset Penjualan";
        sheetSummary.Cell("B3").Value = totalRevenue;
        sheetSummary.Cell("B3").Style.NumberFormat.Format = "Rp #,##0";

        sheetSummary.Cell("A4").Value = "Total Harga Modal (HPP)";
        sheetSummary.Cell("B4").Value = totalCost;
        sheetSummary.Cell("B4").Style.NumberFormat.Format = "Rp #,##0";

        sheetSummary.Cell("A5").Value = "Laba Bersih (Net Profit)";
        sheetSummary.Cell("B5").Value = netProfit;
        sheetSummary.Cell("B5").Style.Font.Bold = true;
        sheetSummary.Cell("B5").Style.NumberFormat.Format = "Rp #,##0";

        sheetSummary.Cell("A6").Value = "Total Transaksi";
        sheetSummary.Cell("B6").Value = transactions.Count;

        sheetSummary.Columns().AdjustToContents();

        // -------------------------------------------------------------
        // Sheet 2: Riwayat Penjualan
        // -------------------------------------------------------------
        var sheetTransactions = workbook.Worksheets.Add("Riwayat Penjualan");
        sheetTransactions.Cell("A1").Value = "No Faktur";
        sheetTransactions.Cell("B1").Value = "Tanggal Transaksi";
        sheetTransactions.Cell("C1").Value = "Total Pembayaran";
        sheetTransactions.Cell("D1").Value = "Metode Pembayaran";
        sheetTransactions.Cell("E1").Value = "Estimasi Profit";

        var headerRange = sheetTransactions.Range("A1:E1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#004AC6");
        headerRange.Style.Font.FontColor = XLColor.White;

        int row = 2;
        foreach (var trx in transactions)
        {
            decimal trxProfit = trx.TransactionItems.Sum(i => (i.UnitPrice - i.CostPrice) * i.Quantity);

            sheetTransactions.Cell(row, 1).Value = trx.InvoiceNumber;
            sheetTransactions.Cell(row, 2).Value = trx.TransactionDate.ToString("yyyy-MM-dd HH:mm");
            sheetTransactions.Cell(row, 3).Value = trx.TotalAmount;
            sheetTransactions.Cell(row, 3).Style.NumberFormat.Format = "Rp #,##0";
            sheetTransactions.Cell(row, 4).Value = trx.PaymentMethod;
            sheetTransactions.Cell(row, 5).Value = trxProfit;
            sheetTransactions.Cell(row, 5).Style.NumberFormat.Format = "Rp #,##0";
            row++;
        }

        sheetTransactions.Columns().AdjustToContents();

        // -------------------------------------------------------------
        // Sheet 3: Buku Kasbon Utang
        // -------------------------------------------------------------
        var sheetDebts = workbook.Worksheets.Add("Buku Kasbon Utang");
        sheetDebts.Cell("A1").Value = "Nama Pelanggan";
        sheetDebts.Cell("B1").Value = "No HP";
        sheetDebts.Cell("C1").Value = "Total Utang";
        sheetDebts.Cell("D1").Value = "Sudah Dibayar";
        sheetDebts.Cell("E1").Value = "Sisa Utang";
        sheetDebts.Cell("F1").Value = "Status";

        var debtHeaderRange = sheetDebts.Range("A1:F1");
        debtHeaderRange.Style.Font.Bold = true;
        debtHeaderRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#004AC6");
        debtHeaderRange.Style.Font.FontColor = XLColor.White;

        int debtRow = 2;
        foreach (var debt in debts)
        {
            sheetDebts.Cell(debtRow, 1).Value = debt.CustomerName;
            sheetDebts.Cell(debtRow, 2).Value = debt.CustomerPhone;
            sheetDebts.Cell(debtRow, 3).Value = debt.TotalDebt;
            sheetDebts.Cell(debtRow, 3).Style.NumberFormat.Format = "Rp #,##0";
            sheetDebts.Cell(debtRow, 4).Value = debt.PaidDebt;
            sheetDebts.Cell(debtRow, 4).Style.NumberFormat.Format = "Rp #,##0";
            sheetDebts.Cell(debtRow, 5).Value = debt.RemainingDebt;
            sheetDebts.Cell(debtRow, 5).Style.NumberFormat.Format = "Rp #,##0";
            sheetDebts.Cell(debtRow, 6).Value = debt.IsSettled ? "Lunas" : "Belum Lunas";
            debtRow++;
        }

        sheetDebts.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}
