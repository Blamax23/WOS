using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WOS.Model;
using System.Text.Json;
using WOS.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WOS.Front.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IProduitSrv _produitSrv;
        private readonly IMarqueSrv _marqueSrv;
        private readonly ICategorieSrv _categorieSrv;

        public ProductController(IProduitSrv produitSrv, IMarqueSrv marqueSrv, ICategorieSrv categorieSrv)
        {
            _produitSrv = produitSrv;
            _marqueSrv = marqueSrv;
            _categorieSrv = categorieSrv;
        }
        // GET: ProductController

        [Route("")]
        [HttpGet]
        public ActionResult Index()
        {
            ProductViewModel productViewModel = new ProductViewModel
            {
                Produits = _produitSrv.GetProduits(),
                Marques = _marqueSrv.GetAllMarques(),
                Categories = _categorieSrv.GetAllCategories()
            };
            return View(productViewModel);
        }

        [HttpPost]
        [Route("AddProduct")]
        public ActionResult AddProduct(IFormFile[] Sources, string marque, string categorie, string nom, string description, bool active, string SelectedColors, string StockData)
        {
            List<string> colors = SelectedColors.Split(',').ToList();
            List<ProduitCouleur> produitCouleurs = new List<ProduitCouleur>();
            // On récupère les enums pour chaque couleur de l'enum ProduitCouleurEnum
            foreach (var color in colors)
            {
                if (!Enum.IsDefined(typeof(ProduitCouleurEnum), color))
                {
                    return BadRequest("Couleur non reconnue");
                }

                // On convertir la couleur en enum
                ProduitCouleurEnum couleurEnum = (ProduitCouleurEnum)Enum.Parse(typeof(ProduitCouleurEnum), color);
                ProduitCouleur nouvelleCouleur = new ProduitCouleur
                {
                    Couleur = couleurEnum.GetName(),
                    CodeHex = couleurEnum.GetHexCode()
                };

                produitCouleurs.Add(nouvelleCouleur);
            }

            // Convertir StockData en liste d'objets
            var stockItems = JsonSerializer.Deserialize<List<StockItem>>(StockData);

            List<ProduitImage> productImages = new List<ProduitImage>();
            bool first = true;
            foreach (var source in Sources)
            {
                if (source != null && source.Length > 0)
                {
                    var chemin = "~/uploads/" + source.FileName;
                    var cheminCopy = "wwwroot/uploads/" + source.FileName;
                    // On regarde si le fichier existe déjà 
                    if (System.IO.File.Exists(cheminCopy))
                    {
                        System.IO.File.Delete(cheminCopy);
                    }
                    using (var stream = new FileStream(cheminCopy, FileMode.Create))
                    {
                        source.CopyTo(stream);
                    }

                    //var extension = Path.GetExtension(source.FileName).Substring(1);
                    //string typeString = "";
                    //switch (extension.ToLower())
                    //{
                    //    case "jpg":
                    //    case "png":
                    //        typeString = "image";
                    //        break;
                    //    case "mp4":
                    //        typeString = "video";
                    //        break;
                    //    case "pdf":
                    //        typeString = "pdf";
                    //        break;
                    //    case "mp3":
                    //        typeString = "audio";
                    //        break;
                    //    default:
                    //        typeString = "link";
                    //        break;
                    //}

                    ProduitImage nouvelleSource = new ProduitImage
                    {
                        Url = chemin
                    };

                    if (first)
                    {
                        nouvelleSource.Principale = true;
                        first = false;
                    }

                    productImages.Add(nouvelleSource);
                }
            }

            var productTailles = new List<ProduitTaille>();
            foreach (var item in stockItems)
            {
                productTailles.Add(new ProduitTaille
                {
                    Taille = item.Size,
                    Stock = int.Parse(item.Quantity),
                    Prix = decimal.Parse(item.Price)
                });
            }

            Produit lastProduit = _produitSrv.GetProduits().OrderByDescending(p => p.Id).FirstOrDefault();
            int lastId = lastProduit == null ? 0 : lastProduit.Id;

            var newProduct = new Produit
            {
                Id = lastId + 1,
                Nom = nom,
                Description = description,
                Actif = active,
                MarqueId = int.Parse(marque),
                CategorieId = int.Parse(categorie),
                DateCreation = DateTime.Now,
                ProduitCouleurs = produitCouleurs,
                ProduitImages = productImages,
                ProduitTailles = productTailles
            };

            _produitSrv.AddProduit(newProduct);

            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Route("UpdateStockProduct")]
        public ActionResult UpdateStockProduct(string id, string stockData)
        {
            Int32.TryParse(id, out int idProduit);

            Produit produit = _produitSrv.GetProduitById(idProduit);

            var item = JsonSerializer.Deserialize<StockItem>(stockData);

            ProduitTaille produitTaille = produit.ProduitTailles.FirstOrDefault(pt => pt.Taille == item.Size);
            if (produitTaille != null)
            {
                produitTaille.Stock = int.Parse(item.Quantity);
                produitTaille.Prix = decimal.Parse(item.Price);
                produitTaille.PrixPromo = decimal.Parse(item.PriceProm);
            }

            _produitSrv.UpdateProduit(produit);

            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Route("DeleteProduct")]
        public ActionResult DeleteProduct(string id)
        {
            Int32.TryParse(id, out int idProduit);

            _produitSrv.DeleteProduit(idProduit);

            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Route("GetProductFiltered")]
        public ActionResult GetProductFiltered(List<string> marque, List<string> categorie, List<string> couleur, string prixMin, string prixMax)
        {
            List<int> marques = new List<int>();
            List<int> categories = new List<int>();
            List<string> couleurs = new List<string>();
            decimal.TryParse(prixMin, out decimal prixMinDecimal);
            decimal.TryParse(prixMax, out decimal prixMaxDecimal);

            foreach (var item in marque)
            {
                if (item == null) continue;
                string idMarque = item.Split('-')[1];
                marques.Add(int.Parse(idMarque));
            }

            foreach (var item in categorie)
            {
                if (item == null) continue;
                string idCategorie = item.Split('-')[1];
                categories.Add(int.Parse(idCategorie));
            }

            foreach (var item in couleur)
            {
                ProduitCouleurEnum couleurEnum = (ProduitCouleurEnum)Enum.Parse(typeof(ProduitCouleurEnum), item);
                couleurs.Add(couleurEnum.GetName());
            }

            List<Produit> products = _produitSrv.GetProduits();

            List<Produit> productsFiltered = products;

            if (marques.Count > 0)
            {
                productsFiltered = productsFiltered.Where(p => marques.Contains(p.MarqueId.Value)).ToList();
            }

            if (categories.Count > 0)
            {
                productsFiltered = productsFiltered.Where(p => categories.Contains(p.CategorieId.Value)).ToList();
            }

            if (couleurs.Count > 0)
            {
                productsFiltered = productsFiltered.Where(p => p.ProduitCouleurs.Any(pc => couleurs.Contains(pc.Couleur))).ToList();
            }

            if (prixMinDecimal > 0)
            {
                productsFiltered = productsFiltered.Where(p => p.ProduitTailles.Any(pt => pt.Prix >= prixMinDecimal)).ToList();
            }

            if (prixMaxDecimal > 0)
            {
                productsFiltered = productsFiltered.Where(p => p.ProduitTailles.Any(pt => pt.Prix <= prixMaxDecimal)).ToList();
            }



            ProductViewModel productViewModel = new ProductViewModel
            {
                Produits = productsFiltered,
                Marques = _marqueSrv.GetAllMarques(),
                Categories = _categorieSrv.GetAllCategories()
            };

            return PartialView("_FilteredProducts", productsFiltered);
        }

        [HttpGet]
        [Route("sneakers/{id}")]
        public ActionResult ViewProduct(int id)
        {
            Produit produit = _produitSrv.GetProduitById(id);

            return View(produit);
        }
    }
}
