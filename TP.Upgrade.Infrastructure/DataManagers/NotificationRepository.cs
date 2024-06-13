using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(TP_DbContext dbContext) : base(dbContext){}
    }
}
