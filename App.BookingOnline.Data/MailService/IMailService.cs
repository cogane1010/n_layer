using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.BookingOnline.Data.MailService
{
    public interface IMailService
    {
        Task SendMailAsync(string To, string Cc, string Bcc, string Subject, string Content, string UserName, bool UseAsync = true, bool isHTML = true, List<string> attFiles = null);
    }
}