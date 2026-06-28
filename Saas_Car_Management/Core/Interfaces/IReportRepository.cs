using System.Threading.Tasks;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IReportRepository
    {
        Task<byte[]> ExportBookingsAsync(int tenantId);
        Task<byte[]> ExportRevenueAsync(int tenantId);
        Task<byte[]> ExportVehiclesAsync(int tenantId);
        Task<byte[]> ExportDriversAsync(int tenantId);
        Task<byte[]> ExportVendorsAsync(int tenantId);
        Task<byte[]> ExportMarketplaceAsync(int tenantId);
    }
}
