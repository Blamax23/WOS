using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WOS.Model;
using System.Text.Json;
using WOS.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using iText.Kernel.Geom;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Globalization;

namespace WOS.Front.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IProduitSrv _produitSrv;
        private readonly IMarqueSrv _marqueSrv;
        private readonly ICategorieSrv _categorieSrv;
        private readonly IAvisSrv _avisSrv;
        private readonly IGlobalDataSrv _globalDataSrv;
        private readonly ICompositeViewEngine _viewEngine;
        private int pageNumber;
        private int pageSize = 4;

        public ProductController(IProduitSrv produitSrv, IMarqueSrv marqueSrv, ICategorieSrv categorieSrv, IAvisSrv avisSrv, IGlobalDataSrv globalDataSrv, ICompositeViewEngine viewEngine)
        {
            _produitSrv = produitSrv;
            _marqueSrv = marqueSrv;
            _categorieSrv = categorieSrv;
            _avisSrv = avisSrv;
            _globalDataSrv = globalDataSrv;
            _viewEngine = viewEngine;
        }
        // GET: ProductController

        [Route("list")]
        [HttpGet]
        public ActionResult Index([FromQuery] string pageSeen = "1")
        {
            int page = Int32.TryParse(pageSeen, out page) ? page : 1;
            this.pageNumber = page;

            // Récupération des données
            var products = _globalDataSrv.Produits;
            var lignes = _globalDataSrv.LignesCommande;

            // On regroupe par ProduitId, trie par nombre d'occurrences et applique la pagination
            var p = lignes
                .GroupBy(l => l.ProduitId)
                .OrderByDescending(g => g.Count())
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Liste triée des produits
            List<Produit> productsSorted = FiltrerEtTrierProduits(
                    _globalDataSrv.Produits,
                    new List<int>(),  // Pas de filtre de marque
                    new List<int>(),  // Pas de filtre de catégorie
                    new List<string>(),  // Pas de filtre de couleur
                    null,
                    null,
                    "tendances",
                    pageNumber,
                    pageSize
                );

            int nbPages = (int)Math.Ceiling((double)productsSorted.Count / pageSize);

            HttpContext.Session.SetInt32("NbPages", nbPages);

            ProductViewModel productViewModel = new ProductViewModel
            {
                Produits = productsSorted.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                Marques = _globalDataSrv.Marques,
                Categories = _globalDataSrv.Categories,
                NbPages = nbPages,
                Page = pageNumber
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
                    // On enlève les accents du FileName
                    var fileName = source.FileName.Normalize(NormalizationForm.FormD);
                    var chars = fileName.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
                    fileName = new string(chars).Normalize(NormalizationForm.FormC);

                    var chemin = "~/uploads/" + fileName;
                    var cheminCopy = "wwwroot/uploads/" + fileName;
                    // On regarde si le fichier existe déjà 
                    if (System.IO.File.Exists(cheminCopy))
                    {
                        System.IO.File.Delete(cheminCopy);
                    }
                    using (var stream = new FileStream(cheminCopy, FileMode.Create))
                    {
                        source.CopyTo(stream);
                    }

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

            Produit lastProduit = _globalDataSrv.Produits.OrderByDescending(p => p.Id).FirstOrDefault();
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

            decimal? promoPrice = null;
            if ((item.PriceProm != null && item.PriceProm != ""))
            {
                promoPrice = decimal.Parse(item.PriceProm);
            }

            ProduitTaille produitTaille = produit.ProduitTailles.FirstOrDefault(pt => pt.Taille == item.Size);
            if (produitTaille != null)
            {
                produitTaille.Stock = int.Parse(item.Quantity);
                produitTaille.Prix = decimal.Parse(item.Price);
                produitTaille.PrixPromo = promoPrice;
            }

            _produitSrv.UpdateProduitTaille(produitTaille);

            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Route("AddRowTableStock")]
        public ActionResult AddRowTableStock(string id, string taille, string quantite, string prix, string promo)
        {
            Produit produit = _produitSrv.GetProduitById(int.Parse(id));

            Int32.TryParse(quantite, out int stock);
            decimal price = decimal.Parse(prix);
            decimal? promoPrice = null;
            if (promo != null)
            {
                promoPrice = decimal.Parse(promo);
            }

            ProduitTaille produitTaille = new ProduitTaille
            {
                Taille = taille,
                Stock = stock,
                Prix = price,
                PrixPromo = promoPrice
            };

            _produitSrv.AddProduitTaille(produitTaille, produit.Id);

            return Ok();
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
        public ActionResult GetProductFiltered(List<string> marque, List<string> categorie, List<string> couleur, string prixMin = null, string prixMax = null, string tri = "tendances", int page = 0)
        {
            ProductsListViewModel productsListViewModel = new ProductsListViewModel();
            List<int> marques = new List<int>();
            List<int> categories = new List<int>();
            List<string> couleurs = new List<string>();
            decimal.TryParse(prixMin, out decimal prixMinDecimal);
            decimal.TryParse(prixMax, out decimal prixMaxDecimal);

            int pageNumber = 0;
            if (page == 0)
            {
                pageNumber = HttpContext.Session.GetInt32("PageNumber") ?? 1;
            }
            else
            {
                pageNumber = page;
            }

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

            List<Produit> products = _globalDataSrv.Produits;

            List<Produit> productsFiltered = FiltrerEtTrierProduits(
                _globalDataSrv.Produits,
                marques,
                categories,
                couleurs,
                prixMinDecimal > 0 ? prixMinDecimal : (decimal?)null,
                prixMaxDecimal > 0 ? prixMaxDecimal : (decimal?)null,
                tri,
                pageNumber,
                pageSize
            );

            int nbPages = (int)Math.Ceiling((double)productsFiltered.Count / pageSize);

            if (pageNumber > nbPages)
            {
                pageNumber = nbPages;
            }

            HttpContext.Session.SetInt32("NbPages", nbPages);
            HttpContext.Session.SetInt32("PageNumber", pageNumber);

            productsFiltered = productsFiltered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            productsListViewModel.Produits = productsFiltered;
            productsListViewModel.Pagination = new PaginationModel
            {
                Page = pageNumber,
                NbPages = nbPages
            };

            //return PartialView("_FilteredProducts", productsListViewModel);
            return Json(new
            {
                html = RenderPartialViewToString("_FilteredProducts", productsListViewModel),
                nbPages = nbPages
            });
        }

        [HttpPost]
        [Route("GetProductSorted")]
        public ActionResult GetProductSorted(string tri)
        {
            if (tri == "ascending-alphabet")
                return PartialView("_FilteredProducts", _globalDataSrv.Produits.OrderBy(p => p.Nom).ToList());
            else if (tri == "descending-alphabet")
                return PartialView("_FilteredProducts", _globalDataSrv.Produits.OrderByDescending(p => p.Nom).ToList());
            else if (tri == "ascending-price")
                return PartialView("_FilteredProducts", _globalDataSrv.Produits.OrderBy(p => p.ProduitTailles.Min(pt => pt.Prix)).ToList());
            else if (tri == "descending-price")
                return PartialView("_FilteredProducts", _globalDataSrv.Produits.OrderByDescending(p => p.ProduitTailles.Min(pt => pt.Prix)).ToList());
            else
            {
                // Dans les lignes commandes, on compte les id product qui reviennent le plus de fois
                var products = _globalDataSrv.Produits;
                var lignes = _globalDataSrv.LignesCommande;
                // On classe dans tableau les product_id qui revienne le plus souvent
                var p = lignes.GroupBy(l => l.ProduitId).OrderByDescending(g => g.Count()).ToList();
                // On crée une liste de produit qui contient les produits triés
                List<Produit> productsSorted = new List<Produit>();
                List<Produit> productsNoSorted = products;
                foreach (var prod in p)
                {
                    productsSorted.Add(products.FirstOrDefault(p => p.Id == prod.Key));
                    productsNoSorted.Remove(products.FirstOrDefault(p => p.Id == prod.Key));
                }

                productsSorted.AddRange(productsNoSorted);

                return PartialView("_FilteredProducts", productsSorted);
            }
        }

        [HttpGet]
        [Route("sneakers/{id}")]
        public ActionResult ViewProduct(int id)
        {
            Produit produit = _produitSrv.GetProduitById(id);

            return View(produit);
        }

        [HttpPost]
        [Route("UpdateProductActive")]
        public ActionResult UpdateActive(string id, string active)
        {
            try
            {
                Int32.TryParse(id, out int productId);
                bool.TryParse(active, out bool act);

                _produitSrv.UpdateActiveProduit(productId, act);

                return Ok(new { errorMessage = "" });
            }
            catch (Exception e)
            {
                return Ok(new { errorMessage = "Erreur dans la modification du statut Actif." });
            }
        }

        [HttpPost]
        [Route("UpdateProductTendance")]
        public ActionResult UpdateTendance(string id, string tendance)
        {
            try
            {
                Int32.TryParse(id, out int productId);
                bool.TryParse(tendance, out bool tend);

                _produitSrv.UpdateTendanceProduit(productId, tend);

                return Ok(new { errorMessage = "" });
            }
            catch (Exception e)
            {
                return Ok(new { errorMessage = "Erreur dans la modification du statut Tendance." });
            }
        }

        private List<Produit> FiltrerEtTrierProduits(
    List<Produit> produits,
    List<int> marques,
    List<int> categories,
    List<string> couleurs,
    decimal? prixMin,
    decimal? prixMax,
    string tri,
    int page,
    int pageSize)
        {
            try
            {
                var produitsFiltres = produits.AsQueryable();

                // Filtrage
                if (marques.Any())
                    produitsFiltres = produitsFiltres.Where(p => marques.Contains(p.MarqueId.Value));

                if (categories.Any())
                    produitsFiltres = produitsFiltres.Where(p => categories.Contains(p.CategorieId.Value));

                if (couleurs.Any())
                    produitsFiltres = produitsFiltres.Where(p => p.ProduitCouleurs.Any(pc => couleurs.Contains(pc.Couleur)));

                if (prixMin.HasValue)
                    produitsFiltres = produitsFiltres.Where(p => p.ProduitTailles.Any(pt => pt.Prix >= prixMin.Value));

                if (prixMax.HasValue)
                    produitsFiltres = produitsFiltres.Where(p => p.ProduitTailles.Any(pt => pt.Prix <= prixMax.Value));

                // Tri
                switch (tri)
                {
                    case "ascending-alphabet":
                        produitsFiltres = produitsFiltres.OrderBy(p => p.Nom);
                        break;
                    case "descending-alphabet":
                        produitsFiltres = produitsFiltres.OrderByDescending(p => p.Nom);
                        break;
                    case "ascending-price":
                        produitsFiltres = produitsFiltres.OrderBy(p => p.ProduitTailles.Min(pt => pt.Prix));
                        break;
                    case "descending-price":
                        produitsFiltres = produitsFiltres.OrderByDescending(p => p.ProduitTailles.Min(pt => pt.Prix));
                        break;
                    case "tendances":
                    default:
                        var tendances = _globalDataSrv.LignesCommande
                            .GroupBy(l => l.ProduitId)
                            .OrderByDescending(g => g.Count())
                            .Select(g => g.Key)
                            .ToList();

                        produitsFiltres = produitsFiltres
                            .OrderBy(p => tendances.IndexOf(p.Id)); // Trier selon l'ordre des tendances
                        break;
                }

                // Pagination
                return produitsFiltres.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        [HttpGet]
        [Route("AddAvisProduct")]
        public ActionResult AddAvisProduct(string idProduit, string code)
        {
            try
            {
                AvisViewModel avisViewModel = new AvisViewModel();
                var produitIds = idProduit.Split(',').Select(int.Parse).ToList();
                foreach (var id in produitIds)
                {
                    Produit produit = _produitSrv.GetProduitById(id);
                    avisViewModel.Produits.Add(produit);
                }

                // On recherche dans les avis si le client a déjà donné son avis avec le code
                var avis = _globalDataSrv.Avis.FirstOrDefault(a => a.CodeAvis == code);
                if (avis != null)
                {
                    avisViewModel.IsPosted = true;
                }
                avisViewModel.CodeAvis = code;

                return View(avisViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("SubmitAvis")]
        public ActionResult AddAvis(string produitIds = null, string avisText = null, int note = 0, string codeCommande = null)
        {
            try
            {
                foreach (var produitId in produitIds.Split(',').Select(int.Parse))
                {
                    // On récupère le client avec le claimstype.Email
                    int id = 0;
                    Client client = _globalDataSrv.Clients.FirstOrDefault(c => c.Email == User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value);
                    Admin admin = new Admin();
                    if (client == null)
                    {
                        admin = _globalDataSrv.Admins.FirstOrDefault(a => a.Email == User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value);
                        id = admin.Id;
                    }
                    else
                    {
                        id = client.Id;
                    }
                    Avis avis = new Avis
                    {
                        ClientId = id,
                        ProduitId = produitId,
                        Commentaire = avisText,
                        Note = note,
                        DateAvis = DateTime.Now,
                        CodeAvis = codeCommande
                    };

                    _produitSrv.AddAvis(avis);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        protected async Task<string> RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.ActionDescriptor.ActionName;
            }

            using (var sw = new StringWriter())
            {
                var actionContext = new ActionContext(HttpContext, RouteData, new ActionDescriptor());

                var viewResult = _viewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary<object>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return sw.ToString();
            }
        }
    }
}
