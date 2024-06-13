using AutoMapper;
using System.Collections.Generic;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class SearchKeyRepository : Repository<SearchKey>, ISearchKeyRepository
    {
        public SearchKeyRepository(
            TP_DbContext _dbContext) : base(_dbContext){}

        public IQueryable<SearchKey> GetByKey(string key)
        {
            return GetWithCondition(x => x.Keyword == key);
        }

        public IQueryable<SearchKey> GetTopSearchKeyList(GetTopSearchKeyListRequest request)
        {
            var skip = (request.Page - 1) * request.Limit;
            return GetReadOnlyList()
                .OrderByDescending(x => x.Count)
                .Skip(skip)
                .Take(request.Limit);
        }
    }
}
