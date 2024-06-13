using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface ICategoryService
    {
        Task<bool> IsSegmentExist(short segmentId);
        Task<bool> IsGenreExist(short segmentId, short genreId);
        Task<bool> IsSubGenreExist(short GenreId, short SubGenreId);
        Task<List<CategoryDto>> GetGenresBySegmentId(short segmentId);
        Task<List<CategoryDto>> GetSubGenresByGenreId(short genreId);
        Task<ResponseModel> GetCategories(CancellationToken ct = default);
        Task<string?> GetPoster();
        Task<ResponseModel> AddPoster(AddPosterRequest request, CancellationToken ct = default);
    }
}
