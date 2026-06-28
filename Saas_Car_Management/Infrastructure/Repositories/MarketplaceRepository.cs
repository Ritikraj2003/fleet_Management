using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;

namespace Saas_Car_Management.Infrastructure.Repositories
{
    public class MarketplaceRepository : IMarketplaceRepository
    {
        private readonly ApplicationDbContext _context;

        public MarketplaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MarketplaceRequestDto>> GetRequestsAsync(int tenantId)
        {
            // All active open requests from OTHER companies. Own listings shouldn't be in the Demand Board.
            return await _context.MarketplaceRequests
                .IgnoreQueryFilters()
                .Where(r => r.Status == "Open" && r.TenantId != tenantId)
                .Select(r => new MarketplaceRequestDto
                {
                    Id = r.Id,
                    TenantId = r.TenantId,
                    BuyerCompanyName = _context.Tenants.FirstOrDefault(t => t.Id == r.TenantId)!.CompanyName,
                    RequiredQty = r.RequiredQty,
                    VehicleTypeRequired = r.VehicleTypeRequired,
                    ScheduledStart = r.ScheduledStart,
                    ScheduledEnd = r.ScheduledEnd,
                    TargetPrice = r.TargetPrice,
                    Status = r.Status
                })
                .ToListAsync();
        }

        public async Task<MarketplaceRequestDto?> CreateRequestAsync(int tenantId, CreateMarketplaceRequestDto dto)
        {
            var r = new MarketplaceRequest
            {
                TenantId = tenantId,
                RequiredQty = dto.RequiredQty,
                VehicleTypeRequired = dto.VehicleTypeRequired,
                ScheduledStart = dto.ScheduledStart.ToUniversalTime(),
                ScheduledEnd = dto.ScheduledEnd.ToUniversalTime(),
                TargetPrice = dto.TargetPrice,
                Status = "Open"
            };

            await _context.MarketplaceRequests.AddAsync(r);
            await _context.SaveChangesAsync();

            var tenant = await _context.Tenants.FindAsync(tenantId);

            return new MarketplaceRequestDto
            {
                Id = r.Id,
                TenantId = r.TenantId,
                BuyerCompanyName = tenant?.CompanyName ?? "Your Company",
                RequiredQty = r.RequiredQty,
                VehicleTypeRequired = r.VehicleTypeRequired,
                ScheduledStart = r.ScheduledStart,
                ScheduledEnd = r.ScheduledEnd,
                TargetPrice = r.TargetPrice,
                Status = r.Status
            };
        }

        public async Task<IEnumerable<MarketplaceOfferDto>> GetOffersAsync(int tenantId)
        {
            // View bids relevant to user tenant's requests or user tenant's submitted bids
            return await _context.MarketplaceOffers
                .IgnoreQueryFilters()
                .Include(o => o.MarketplaceRequest)
                .Where(o => o.MarketplaceRequest.TenantId == tenantId || o.TenantId == tenantId)
                .Select(o => new MarketplaceOfferDto
                {
                    Id = o.Id,
                    TenantId = o.TenantId,
                    BuyerTenantId = o.MarketplaceRequest.TenantId,
                    SellerCompanyName = _context.Tenants.FirstOrDefault(t => t.Id == o.TenantId)!.CompanyName,
                    MarketplaceRequestId = o.MarketplaceRequestId,
                    OfferPrice = o.OfferPrice,
                    QuantityOffered = o.QuantityOffered,
                    Status = o.Status
                })
                .ToListAsync();
        }

        public async Task<MarketplaceOfferDto?> CreateOfferAsync(int tenantId, CreateMarketplaceOfferDto dto)
        {
            var req = await _context.MarketplaceRequests.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == dto.MarketplaceRequestId);
            if (req == null) return null;

            var offer = new MarketplaceOffer
            {
                TenantId = tenantId,
                MarketplaceRequestId = dto.MarketplaceRequestId,
                OfferPrice = dto.OfferPrice,
                QuantityOffered = dto.QuantityOffered,
                Status = "Pending"
            };

            await _context.MarketplaceOffers.AddAsync(offer);
            await _context.SaveChangesAsync();

            var tenant = await _context.Tenants.FindAsync(tenantId);

