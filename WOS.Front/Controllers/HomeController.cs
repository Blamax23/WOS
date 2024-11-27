using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using WOS.Front.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using WOS.Model;
using WOS.Dal.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WOS.Front.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClientSrv _clientSrv;

        public HomeController(ILogger<HomeController> logger, IClientSrv clientSrv)
        {
            _logger = logger;
            _clientSrv = clientSrv;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetString("ReturnUrl", HttpContext.Request.Path);
            List<Client> clients = _clientSrv.GetAllClients();
            return View(clients);
        }

        [HttpPost]
        public IActionResult AddClient(string nom, string prenom, string email, string password)
        {
            Client client = new Client
            {
                Nom = nom,
                Prenom = prenom,
                Email = email,
                MotDePasse = HashPassword(password)
            };
            _clientSrv.AddClient(client);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
