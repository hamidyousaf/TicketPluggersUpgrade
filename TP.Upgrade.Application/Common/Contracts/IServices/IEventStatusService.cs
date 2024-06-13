using TP.Upgrade.Application.DTOs;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface IEventStatusService
    {
        Task<bool> IsEventStatusExist(byte eventStatusId);
        Task<List<EventStatusDto>> GetEventStatuses();
    }
}
