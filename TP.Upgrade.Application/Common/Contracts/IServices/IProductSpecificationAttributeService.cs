using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface IProductSpecificationAttributeService
    {
        Task<List<ProductSpecificationAttribute>> GetProductSpecificationAttributes(long productId = 0,
            int specificationAttributeOptionId = 0);
    }
}
