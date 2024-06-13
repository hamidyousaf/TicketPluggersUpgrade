using Microsoft.EntityFrameworkCore;
using TicketServer.Helpers;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Services
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileHelper _fileHelper;
        public CategoryService(ICategoryRepository categoryRepository, IFileHelper fileHelper)
        {
            _categoryRepository = categoryRepository;
            _fileHelper = fileHelper;
        }
        public async Task<bool> IsGenreExist(short segmentId, short genreId)
        {
            return await _categoryRepository.IsGenreExist(segmentId, genreId);
        }
        public async Task<bool> IsSegmentExist(short segmentId)
        {
            return await _categoryRepository.IsSegmentExist(segmentId);
        }
        public async Task<bool> IsSubGenreExist(short genreId, short subGenreId)
        {
            return await _categoryRepository.IsSubGenreExist(genreId, subGenreId);
        }
        public async Task<List<CategoryDto>> GetGenresBySegmentId(short segmentId)
        {
            return await _categoryRepository.GetGenresBySegmentId(segmentId);
        }
        public async Task<List<CategoryDto>> GetSubGenresByGenreId(short genreId)
        {
            return await _categoryRepository.GetSubGenresByGenreId(genreId);
        }

        public async Task<ResponseModel> GetCategories(CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get categories.
            var categories = await _categoryRepository.GetCategories().ToListAsync(ct);

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = categories
            };
        }
        public async Task<string?> GetPoster()
        {
            var category = await _categoryRepository.GetReadOnlyList()
                .FirstOrDefaultAsync(x => x.ChildLevel == 2 && !x.IsDeleted && x.Published && x.IsPosterActive);

            if (category is null)
            {
                return null;
            }

            return category.ImageURL;
        }
        public async Task<ResponseModel> AddPoster(AddPosterRequest request, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Request has been cancalled."
            };

            // Get sub genre by id.
            var category = await _categoryRepository.GetAll()
                .SingleOrDefaultAsync(x => x.ChildLevel == 2 && !x.IsDeleted && x.Published && x.Id == request.SubGenreId, ct);

            if (category is null)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = $"Genre not exist with id {request.SubGenreId}."
                };
            }

            var categories = await _categoryRepository.GetAll().Where(x => x.ChildLevel == 2 && !x.IsDeleted && x.Published && x.IsPosterActive).ToListAsync(ct);
            if (categories.Count > 0)
            {
                categories.ForEach(x => x.IsPosterActive = false);
            }

            // add poster.
            category.ImageURL = await _fileHelper.UploadFile(@"static//posters", request.File);
            category.IsPosterActive = true;
            await _categoryRepository.Change(category);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Poster added successfully."
            };
        }
    }
}