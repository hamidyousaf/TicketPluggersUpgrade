using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class SearchKeyService : ISearchKeyService
    {
        private readonly ISearchKeyRepository _searchKeyRepository;
        public SearchKeyService(
            ISearchKeyRepository searchKeyRepository) 
        {
            _searchKeyRepository = searchKeyRepository;
        }
        public async Task AddOrUpdateSearchKey(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return;
            searchText = searchText.Trim();
            var record = await _searchKeyRepository.GetByKey(searchText).FirstOrDefaultAsync();
            if (record != null)
            {
                //update
                record.Count = record.Count + 1;
                await _searchKeyRepository.Save();
            }
            else
            {
                // add
                SearchKey searchKey = new SearchKey();
                searchKey.Keyword = searchText;
                searchKey.Count = 1;
                await _searchKeyRepository.Add(searchKey);
            }
        }

        public async Task<ResponseModel> GetTopSearchKeyList(GetTopSearchKeyListRequest request, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get data.
            var data = await _searchKeyRepository.GetTopSearchKeyList(request).ToListAsync(ct);
            return new ResponseModel()
            {
                Data = data,
                IsSuccess = true
            };
        }
        public async Task<string[]> GetTop10SearchKeys(CancellationToken ct)
        {
            var keyList = await _searchKeyRepository.GetReadOnlyList().OrderByDescending(x => x.Count).Select(x => x.Keyword).Take(10).ToArrayAsync(ct);
            return keyList;
        }
    }
}
