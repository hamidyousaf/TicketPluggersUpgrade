using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;

namespace TP.Upgrade.Application.Services
{
    public sealed class SplitTicketOptionService : ISplitTicketOptionService
    {
        private readonly ISplitTicketOptionRepository _splitTicketOptionRepository;
        public SplitTicketOptionService(ISplitTicketOptionRepository splitTicketOptionRepository)
        {
            _splitTicketOptionRepository = splitTicketOptionRepository;
        }
        public async Task<bool> IsSplitTicketOptionExist(byte optionId)
        {
            return await _splitTicketOptionRepository.IsSplitTicketOptionExist(optionId);
        }
    }
}
