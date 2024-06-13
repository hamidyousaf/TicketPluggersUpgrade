using AutoMapper;
using AutoMapper.QueryableExtensions;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class SpecificationAttributeOptionRepository : Repository<SpecificationAttributeOption>, ISpecificationAttributeOptionRepository
    {
        private readonly TP_DbContext _dbContext;
        private readonly IMapper _mapper;
        public SpecificationAttributeOptionRepository(
            TP_DbContext dbContext,
            IMapper mapper
            ) : base(dbContext)
        {
            _mapper = mapper;
        }
        public IQueryable<SpecificationAttributeOptionLite> GetSpecificationAttributeOptionsBySpecificationAttributeId(byte specificationAttributeId)
        {
            return GetAll()
                .Where(x => x.SpecificationAttributeId == specificationAttributeId)
                .ProjectTo<SpecificationAttributeOptionLite>(_mapper.ConfigurationProvider);
        }
    }
}
