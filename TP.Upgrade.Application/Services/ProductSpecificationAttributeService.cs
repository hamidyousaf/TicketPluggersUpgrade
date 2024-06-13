using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class ProductSpecificationAttributeService : IProductSpecificationAttributeService
    {
        private readonly IProductSpecificationAttributeRepository _productSpecificationAttributeRepository;

        public ProductSpecificationAttributeService(IProductSpecificationAttributeRepository productSpecificationAttributeRepository)
        {
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
        }
        public async Task<List<ProductSpecificationAttribute>> GetProductSpecificationAttributes(long productId = 0,
            int specificationAttributeOptionId = 0)
        {
            var query = _productSpecificationAttributeRepository.GetReadOnlyList();
            if (productId > 0)
                query = query.Where(psa => psa.ProductId == productId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);

            return await query.ToListAsync();
        }
    }
}
