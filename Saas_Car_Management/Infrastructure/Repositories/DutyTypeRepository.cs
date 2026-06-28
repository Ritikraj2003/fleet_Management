using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;

namespace Saas_Car_Management.Infrastructure.Repositories
{
    public class DutyTypeRepository : Repository<DutyType>, IDutyTypeRepository
    {
        public DutyTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
