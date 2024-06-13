using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface ISplitTicketOptionRepository : IRepository<SplitTicketOption>
    {
        Task<bool> IsSplitTicketOptionExist(byte optionId);
    }
}
