namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}
