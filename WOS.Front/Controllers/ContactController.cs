using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using WOS.Dal.Context;

namespace WOS.Front.Controllers
{
    [Route("[controller]")]
    public class ContactController : Controller
    {
        private readonly WOSDbContext _context;
        private readonly IConfiguration _configuration;

        public ContactController(WOSDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        // GET: ContactController
        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string Email, string Objet, string Contenu)
        {
            try
            {
                string fromMail = _configuration.GetSection("EmailSettings")["EmailSender"];
                string fromPassword = _configuration.GetSection("EmailSettings")["Password"];
                string toMail = _configuration.GetSection("EmailSettings")["EmailReceiver"];
                string smtpServer = _configuration.GetSection("EmailSettings")["Host"];

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromMail);
                mail.To.Add(new MailAddress(toMail));
                mail.Subject = Objet;
                mail.Body = $"<html><body>De : {Email}<br><br>{Contenu}</body></html>";
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(smtpServer)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true
                };

                smtp.Send(mail);
                // Rediriger l'utilisateur vers une page de succès
                return RedirectToAction("SuccessPage", "Contact");
            }
            catch (Exception ex)
            {
                // Gérer les erreurs éventuelles
                ModelState.AddModelError("", "Une erreur s'est produite lors de l'envoi du message : " + ex.Message);
                return View("Contact", new { Email, Objet, Contenu });
            }
        }

        [HttpGet]
        [Route("success")]
        public IActionResult SuccessPage()
        {
            return View();
        }
    }
}
