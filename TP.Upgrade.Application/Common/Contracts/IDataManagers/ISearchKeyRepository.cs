using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface ISearchKeyRepository : IRepository<SearchKey>
    {
        IQueryable<SearchKey> GetByKey(string key);
        IQueryable<SearchKey> GetTopSearchKeyList(GetTopSearchKeyListRequest request);

    }
}
