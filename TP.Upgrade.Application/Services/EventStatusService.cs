using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;

namespace TP.Upgrade.Application.Services
{
    public sealed class EventStatusService : IEventStatusService
    {
        private readonly IEventStatusRepository _eventStatusRepository;
        private readonly IMapper _mapper;
        public EventStatusService(
            IEventStatusRepository eventStatusRepository,
            IMapper mapper)
        {
            _eventStatusRepository = eventStatusRepository;
            _mapper = mapper;
        }
        public async Task<bool> IsEventStatusExist(byte eventStatusId)
        {
            return await _eventStatusRepository.GetAll().AnyAsync(x => x.Id == eventStatusId);
        }
        public async Task<List<EventStatusDto>> GetEventStatuses()
        {
            return await _eventStatusRepository
                .GetAll()
                .ProjectTo<EventStatusDto>(_mapper.ConfigurationProvider)    // This is use to prevent to get extra columns.
                .ToListAsync();
        }
    }
}
