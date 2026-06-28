using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class BookingsController : BaseApiController
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _bookingRepository.GetBookingsAsync(GetTenantId());
            return Ok(bookings);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetBookingHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "")
        {
            var result = await _bookingRepository.GetBookingHistoryAsync(GetTenantId(), page, pageSize, search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id, GetTenantId());
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            var result = await _bookingRepository.CreateBookingAsync(GetTenantId(), dto);
            if (result == null) return BadRequest("Could not execute booking submission.");
            return Ok(result);
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartBooking(int id)
        {
            var success = await _bookingRepository.StartBookingAsync(id, GetTenantId());
            if (!success) return NotFound();
            return Ok(new { message = "Booking has started." });
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteBooking(int id, [FromBody] CompleteBookingDto dto = null)
        {
            var success = await _bookingRepository.CompleteBookingAsync(id, GetTenantId(), dto);
            if (!success) return NotFound();
            return Ok(new { message = "Booking marked as completed." });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var success = await _bookingRepository.CancelBookingAsync(id, GetTenantId());
            if (!success) return NotFound();
            return Ok(new { message = "Booking cancelled successfully." });
        }
    }
}
