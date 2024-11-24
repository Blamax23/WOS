using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using WOS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using WOS.Dal.Interfaces;

namespace WOS.Front.Services
{
    public class AuthenticationSrv : IAuthenticationSrv
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public AuthenticationSrv(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<ClaimsPrincipal> LoginAccountClient(Client client)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, client.Nom),
            new Claim(ClaimTypes.Email, client.Email),
            new Claim(ClaimTypes.Role, "Client")
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            var principal = new ClaimsPrincipal(identity);

            return principal;
         }

        public async Task<ClaimsPrincipal> LoginAccountAdmin(Admin admin)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, admin.Nom),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.Role, "Admin")
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            var principal = new ClaimsPrincipal(identity);

            // Utilisation de IHttpContextAccessor pour accéder à HttpContext et signer l'utilisateur
            return principal;
        }

        public void SendEmail(string email, string subject, string body)
        {
            string fromMail = _configuration.GetSection("EmailSettings")["EmailSender"];
            string fromPassword = _configuration.GetSection("EmailSettings")["Password"];
            string toMail = _configuration.GetSection("EmailSettings")["EmailReceiver"];
            string smtpServer = _configuration.GetSection("EmailSettings")["Host"];

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromMail);
            mail.To.Add(new MailAddress(email));
            mail.Subject = subject;
            mail.Body = $"<html><body>{body}</body></html>";
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient(smtpServer)
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true
            };

            smtp.Send(mail);
        }
    }
}
