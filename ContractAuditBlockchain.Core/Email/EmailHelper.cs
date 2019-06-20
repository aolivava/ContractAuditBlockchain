using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.Core.Email
{
    public class EmailHelper
    {
        public string From { get; set; }
        public string FromName { get; set; }

        public IEnumerable<string> To { get; set; }
        public IEnumerable<string> ToName { get; set; }

        public string Bcc { get; set; }
        public string BccName { get; set; }
        public string Cc { get; set; }
        public string CcName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool UseAlternateViews { get; set; }
        public AlternateView AlternateViewPlain { get; set; }
        public AlternateView AlternateViewHtml { get; set; }

        public IEnumerable<Attachment> Attachments { get; set; }

        public Task SendMailAsync()
        {
            using (MailMessage mailMessage = CreateMessage())
            using (var mSmtpClient = new SmtpClient())
            {
                // Send async "doesn't work"
                //return  mSmtpClient.SendMailAsync(mailMessage);
                mSmtpClient.Send(mailMessage);
                return Task.FromResult(0);
            }
        }

        public void SendMail(string exceptionMsg)
        {
            MailMessage mailMessage = CreateMessage();
            SmtpClient mSmtpClient = new SmtpClient();
            try
            {
                mSmtpClient.Send(mailMessage);
            }
            catch (SmtpException e)
            {
                throw new Exception(exceptionMsg, e);
            }
            finally
            {
                mSmtpClient.Dispose();
                mailMessage.Dispose();
            }
        }

        private MailMessage CreateMessage()
        {
            MailMessage mailMessage = new MailMessage();
            if (!string.IsNullOrEmpty(From))
                mailMessage.From = new MailAddress(From, FromName);

            CreateMailAddresses(mailMessage.To, To, ToName);

            if (!string.IsNullOrEmpty(Bcc))
                mailMessage.Bcc.Add(new MailAddress(Bcc, BccName));
            if (!string.IsNullOrEmpty(Cc))
                mailMessage.CC.Add(new MailAddress(Cc, CcName));

            mailMessage.Subject = Subject != null ? Subject.Replace("\r\n", " // ") : "";
            mailMessage.Body = Body;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.Normal;

            if (UseAlternateViews)
            {
                mailMessage.AlternateViews.Add(AlternateViewPlain);
                mailMessage.AlternateViews.Add(AlternateViewHtml);
            }

            if ((Attachments != null) && Attachments.Any())
            {
                foreach (var attachment in Attachments)
                {
                    mailMessage.Attachments.Add(attachment);
                }
            }

            return mailMessage;
        }

        void CreateMailAddresses(MailAddressCollection collection, IEnumerable<string> addresses, IEnumerable<string> names)
        {
            if (addresses != null)
            {
                if (names == null)
                {
                    names = new string[] { };
                }

                var addressEnum = addresses.GetEnumerator();
                var nameEnum = names.GetEnumerator();
                bool gotAddress = addressEnum.MoveNext();
                bool gotName = nameEnum.MoveNext();
                while (gotAddress)
                {
                    collection.Add(new MailAddress(addressEnum.Current, gotName ? nameEnum.Current : null));
                    gotAddress = addressEnum.MoveNext();
                    gotName = nameEnum.MoveNext();
                }
            }
        }
    }
}