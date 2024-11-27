using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WOS.Model;
using System.Text.Json;

namespace WOS.Front.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("AddProduct")]
        public ActionResult AddProduct(IFormFile[] Sources, string nom, string description, bool active, string SelectedColors, string StockData)
        {
            var colors = SelectedColors.Split(',').ToList();

            // Convertir StockData en liste d'objets
            var stockItems = JsonSerializer.Deserialize<List<StockItem>>(StockData);

            var newProduct = new Produit
            {
                Nom = nom,
                Description = description,
                ProduitCouleurs = colors.Select(c => new ProduitCouleur { Couleur = c }).ToList(),
                Actif = active
            };

            var productTailles = new List<ProduitTaille>();
            foreach (var item in stockItems)
            {
                productTailles.Add(new ProduitTaille
                {
                    Taille = item.Size,
                    Stock = item.Quantity,
                    Prix = item.Price,
                    
                });
            }
        }
    }
}
