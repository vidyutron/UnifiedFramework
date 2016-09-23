using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedFrameWork.Controller
{
    public class SendMail
    {
        internal void CreateEmail(string mailFrom,List<string> mailToList,
            string smtpClient,string subjectLine,string mailBody,int portNumber,
            List<string> attachmentList,string credentialUser,string credentialPass)
        {
            try
            {
                MailMessage mail = new MailMessage();
                Attachment attachment;
                SmtpClient SmtpServer = new SmtpClient(smtpClient);
                mail.From = new MailAddress(mailFrom);
                foreach(string mailToSingle in mailToList)
                {
                    mail.To.Add(mailToSingle);
                }
                mail.Subject = subjectLine;
                mail.IsBodyHtml = true;
                mail.Body = mailBody;
                  
                foreach(string singleAttachment in attachmentList)
                {
                    attachment = new Attachment(singleAttachment);
                    mail.Attachments.Add(attachment);
                }
                SmtpServer.Port = portNumber;
                SmtpServer.Credentials = new System.Net.NetworkCredential(credentialUser, credentialPass);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
