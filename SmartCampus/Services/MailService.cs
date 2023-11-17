using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SmartCampus.Services
{
    public class MailService : IMailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var mail = "faysalahmed2235@gmail.com"; // Your Gmail email address
            var pw = "cbriskianbcikykh"; // Your Gmail password

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, pw)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(mail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true // Set this to true if your message contains HTML
            };
            mailMessage.To.Add(toEmail);

            return client.SendMailAsync(mailMessage);
        }

    }
}
