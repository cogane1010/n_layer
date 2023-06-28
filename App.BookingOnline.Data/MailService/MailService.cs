using App.BookingOnline.Data.Repositories;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App.BookingOnline.Data.MailService
{

    public class MailService : IMailService
    {
        private readonly ISettingRepository _settingRepo;
        private readonly ILogger _log;

        public MailService(ISettingRepository settingRepo, ILogger<MailService> logger)
        {
            _settingRepo = settingRepo;
            _log = logger;
        }

        public static Stream Base64ToImageStream(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            return ms;
        }

        // Xu lý base64 Image
        private static AlternateView ContentToAlternateView(string content)
        {
            var imgCount = 0;
            List<LinkedResource> resourceCollection = new List<LinkedResource>();
            foreach (Match m in Regex.Matches(content, "<img(?<value>.*?)>"))
            {
                imgCount++;
                var imgContent = m.Groups["value"].Value;
                string type = Regex.Match(imgContent, ":(?<type>.*?);base64,").Groups["type"].Value;
                string base64 = Regex.Match(imgContent, "base64,(?<base64>.*?)\"").Groups["base64"].Value;
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(base64))
                {
                    //ignore replacement when match normal <img> tag
                    continue;
                }
                var replacement = " src=\"cid:" + imgCount + "\"";
                content = content.Replace(imgContent, replacement);
                var tempResource = new LinkedResource(Base64ToImageStream(base64), new ContentType(type))
                {
                    ContentId = imgCount.ToString()
                };
                resourceCollection.Add(tempResource);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html);
            foreach (var item in resourceCollection)
            {
                alternateView.LinkedResources.Add(item);
            }

            return alternateView;
        }


        /// <summary>
        /// To, Cc, Bcc mail cách nhau dau ";"
        /// UserName: user thao tác
        /// </summary>
        /// <param name="To"></param>
        /// <param name="Cc"></param>
        /// <param name="Bcc"></param>
        /// <param name="Subject"></param>
        /// <param name="Content"></param>
        /// <param name="UserName"></param>
        public async Task SendMailAsync(string To, string Cc, string Bcc, string Subject, string Content, string UserName, bool UseAsync = true, bool isHTML = true, List<string> attachFilePaths = null)
        {
            UserName = UserName.Trim();

            string server = _settingRepo.GetSetting("mail_send_server");
            int port = int.Parse(_settingRepo.GetSetting("mail_send_port"));
            bool isSSL = _settingRepo.GetSetting("mail_send_isssl") == "true";

            string userName = _settingRepo.GetSetting("mail_send_username");
            string pass = _settingRepo.GetSetting("mail_send_userpass");

            var message = new MailMessage();

            message.From = new MailAddress(userName);

            var to = To.Split(';');
            foreach (var item in to)
            {
                message.To.Add(new MailAddress(item));
            }

            if (!string.IsNullOrWhiteSpace(Cc))
            {
                var cc = Cc.Split(';');
                foreach (var item in cc)
                {
                    message.CC.Add(new MailAddress(item));
                }
            }

            if (!string.IsNullOrWhiteSpace(Bcc))
            {
                var bcc = Bcc.Split(';');
                foreach (var item in bcc)
                {
                    message.Bcc.Add(new MailAddress(item));
                }
            }

            message.Subject = Subject;
            message.SubjectEncoding = Encoding.UTF8;

            message.IsBodyHtml = isHTML;
            // Xu ly anh trong mail
            AlternateView alterView = ContentToAlternateView(Content);
            message.AlternateViews.Add(alterView);

            message.Body = Content;
            message.BodyEncoding = Encoding.UTF8;

            if (attachFilePaths != null && attachFilePaths.Count > 0)
            {
                foreach (var filePath in attachFilePaths)
                {
                    if (File.Exists(filePath))
                        message.Attachments.Add(new Attachment(filePath));
                }
            }



            //var logSendMail = _logSendMailRepo.Add(new Re_LogSendMail()
            //{
            //    CreatedDate = DateTime.Now,
            //    CreatedUser = UserName,
            //    From = userName,
            //    To = To,
            //    Cc = Cc,
            //    Bcc = Bcc,
            //    Subject = Subject,
            //    Content = Content,
            //    Status = "sending"
            //});

            //var UserState = logSendMail.Id;
            var UserState = 1;



            if (UseAsync)
            {
                var client = new SmtpClient(server, port) { Credentials = new NetworkCredential(userName, pass) };
                client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);              
                _ = Task.Run(() => client.SendAsync(message, UserState));
                using (LogContext.PushProperty("MethodName", "SendMailAsync"))
                {
                    _log.LogInformation(To);
                    _log.LogInformation(Subject);
                }
            }
            else
            {
                using (var client = new SmtpClient(server, port) { Credentials = new NetworkCredential(userName, pass) })
                {
                    await client.SendMailAsync(message);

                    /*
                    sendMailCallbackDone = false;
                    sendMailErrorMessage = "";
                    client.Timeout = 30 * 1000;
                    await Task.Run(() => client.SendAsync(message, UserState));

                    int timeout = 60 * 1000;
                    Stopwatch st = new Stopwatch();
                    st.Start();
                    // ch? g?i mail xong ho?c quá 60s stop
                    while (!sendMailCallbackDone && st.ElapsedMilliseconds < timeout) { }
                    st.Stop();

                    if (!string.IsNullOrWhiteSpace(sendMailErrorMessage))
                    {
                        client.Dispose();
                        throw new Exception(sendMailErrorMessage);
                    }

                    if (st.ElapsedMilliseconds >= timeout)
                    {
                        client.Dispose();
                        throw new Exception("Send mail timeout");
                    }
                    */
                }
            }
        }

        private bool sendMailCallbackDone = false;
        private string sendMailErrorMessage = "";
        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                using (LogContext.PushProperty("MethodName", "SendCompletedCallback"))
                {
                    _log.LogError("Mail cancel");
                }
            }

            if (e.Error != null)
            {
                using (LogContext.PushProperty("MethodName", "SendCompletedCallback"))
                {
                    _log.LogError(e.Error.Message);
                    _log.LogError(e.Error.InnerException.Message);
                }
                sendMailErrorMessage = e.Error.InnerException != null ? e.Error.InnerException.Message : e.Error.Message;
            }

            sendMailCallbackDone = true;
        }

    }
}