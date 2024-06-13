using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface ISpecificationAttributeOptionRepository : IRepository<SpecificationAttributeOption>
    {
        IQueryable<SpecificationAttributeOptionLite> GetSpecificationAttributeOptionsBySpecificationAttributeId(byte specificationAttributeId);
    }
}
