using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface ISpecificationAttributeOptionService
    {
        Task<List<SpecificationAttributeOptionLite>> GetSpecificationAttributeOptionsBySpecificationAttributeId(byte specificationAttributeId);
    }
}