            return new MarketplaceOfferDto
            {
                Id = offer.Id,
                TenantId = offer.TenantId,
                SellerCompanyName = tenant?.CompanyName ?? "Your Company",
                MarketplaceRequestId = offer.MarketplaceRequestId,
                OfferPrice = offer.OfferPrice,
                QuantityOffered = offer.QuantityOffered,
                Status = offer.Status
            };
        }

        public async Task<bool> AcceptOfferAsync(int offerId, int tenantId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var offer = await _context.MarketplaceOffers
                    .IgnoreQueryFilters()
                    .Include(o => o.MarketplaceRequest)
                    .FirstOrDefaultAsync(o => o.Id == offerId);

                if (offer == null || offer.MarketplaceRequest.TenantId != tenantId) return false;

                offer.Status = "Accepted";

                // Close request
                offer.MarketplaceRequest.Status = "Fulfilled";

                // Reject other offers for this request
                var otherOffers = await _context.MarketplaceOffers
                    .IgnoreQueryFilters()
                    .Where(o => o.MarketplaceRequestId == offer.MarketplaceRequestId && o.Id != offer.Id)
                    .ToListAsync();

                foreach (var oo in otherOffers)
                {
                    oo.Status = "Rejected";
                }

                // Create assignments
                var assignment = new MarketplaceAssignment
                {
                    TenantId = tenantId,
                    MarketplaceOfferId = offer.Id,
                    ProviderCompanyId = offer.TenantId,
                    CarId = null,
                    DriverId = null,
                    PlateNumber = "Pending Allocation",
                    DriverName = "Pending Allocation",
                    DriverPhone = "Pending Allocation",
                    Price = offer.OfferPrice,
                    Status = "Assigned",
                    SettlementStatus = "Pending"
                };
                await _context.MarketplaceAssignments.AddAsync(assignment);

                // Create Marketplace double transaction entry
                var commission = offer.OfferPrice * 0.05m; // 5% SaaS platform fee
                var refId = $"BID-{offer.Id}";

                // Buyer side debit
                await _context.MarketplaceTransactions.AddAsync(new MarketplaceTransaction
                {
                    TenantId = tenantId,
                    BuyerTenantId = tenantId,
                    SellerTenantId = offer.TenantId,
                    Amount = offer.OfferPrice,
                    TransactionType = $"Settlement [{refId}]",
                    Status = "Pending"
                });

                // Seller side credit
                await _context.MarketplaceTransactions.AddAsync(new MarketplaceTransaction
                {
                    TenantId = offer.TenantId,
                    BuyerTenantId = tenantId,
                    SellerTenantId = offer.TenantId,
                    Amount = offer.OfferPrice - commission,
                    TransactionType = $"Settlement [{refId}]",
                    Status = "Pending"
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> AssignVehicleAsync(int assignmentId, int providerTenantId, AssignMarketplaceVehicleDto dto)
        {
            var assignment = await _context.MarketplaceAssignments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.Id == assignmentId && a.ProviderCompanyId == providerTenantId);

            if (assignment == null) return false;

            var car = await _context.Cars.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == dto.CarId && c.TenantId == providerTenantId && !c.IsDeleted);
            
            var driver = await _context.Drivers.IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == dto.DriverId && d.TenantId == providerTenantId && !d.IsDeleted);

            if (car != null)
            {
                assignment.CarId = car.Id;
                assignment.PlateNumber = car.PlateNumber;
            }

            if (driver != null)
            {
                assignment.DriverId = driver.Id;
                assignment.DriverName = $"{driver.FirstName} {driver.LastName}";
                assignment.DriverPhone = driver.Phone;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MarketplaceAssignmentDto>> GetAssignmentsAsync(int tenantId)
        {
            return await _context.MarketplaceAssignments
                .IgnoreQueryFilters()
                .Where(a => a.TenantId == tenantId || a.ProviderCompanyId == tenantId)
                .Select(a => new MarketplaceAssignmentDto
                {
                    Id = a.Id,
                    TenantId = a.TenantId,
                    MarketplaceOfferId = a.MarketplaceOfferId,
                    ProviderCompanyId = a.ProviderCompanyId,
                    ProviderCompanyName = _context.Tenants.FirstOrDefault(t => t.Id == a.ProviderCompanyId)!.CompanyName,
                    PlateNumber = a.PlateNumber,
                    DriverName = a.DriverName,
                    DriverPhone = a.DriverPhone,
                    Price = a.Price,
                    Status = a.Status,
                    SettlementStatus = a.SettlementStatus
                })
                .ToListAsync();
        }

        public async Task<PagedMarketplaceTransactionResponseDto> GetTransactionsAsync(int tenantId, int page = 1, int pageSize = 7)
        {
            var query = _context.MarketplaceTransactions
                .Where(t => t.TenantId == tenantId)
                .OrderByDescending(t => t.TransactionDate);

            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new MarketplaceTransactionDto
                {
                    Id = t.Id,
                    BuyerTenantId = t.BuyerTenantId,
                    BuyerCompanyName = _context.Tenants.FirstOrDefault(x => x.Id == t.BuyerTenantId)!.CompanyName,
                    SellerTenantId = t.SellerTenantId,
                    SellerCompanyName = _context.Tenants.FirstOrDefault(x => x.Id == t.SellerTenantId)!.CompanyName,
                    Amount = t.Amount,
                    TransactionType = t.TransactionType,
                    TransactionDate = t.TransactionDate,
                    Status = t.Status
                })
                .ToListAsync();

            return new PagedMarketplaceTransactionResponseDto
            {
                Data = data,
                TotalCount = totalCount
            };
        }

        public async Task<MarketplaceTransactionDto?> CreateTransactionAsync(int tenantId, CreateMarketplaceTransactionDto dto)
        {
            var assignment = await _context.MarketplaceAssignments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.Id == dto.MarketplaceAssignmentId);

            if (assignment == null) return null;

            var originalRefId = $"BID-{assignment.MarketplaceOfferId}";
            var originalTxType = $"Settlement [{originalRefId}]";

            var existingTxs = await _context.MarketplaceTransactions
                .IgnoreQueryFilters()
                .Where(t => t.TransactionType == originalTxType)
                .ToListAsync();

            MarketplaceTransaction? returnTx = null;

            if (existingTxs.Any())
            {
                foreach (var tx in existingTxs)
                {
                    tx.Status = "Completed";
                    tx.TransactionDate = DateTime.UtcNow;

                    if (tx.TenantId == tx.BuyerTenantId)
                    {
                        tx.Amount = dto.Amount;
                    }
                    else
                    {
                        var commission = dto.Amount * 0.05m;
                        tx.Amount = dto.Amount - commission;
                    }

                    if (tx.TenantId == tenantId)
                    {
                        returnTx = tx;
                    }
                }
                returnTx ??= existingTxs.First();
            }
            else
            {
                // Fallback if existing transactions were not found
                var refId = $"B2B-{assignment.Id}";
                var t1 = new MarketplaceTransaction
                {
                    TenantId = tenantId,
                    BuyerTenantId = tenantId,
                    SellerTenantId = assignment.ProviderCompanyId,
                    Amount = dto.Amount,
                    TransactionType = $"Settlement [{refId}]",
                    Status = "Completed",
                    TransactionDate = DateTime.UtcNow
                };
                await _context.MarketplaceTransactions.AddAsync(t1);

                var t2 = new MarketplaceTransaction
                {
                    TenantId = assignment.ProviderCompanyId,
                    BuyerTenantId = tenantId,
                    SellerTenantId = assignment.ProviderCompanyId,
                    Amount = dto.Amount - (dto.Amount * 0.05m),
                    TransactionType = $"Settlement [{refId}]",
                    Status = "Completed",
                    TransactionDate = DateTime.UtcNow
                };
                await _context.MarketplaceTransactions.AddAsync(t2);

                returnTx = t1;
            }

            assignment.SettlementStatus = "Completed";
            
            await _context.SaveChangesAsync();

            return new MarketplaceTransactionDto
            {
                Id = returnTx.Id,
                BuyerTenantId = returnTx.BuyerTenantId,
                BuyerCompanyName = _context.Tenants.FirstOrDefault(x => x.Id == returnTx.BuyerTenantId)?.CompanyName ?? "",
                SellerTenantId = returnTx.SellerTenantId,
                SellerCompanyName = _context.Tenants.FirstOrDefault(x => x.Id == returnTx.SellerTenantId)?.CompanyName ?? "",
                Amount = returnTx.Amount,
                TransactionType = returnTx.TransactionType,
                TransactionDate = returnTx.TransactionDate,
                Status = returnTx.Status
            };
        }
    }
}
