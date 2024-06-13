using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface ISearchKeyService
    {
        Task AddOrUpdateSearchKey(string searchText);
        Task<ResponseModel> GetTopSearchKeyList(GetTopSearchKeyListRequest request, CancellationToken ct = default);
        Task<string[]> GetTop10SearchKeys(CancellationToken ct = default);
    }
}
