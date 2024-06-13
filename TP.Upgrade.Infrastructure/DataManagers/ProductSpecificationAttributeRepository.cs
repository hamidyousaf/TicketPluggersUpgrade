using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public class ProductSpecificationAttributeRepository : Repository<ProductSpecificationAttribute>, IProductSpecificationAttributeRepository
    {
        public ProductSpecificationAttributeRepository(TP_DbContext dbContext) : base(dbContext){}
    }
}
