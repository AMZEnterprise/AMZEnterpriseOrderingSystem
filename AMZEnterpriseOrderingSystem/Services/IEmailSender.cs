using System.Threading.Tasks;

namespace AMZEnterpriseOrderingSystem.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
