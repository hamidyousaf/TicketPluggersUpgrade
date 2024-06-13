using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface IReportProblemService
    {
        Task<ResponseModel> ReportProblem(ReportProblemRequest request, CancellationToken ct = default);
        Task<ResponseModel> ReportProblemMutiple(List<long> orderIds, long customerId, CancellationToken ct = default);
    }
}
