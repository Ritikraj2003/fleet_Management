using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class ReportsController : BaseApiController
    {
        private readonly IReportRepository _reportRepository;

        public ReportsController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet("export/bookings")]
        public async Task<IActionResult> ExportBookings()
        {
            var data = await _reportRepository.ExportBookingsAsync(GetTenantId());
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"bookings_report_{System.DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        [HttpGet("export/revenue")]
        public async Task<IActionResult> ExportRevenue()
        {
            var data = await _reportRepository.ExportRevenueAsync(GetTenantId());
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"revenue_report_{System.DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        [HttpGet("export/vehicles")]
        public async Task<IActionResult> ExportVehicles()
        {
            var data = await _reportRepository.ExportVehiclesAsync(GetTenantId());
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"vehicles_report_{System.DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        [HttpGet("export/drivers")]
        public async Task<IActionResult> ExportDrivers()
        {
            var data = await _reportRepository.ExportDriversAsync(GetTenantId());
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"drivers_report_{System.DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        [HttpGet("export/vendors")]
        public async Task<IActionResult> ExportVendors()
        {
            var data = await _reportRepository.ExportVendorsAsync(GetTenantId());
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"vendors_report_{System.DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        [HttpGet("export/marketplace")]
        public async Task<IActionResult> ExportMarketplace()
        {
            var data = await _reportRepository.ExportMarketplaceAsync(GetTenantId());
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"marketplace_report_{System.DateTime.UtcNow:yyyyMMdd}.xlsx");
        }
    }
}
