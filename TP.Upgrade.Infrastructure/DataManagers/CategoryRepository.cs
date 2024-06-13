using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly IMapper _mapper;
        public CategoryRepository(
            TP_DbContext _dbContext,
            IMapper mapper) : base(_dbContext)
        {
            _mapper = mapper;
        }
        public async Task<bool> IsSegmentExist(short segmentId)
        {
            return await GetAll()
                .AnyAsync(x => x.Id == segmentId && x.ChildLevel == 0);
        }
        public async Task<bool> IsGenreExist(short segmentId, short genreId)
        {
            return await GetAll()
                .AnyAsync(x => x.Id == genreId && x.ChildLevel == 1 && x.ParentCategoryId == segmentId);
        }
        public async Task<bool> IsSubGenreExist(short genreId, short subGenreId)
        {
            return await GetAll()
                .AnyAsync(x => x.Id == subGenreId && x.ChildLevel == 1 && x.ParentCategoryId == genreId);
        }
        public async Task<List<CategoryDto>> GetGenresBySegmentId(short segmentId)
        {
            return await GetAll()
                .Where(x => x.ChildLevel == 1 && x.ParentCategoryId == segmentId)
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        public async Task<List<CategoryDto>> GetSubGenresByGenreId(short genreId)
        {
            return await GetAll()
                .Where(x => x.ChildLevel == 2 && x.ParentCategoryId == genreId)
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public IQueryable<CategoryDto> GetCategories()
        {
            return GetReadOnlyList()
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider);
        }
    }
}