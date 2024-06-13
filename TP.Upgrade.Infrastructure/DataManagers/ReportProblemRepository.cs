using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public class ReportProblemRepository : Repository<ReportProblem>, IReportProblemRepository
    {
        public ReportProblemRepository(TP_DbContext dbContext) : base(dbContext){}
        public async Task<bool> InsertReportProblem(ReportProblem report, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return false;

            var result = await Add(report);

            if (result is 0)
            {
                return false;
            }

            return true;
        }
    }
}
