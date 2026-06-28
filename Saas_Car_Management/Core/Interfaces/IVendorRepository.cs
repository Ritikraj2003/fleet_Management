using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IVendorRepository
    {
        Task<IEnumerable<PartnerDto>> GetVendorsAsync(int tenantId);
        Task<PartnerDto?> GetVendorByIdAsync(int id, int tenantId);
        Task<PartnerDto?> CreateVendorAsync(int tenantId, CreatePartnerDto dto);
        Task<bool> UpdateVendorAsync(int id, int tenantId, CreatePartnerDto dto);
        Task<bool> DeleteVendorAsync(int id, int tenantId);

        Task<IEnumerable<PartnerVehicleDto>> GetVendorVehiclesAsync(int partnerId, int tenantId);
        Task<IEnumerable<PartnerVehicleDto>> GetAllVendorVehiclesAsync(int tenantId);
        Task<PartnerVehicleDto?> CreateVendorVehicleAsync(int tenantId, CreatePartnerVehicleDto dto);

        Task<IEnumerable<PartnerDriverDto>> GetVendorDriversAsync(int partnerId, int tenantId);
        Task<IEnumerable<PartnerDriverDto>> GetAllVendorDriversAsync(int tenantId);
        Task<PartnerDriverDto?> CreateVendorDriverAsync(int tenantId, CreatePartnerDriverDto dto);

        Task<PagedVendorPaymentResponseDto> GetVendorPaymentsAsync(int tenantId, int page = 1, int pageSize = 7);
        Task<VendorPaymentDto?> RecordPaymentAsync(int tenantId, CreateVendorPaymentDto dto);

        // Magic Link (Public)
        Task<PartnerDto?> GetVendorByTokenAsync(string token);
        Task<IEnumerable<PartnerVehicleDto>> GetVendorVehiclesByTokenAsync(string token);
        Task<bool> UpdateVehicleStatusAsync(string token, int vehicleId, string status);
    }
}
