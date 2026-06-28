using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IMarketplaceRepository
    {
        Task<IEnumerable<MarketplaceRequestDto>> GetRequestsAsync(int tenantId);
        Task<MarketplaceRequestDto?> CreateRequestAsync(int tenantId, CreateMarketplaceRequestDto dto);
        Task<IEnumerable<MarketplaceOfferDto>> GetOffersAsync(int tenantId);
        Task<MarketplaceOfferDto?> CreateOfferAsync(int tenantId, CreateMarketplaceOfferDto dto);
        Task<bool> AcceptOfferAsync(int offerId, int tenantId);
        Task<bool> AssignVehicleAsync(int assignmentId, int providerTenantId, AssignMarketplaceVehicleDto dto);
        Task<IEnumerable<MarketplaceAssignmentDto>> GetAssignmentsAsync(int tenantId);
        Task<PagedMarketplaceTransactionResponseDto> GetTransactionsAsync(int tenantId, int page = 1, int pageSize = 7);
        Task<MarketplaceTransactionDto?> CreateTransactionAsync(int tenantId, CreateMarketplaceTransactionDto dto);
    }
}
