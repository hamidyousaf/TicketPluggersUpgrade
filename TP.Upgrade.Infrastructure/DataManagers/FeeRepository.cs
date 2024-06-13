using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class FeeRepository : Repository<PlatformFee>, IFeeRepository
    {
        public FeeRepository(TP_DbContext dbContext) : base(dbContext){}
    }
}
