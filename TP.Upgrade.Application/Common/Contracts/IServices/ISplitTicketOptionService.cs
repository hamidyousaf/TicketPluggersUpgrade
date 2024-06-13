namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface ISplitTicketOptionService
    {
        Task<bool> IsSplitTicketOptionExist(byte optionId);
    }
}
