using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<BookingDto>> GetBookingsAsync(int tenantId);
        Task<BookingDto?> GetBookingByIdAsync(int id, int tenantId);
        Task<BookingDto?> CreateBookingAsync(int tenantId, CreateBookingDto dto);
        Task<bool> StartBookingAsync(int id, int tenantId);
        Task<bool> CompleteBookingAsync(int id, int tenantId, CompleteBookingDto dto = null);
        Task<bool> CancelBookingAsync(int id, int tenantId);
        Task<PagedBookingResponseDto> GetBookingHistoryAsync(int tenantId, int page = 1, int pageSize = 10, string search = "");
    }
}
