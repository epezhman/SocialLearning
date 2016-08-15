using System;
using System.Net;
using System.Net.Mail;

namespace UT.SL.Helper
{
    public class MailUtils
    {
        
        public class SMTPServer
        {
            public string ServerUrl { get; set; }
            public int ServerPort { get; set; }
            public bool isSSL { get; set; }
        }

        public class RegisteredSMTPServers
        {
            public static SMTPServer Gmail { get { return new SMTPServer() { isSSL = true, ServerPort = 587, ServerUrl = "smtp.gmail.com" }; } }
            public static SMTPServer DoosMooc { get { return new SMTPServer() { isSSL = false, ServerPort = 587, ServerUrl = "mail.doosmooc.com" }; } }
        }

        public class EmailAccount
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class RegisteredMailAccounts
        {
            public static EmailAccount GmailDoosMooc { get { return new EmailAccount() { Email = "doosmooc@gmail.com", Password = "Master2012" }; } }
            public static EmailAccount DoosMooc { get { return new EmailAccount() { Email = "no_reply@doosmooc.com", Password = "Master2012#" }; } }
        }
        
        public static bool SendMailbyGmail(         
          string pTo,
          string pSubject,
          string pBody,
          bool isHtml = true,
          string pAttachmentPath = "")
        {
            return SendbySMTP(RegisteredSMTPServers.Gmail, RegisteredMailAccounts.GmailDoosMooc, pTo, pSubject, pBody, true, pAttachmentPath, System.Text.Encoding.UTF8);
        }

        public static bool SendMailByDoosMooc(
          string pTo,
          string pSubject,
          string pBody,
          bool isHtml = true,
          string pAttachmentPath = "")
        {
            return SendbySMTP(RegisteredSMTPServers.DoosMooc, RegisteredMailAccounts.DoosMooc, pTo, pSubject, pBody, isHtml, pAttachmentPath, System.Text.Encoding.UTF8);
        }

        public static bool SendbySMTP(
          SMTPServer smtpServer,
          EmailAccount accountInfo,
          string pTo,
          string pSubject,
          string pBody,
          bool isHtml,
          string pAttachmentPath,
          System.Text.Encoding encoding)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(accountInfo.Email);
                    mail.To.Add(pTo);
                    mail.Subject = pSubject;
                    mail.Body = pBody;
                    mail.IsBodyHtml = isHtml;
                    mail.BodyEncoding = encoding;

                    //mail.Attachments.Add(new Attachment("C:\\SomeFile.txt"));

                    using (SmtpClient smtp = new SmtpClient(smtpServer.ServerUrl, smtpServer.ServerPort))
                    {
                        smtp.EnableSsl = smtpServer.isSSL;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(accountInfo.Email, accountInfo.Password);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
