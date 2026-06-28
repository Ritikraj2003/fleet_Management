using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData([FromQuery] System.DateTime? startDate = null, [FromQuery] System.DateTime? endDate = null)
        {
            if (User.IsInRole("SUPER_ADMIN"))
            {
                var saData = await _dashboardRepository.GetSuperAdminDashboardAsync();
                return Ok(saData);
            }

            var tenantData = await _dashboardRepository.GetTenantDashboardAsync(GetTenantId(), startDate, endDate);
            return Ok(tenantData);
        }
    }
}
