using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface IReportProblemRepository : IRepository<ReportProblem>
    {
        Task<bool> InsertReportProblem(ReportProblem report, CancellationToken ct = default);
    }
}
