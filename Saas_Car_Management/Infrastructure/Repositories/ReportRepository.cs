using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;
using ClosedXML.Excel;
using System.IO;

namespace Saas_Car_Management.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> ExportBookingsAsync(int tenantId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.TenantId == tenantId)
                .Include(b => b.Customer)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Bookings");
            worksheet.Cell(1, 1).Value = "Booking ID";
            worksheet.Cell(1, 2).Value = "Customer";
            worksheet.Cell(1, 3).Value = "Scheduled Start";
            worksheet.Cell(1, 4).Value = "Scheduled End";
            worksheet.Cell(1, 5).Value = "Status";
            worksheet.Cell(1, 6).Value = "Total Amount";

            int row = 2;
            foreach (var b in bookings)
            {
                worksheet.Cell(row, 1).Value = b.Id;
                worksheet.Cell(row, 2).Value = b.Customer?.Name ?? "";
                worksheet.Cell(row, 3).Value = b.ScheduledStart.ToString("yyyy-MM-dd HH:mm");
                worksheet.Cell(row, 4).Value = b.ScheduledEnd.ToString("yyyy-MM-dd HH:mm");
                worksheet.Cell(row, 5).Value = b.Status ?? "";
                worksheet.Cell(row, 6).Value = (double)b.TotalAmount;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportRevenueAsync(int tenantId)
        {
            var invoices = await _context.Invoices
                .Where(i => i.TenantId == tenantId)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Revenue");
            worksheet.Cell(1, 1).Value = "Invoice ID";
            worksheet.Cell(1, 2).Value = "Invoice Number";
            worksheet.Cell(1, 3).Value = "Issue Date";
            worksheet.Cell(1, 4).Value = "Due Date";
            worksheet.Cell(1, 5).Value = "Total Amount";
            worksheet.Cell(1, 6).Value = "Paid Amount";
            worksheet.Cell(1, 7).Value = "Status";

            int row = 2;
            foreach (var i in invoices)
            {
                worksheet.Cell(row, 1).Value = i.Id;
                worksheet.Cell(row, 2).Value = i.InvoiceNumber ?? "";
                worksheet.Cell(row, 3).Value = i.IssueDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 4).Value = i.DueDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 5).Value = (double)i.TotalAmount;
                worksheet.Cell(row, 6).Value = (double)i.PaidAmount;
                worksheet.Cell(row, 7).Value = i.Status ?? "";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportVehiclesAsync(int tenantId)
        {
            var cars = await _context.Cars
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Vehicles");
            worksheet.Cell(1, 1).Value = "Car ID";
            worksheet.Cell(1, 2).Value = "Make";
            worksheet.Cell(1, 3).Value = "Model";
            worksheet.Cell(1, 4).Value = "Year";
            worksheet.Cell(1, 5).Value = "Plate Number";
            worksheet.Cell(1, 6).Value = "Color";
            worksheet.Cell(1, 7).Value = "Status";
            worksheet.Cell(1, 8).Value = "Is Own";

            int row = 2;
            foreach (var c in cars)
            {
                worksheet.Cell(row, 1).Value = c.Id;
                worksheet.Cell(row, 2).Value = c.Make ?? "";
                worksheet.Cell(row, 3).Value = c.Model ?? "";
                worksheet.Cell(row, 4).Value = c.Year;
                worksheet.Cell(row, 5).Value = c.PlateNumber ?? "";
                worksheet.Cell(row, 6).Value = c.Color ?? "";
                worksheet.Cell(row, 7).Value = c.Status ?? "";
                worksheet.Cell(row, 8).Value = c.IsOwnVehicle ? "Yes" : "No";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportDriversAsync(int tenantId)
        {
            var drivers = await _context.Drivers
                .Where(d => d.TenantId == tenantId)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Drivers");
            worksheet.Cell(1, 1).Value = "Driver ID";
            worksheet.Cell(1, 2).Value = "First Name";
            worksheet.Cell(1, 3).Value = "Last Name";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "Phone";
            worksheet.Cell(1, 6).Value = "License Number";
            worksheet.Cell(1, 7).Value = "Status";

            int row = 2;
            foreach (var d in drivers)
            {
                worksheet.Cell(row, 1).Value = d.Id;
                worksheet.Cell(row, 2).Value = d.FirstName ?? "";
                worksheet.Cell(row, 3).Value = d.LastName ?? "";
                worksheet.Cell(row, 4).Value = d.Email ?? "";
                worksheet.Cell(row, 5).Value = d.Phone ?? "";
                worksheet.Cell(row, 6).Value = d.LicenseNumber ?? "";
                worksheet.Cell(row, 7).Value = d.Status ?? "";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportVendorsAsync(int tenantId)
        {
            var vendors = await _context.Partners
                .Where(p => p.TenantId == tenantId && p.Type == "Vendor")
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Vendors");
            worksheet.Cell(1, 1).Value = "Vendor ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Contact Person";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "Phone";
            worksheet.Cell(1, 6).Value = "Outstanding Balance";
            worksheet.Cell(1, 7).Value = "Status";

            int row = 2;
            foreach (var v in vendors)
            {
                worksheet.Cell(row, 1).Value = v.Id;
                worksheet.Cell(row, 2).Value = v.Name ?? "";
                worksheet.Cell(row, 3).Value = v.ContactName ?? "";
                worksheet.Cell(row, 4).Value = v.Email ?? "";
                worksheet.Cell(row, 5).Value = v.Phone ?? "";
                worksheet.Cell(row, 6).Value = (double)v.Balance;
                worksheet.Cell(row, 7).Value = v.IsActive ? "Active" : "Inactive";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportMarketplaceAsync(int tenantId)
        {
            var txs = await _context.MarketplaceTransactions
                .Where(t => t.TenantId == tenantId)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Marketplace");
            worksheet.Cell(1, 1).Value = "Transaction ID";
            worksheet.Cell(1, 2).Value = "Buyer ID";
            worksheet.Cell(1, 3).Value = "Seller ID";
            worksheet.Cell(1, 4).Value = "Amount";
            worksheet.Cell(1, 5).Value = "Type";
            worksheet.Cell(1, 6).Value = "Date";
            worksheet.Cell(1, 7).Value = "Status";

            int row = 2;
            foreach (var t in txs)
            {
                worksheet.Cell(row, 1).Value = t.Id;
                worksheet.Cell(row, 2).Value = t.BuyerTenantId;
                worksheet.Cell(row, 3).Value = t.SellerTenantId;
                worksheet.Cell(row, 4).Value = (double)t.Amount;
                worksheet.Cell(row, 5).Value = t.TransactionType ?? "";
                worksheet.Cell(row, 6).Value = t.TransactionDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 7).Value = t.Status ?? "";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
