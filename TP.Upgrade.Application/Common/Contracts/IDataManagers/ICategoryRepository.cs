using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<bool> IsSegmentExist(short segmentId);
        Task<bool> IsGenreExist(short segmentId, short genreId);
        Task<bool> IsSubGenreExist(short genreId, short subGenreId);
        Task<List<CategoryDto>> GetGenresBySegmentId(short segmentId);
        Task<List<CategoryDto>> GetSubGenresByGenreId(short genreId);
        IQueryable<CategoryDto> GetCategories();
    }
}
