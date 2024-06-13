using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class SplitTicketOptionRepository : Repository<SplitTicketOption>, ISplitTicketOptionRepository
    {
        public SplitTicketOptionRepository(TP_DbContext dbContext) : base(dbContext){}
        public async Task<bool> IsSplitTicketOptionExist(byte optionId)
        {
            return await GetAll().Select(x => x.Id).AnyAsync(x => x == optionId);
        }
    }
}
