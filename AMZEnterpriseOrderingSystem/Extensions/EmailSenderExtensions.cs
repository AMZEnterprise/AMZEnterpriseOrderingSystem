using AMZEnterpriseOrderingSystem.Services;
using AMZEnterpriseOrderingSystem.Utility;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AMZEnterpriseOrderingSystem.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "تایید ایمیل شما",
                $"برای تایید ایمیل خود بر روی لینک کلیک کنید: <a href='{HtmlEncoder.Default.Encode(link)}'>لینک</a>");
        }

        public static Task SendOrderStatusAsync(this IEmailSender emailSender, string email,string orderNumber, string status)
        {
            string subject = "";
            string message = "";

            if(status==SD.StatusCancelled)
            {
                subject = "سفارش لغو شد";
                message = "سفارش شماره " + orderNumber + " لغو گردید. اگر مشکلی دارید با ما تماس بگیرید.";
            }
            if (status == SD.StatusSubmitted)
            {
                subject = "سفارش با موفقیت انجام شد";
                message = "سفارش شماره " + orderNumber + " با موفقیت انجام گردید.";
            }
            if (status == SD.StatusReady)
            {
                subject = "سفارش برای تحویل آماده است";
                message = "سفارش شماره " + orderNumber + " برای تحویل گرفتن آماده است. اگر مشکلی دارید با ما تماس بگیرید";
            }

            return emailSender.SendEmailAsync(email, subject, message);
        }
    }
}
