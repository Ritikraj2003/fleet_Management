using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class MarketplaceController : BaseApiController
    {
        private readonly IMarketplaceRepository _marketplaceRepository;

        public MarketplaceController(IMarketplaceRepository marketplaceRepository)
        {
            _marketplaceRepository = marketplaceRepository;
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests()
        {
            var requests = await _marketplaceRepository.GetRequestsAsync(GetTenantId());
            return Ok(requests);
        }

        [HttpPost("requests")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateMarketplaceRequestDto dto)
        {
            var result = await _marketplaceRepository.CreateRequestAsync(GetTenantId(), dto);
            if (result == null) return BadRequest("Could not post marketplace request.");
            return Ok(result);
        }

        [HttpGet("offers")]
        public async Task<IActionResult> GetOffers()
        {
            var offers = await _marketplaceRepository.GetOffersAsync(GetTenantId());
            return Ok(offers);
        }

        [HttpPost("offers")]
        public async Task<IActionResult> CreateOffer([FromBody] CreateMarketplaceOfferDto dto)
        {
            var result = await _marketplaceRepository.CreateOfferAsync(GetTenantId(), dto);
            if (result == null) return BadRequest("Could not submit offer/bid.");
            return Ok(result);
        }

        [HttpPost("offers/{offerId}/accept")]
        public async Task<IActionResult> AcceptOffer(int offerId)
        {
            var success = await _marketplaceRepository.AcceptOfferAsync(offerId, GetTenantId());
            if (!success) return BadRequest("Could not accept bid. Verify requester company match.");
            return Ok(new { message = "Bid accepted. Assignment and transaction generated." });
        }

        [HttpGet("assignments")]
        public async Task<IActionResult> GetAssignments()
        {
            var assignments = await _marketplaceRepository.GetAssignmentsAsync(GetTenantId());
            return Ok(assignments);
        }

        [HttpPut("assignments/{id}/assign-vehicle")]
        public async Task<IActionResult> AssignVehicle(int id, [FromBody] AssignMarketplaceVehicleDto dto)
        {
            var success = await _marketplaceRepository.AssignVehicleAsync(id, GetTenantId(), dto);
            if (!success) return BadRequest("Could not assign vehicle/driver. Verify ownership.");
            return Ok(new { message = "Vehicle and driver assigned to marketplace assignment successfully." });
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 7)
        {
            var result = await _marketplaceRepository.GetTransactionsAsync(GetTenantId(), page, pageSize);
            return Ok(result);
        }

        [HttpPost("transactions")]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateMarketplaceTransactionDto dto)
        {
            var result = await _marketplaceRepository.CreateTransactionAsync(GetTenantId(), dto);
            if (result == null) return BadRequest("Could not generate transaction.");
            return Ok(result);
        }
    }
}
