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
        private readonly IProduitSrv _produitSrv;
        private readonly IMarqueSrv _marqueSrv;
        private readonly ICategorieSrv _categorieSrv;
        private readonly IGlobalDataSrv _globalDataSrv;

        public HomeController(ILogger<HomeController> logger, IClientSrv clientSrv, IProduitSrv produitSrv, IMarqueSrv marqueSrv, ICategorieSrv categorieSrv, IGlobalDataSrv globalDataSrv)
        {
            _logger = logger;
            _clientSrv = clientSrv;
            _produitSrv = produitSrv;
            _marqueSrv = marqueSrv;
            _categorieSrv = categorieSrv;
            _globalDataSrv = globalDataSrv;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetString("ReturnUrl", HttpContext.Request.Path);

            List<Produit> allProducts = _globalDataSrv.Produits;

            HomeViewModel homeViewModel = new HomeViewModel();

            var random = new Random();
            var randomList = allProducts.Where(p => p.IsTendance)
                                        .OrderBy(p => random.Next())
                                        .ToList();

            RowHomeModel rowHomeModelTendance = new RowHomeModel()
            {
                Name = "Tendances",
                Produits = randomList
            };

            homeViewModel.RowHome.Add(rowHomeModelTendance);

            List<Categorie> catTendances = _categorieSrv.GetCategoriesByHome();

            foreach (var cat in catTendances)
            {
                RowHomeModel rowHomeModel = new RowHomeModel()
                {
                    Name = cat.Nom,
                    Produits = allProducts.Where(p => p.CategorieId == cat.Id).ToList()
                };

                homeViewModel.RowHome.Add(rowHomeModel);
            }

            List<Marque> marquesTendances = _marqueSrv.GetMarquesByHome();

            foreach(var marque in marquesTendances)
            {
                RowHomeModel rowHomeModel = new RowHomeModel()
                {
                    Name = marque.Nom,
                    Produits = allProducts.Where(p => p.MarqueId == marque.Id).ToList()
                };

                homeViewModel.RowHome.Add(rowHomeModel);
            }

            return View(homeViewModel);
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

        [Route("erreur")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(/*new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }*/);
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
