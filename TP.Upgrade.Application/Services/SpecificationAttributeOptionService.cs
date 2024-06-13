using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class SpecificationAttributeOptionService : ISpecificationAttributeOptionService
    {
        private readonly ISpecificationAttributeOptionRepository _attributeOptionRepository;
        public SpecificationAttributeOptionService(ISpecificationAttributeOptionRepository attributeOptionRepository)
        {
            _attributeOptionRepository= attributeOptionRepository;
        }

        public async Task<List<SpecificationAttributeOptionLite>> GetSpecificationAttributeOptionsBySpecificationAttributeId(byte specificationAttributeId)
        {
            return await _attributeOptionRepository
                .GetSpecificationAttributeOptionsBySpecificationAttributeId(specificationAttributeId)
                .ToListAsync();
        }
    }
}
