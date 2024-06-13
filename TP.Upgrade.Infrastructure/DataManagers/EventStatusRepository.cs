using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class EventStatusRepository : Repository<EventStatus> ,IEventStatusRepository
    {
        public EventStatusRepository(TP_DbContext _dbContext) : base(_dbContext){}
    }
}
