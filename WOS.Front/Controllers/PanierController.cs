﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WOS.Model;
using System.Text.Json;
using WOS.Dal.Interfaces;
using WOS.Dal.Context;
using WOS.Back.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using iText.Commons.Actions.Contexts;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.Rendering;

namespace WOS.Front.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class PanierController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IProduitSrv _produitSrv;
        private readonly IMondialRelaySrv _mondialRelaySrv;
        private readonly IModeLivraisonSrv _modeLivraisonSrv;
        private readonly ICommandeSrv _commandeSrv;
        private readonly IGlobalDataSrv _globalDataSrv;
        private readonly IAdresseSrv _adresseSrv;

        public PanierController(IConfiguration configuration, IProduitSrv produitSrv, IMondialRelaySrv mondialRelaySrv, IModeLivraisonSrv modeLivraisonSrv, ICommandeSrv commandeSrv, IGlobalDataSrv globalDataSrv, IAdresseSrv adresseSrv)
        {
            _configuration = configuration;
            _produitSrv = produitSrv;
            _mondialRelaySrv = mondialRelaySrv;
            _modeLivraisonSrv = modeLivraisonSrv;
            _commandeSrv = commandeSrv;
            _globalDataSrv = globalDataSrv;
            _adresseSrv = adresseSrv;
        }

        [HttpPost]
        [Route("ViewCart")]
        public ActionResult ViewCart([FromBody] List<CartItem> cartItems)
        {
            //var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);
            foreach (var item in cartItems)
            {
                Produit produit = _produitSrv.GetProduitById(item.ProductId);
                item.ImageUrl = produit.ProduitImages.FirstOrDefault(i => i.Principale)?.Url;
                item.Name = produit.Nom;
                item.Price = produit.ProduitTailles.FirstOrDefault(t => t.Taille == item.Size).Prix;

            }



            var cartItemsJson = JsonSerializer.Serialize(cartItems);
            HttpContext.Session.Set("CartItems", System.Text.Encoding.UTF8.GetBytes(cartItemsJson));

            return Ok();
        }

        [HttpGet]
        [Route("Display")]
        public ActionResult DisplayCart()
        {
            var cartItemsBytes = HttpContext.Session.Get("CartItems");

            if (cartItemsBytes == null)
                return View(new List<CartItem>()); // Retourne une liste vide si rien n'est stocké

            // Convertir les bytes en chaîne et désérialiser en liste
            var cartItemsJson = System.Text.Encoding.UTF8.GetString(cartItemsBytes);
            var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);

            // On stocke qu'on reidirge vers l'étape 1 dans les cookies
            HttpContext.Response.Cookies.Append("CartStep", "1");

            return View(cartItems);
        }

        public List<CartItem> GetCartFromCookies(HttpContext context)
        {
            var cartCookie = context.Request.Cookies["Cart"];
            if (!string.IsNullOrEmpty(cartCookie))
            {
                return JsonSerializer.Deserialize<List<CartItem>>(cartCookie);
            }
            return new List<CartItem>();
        }

        public void SaveCartToCookies(HttpContext context, List<CartItem> cart)
        {
            var jsonCart = JsonSerializer.Serialize(cart);
            context.Response.Cookies.Append("Cart", jsonCart, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7)
            });
        }

        public void RemoveItemFromCart(HttpContext context, int idProduct, string size)
        {
            var cartCookies = context.Request.Cookies["Cart"];
            if (!string.IsNullOrEmpty(cartCookies))
            {
                List<CartItem> cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartCookies);
                CartItem itemToDelete = cartItems.FirstOrDefault(i => i.ProductId == idProduct && i.Size == size);
                cartItems.Remove(itemToDelete);
                SaveCartToCookies(context, cartItems);
            }
        }

        public void RemoveAllItemsFromCart(HttpContext context)
        {
            context.Response.Cookies.Delete("Cart");
        }

        [HttpPost]
        [Route("AddToCart")]
        public IActionResult AddToCart(int productId, string name, decimal price, string url, string size)
        {
            string domain = _configuration.GetSection("Site")["Domain"];
            var cart = GetCartFromCookies(HttpContext);
            var existingItem = cart.FirstOrDefault(x => x.ProductId == productId && x.Size == size.Split(" ")[0]);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                url = Path.Combine(domain, url.Split("~/")[1]);
                cart.Add(new CartItem { ProductId = productId, Name = name, Price = price, Quantity = 1, ImageUrl = url, Size = size.Split(" ")[0] });
            }
            SaveCartToCookies(HttpContext, cart);
            return Ok(cart);
        }

        [HttpPost]
        [Route("DeleteInCart")]
        public IActionResult RemoveItem(int productId, string size)
        {
            RemoveItemFromCart(HttpContext, productId, size);
            return Ok();
        }

        [HttpPost]
        [Route("UpdateQuantity")]
        public IActionResult UpdateQuantity(int productId, string size, int quantity)
        {
            var cartCookies = HttpContext.Request.Cookies["Cart"];
            if (!string.IsNullOrEmpty(cartCookies))
            {
                List<CartItem> cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartCookies);
                CartItem itemToUpdate = cartItems.FirstOrDefault(i => i.ProductId == productId && i.Size == size);
                itemToUpdate.Quantity = quantity;
                SaveCartToCookies(HttpContext, cartItems);
            }
            return Ok();
        }

        [HttpGet]
        [Route("")]
        public IActionResult ViewCart()
        {
            var cart = GetCartFromCookies(HttpContext);
            return View(cart);
        }

        [HttpPost]
        [Route("NextStepPurchase")]
        public IActionResult NextStepPurchase(int actualStep, string deliveryInfo = null)
        {
            // On supprime le potentiel cookie de l'étape 1
            HttpContext.Response.Cookies.Delete("CartStep");
            HttpContext.Response.Cookies.Append("CartStep", (actualStep + 1).ToString());

            if (actualStep == 1)
            {
                _mondialRelaySrv.ExempleRecherche();
                var modesLivraison = _globalDataSrv.ModeLivraisons;
                return PartialView("_ViewInfoDelivery", modesLivraison);
            }
            else if (actualStep == 2)
            {
                ViewFinalPurchase viewFinalPurchase = new ViewFinalPurchase();
                // On récupère l'objet deliveryInfo dans le localStora
                DeliveryInfo deliveryInfoObj = new DeliveryInfo();
                if (deliveryInfo != null)
                {
                    // On séserialise la string json
                    deliveryInfoObj = JsonSerializer.Deserialize<DeliveryInfo>(deliveryInfo);
                }
                viewFinalPurchase.DeliveryInfo = deliveryInfoObj;
                // On récupère le panier final
                var cartItemsBytes = HttpContext.Session.Get("CartItems");
                var cartItemsJson = System.Text.Encoding.UTF8.GetString(cartItemsBytes);
                var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);
                viewFinalPurchase.Cart = cartItems;

                // On récupère le mode de livraison
                ModeLivraison modeLivraison = _modeLivraisonSrv.GetModeLivraisonByName(deliveryInfoObj.ModeName);
                viewFinalPurchase.ModeLivraison = modeLivraison;

                // On calcule le prix total
                decimal totalPrice = 0;
                foreach (var item in cartItems)
                {
                    totalPrice += item.Price * item.Quantity;
                }
                totalPrice += (decimal)modeLivraison.PrixLivraison;
                viewFinalPurchase.TotalPrice = totalPrice.ToString();


                //Int32.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value, out int clientId);
                int clientId = 1;
                // On crée l'adresse 
                int adresseLivraisonId = 0;

                Adresse adresseToSend = new Adresse
                {
                    ClientId = clientId,
                    Rue = deliveryInfoObj.Address,
                    Ville = deliveryInfoObj.City,
                    CodePostal = deliveryInfoObj.ZipCode,
                    Pays = deliveryInfoObj.Country,
                    Principale = true,
                    PointRelais = false,
                };

                if (!string.IsNullOrEmpty(deliveryInfoObj.ParcelShop.Name))
                {

                    adresseToSend = new Adresse
                    {
                        ClientId = clientId,
                        Nom = deliveryInfoObj.ParcelShop.Name,
                        Rue = deliveryInfoObj.ParcelShop.Address,
                        Ville = deliveryInfoObj.ParcelShop.City,
                        CodePostal = deliveryInfoObj.ParcelShop.ZipCode,
                        Pays = deliveryInfoObj.ParcelShop.Country,
                        Principale = false,
                        PointRelais = true
                    };

                    _adresseSrv.AddAdresse(adresseToSend);
                    _adresseSrv.ChangeAdressePrincipale();
                    adresseLivraisonId = _globalDataSrv.Adresses.FirstOrDefault(a => a.PointRelais == true && a.Nom == adresseToSend.Nom).Id;
                }
                string numberOrder = GenerateOrderNumber();

                viewFinalPurchase.OrderNumber = numberOrder;

                // On crée la commande
                Commande commande = new Commande
                {
                    ClientId = clientId,
                    MontantTotal = totalPrice,
                    DateCommande = DateTime.Now,
                    StatutId = 1,
                    NumeroCommande = numberOrder,
                    AdresseLivraison = adresseToSend,
                    ModeLivraisonId = modeLivraison.Id
                };

                foreach (var item in cartItems)
                {
                    Produit produit = _produitSrv.GetProduitById(item.ProductId);
                    commande.LignesCommande.Add(new LigneCommande
                    {
                        PrixUnitaire = item.Price,
                        Quantite = item.Quantity,
                        ProduitId = item.ProductId,
                        ProduitCouleurId = produit.ProduitCouleurs.FirstOrDefault().Id,
                        ProduitTailleId = produit.ProduitTailles.FirstOrDefault().Id
                    });
                }
                _adresseSrv.ChangeAdressePrincipale();
                _commandeSrv.AddCommande(commande);

                viewFinalPurchase.Domain = _configuration.GetSection("Site")["Domain"];

                return PartialView("_ViewPayment", viewFinalPurchase);
            }
            else
            {
                // On récupère ce qui est dans le panier
                var cartItemsBytes = HttpContext.Session.Get("CartItems");
                var cartItemsJson = System.Text.Encoding.UTF8.GetString(cartItemsBytes);
                var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);
                return PartialView("_ViewFinalCart", cartItems);
            }
        }

        [HttpPost]
        [Route("PreviousStepPurchase")]
        public IActionResult PreviousStepPurchase(int actualStep)
        {
            // On supprime le potentiel cookie de l'étape 1
            HttpContext.Response.Cookies.Delete("CartStep");
            HttpContext.Response.Cookies.Append("CartStep", (actualStep - 1).ToString());

            if (actualStep == 2)
            {
                // On récupère ce qui est dans le panier
                var cartItemsBytes = HttpContext.Session.Get("CartItems");
                var cartItemsJson = System.Text.Encoding.UTF8.GetString(cartItemsBytes);
                var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);
                return PartialView("_ViewFinalCart", cartItems);
            }
            else if (actualStep == 3)
            {
                // On récupère les modes de livraisons
                var modesLivraison = _globalDataSrv.ModeLivraisons;
                return PartialView("_ViewInfoDelivery", modesLivraison);
            }
            else
                return PartialView("_ViewPayment");

        }

        [HttpGet]
        [Route("GetValidateCart")]
        public ActionResult GetValidateCart()
        {
            var cart = GetCartFromCookies(HttpContext);
            return PartialView("_ViewFinalCart", cart);
        }

        [HttpPost]
        [Route("UpdateDeliveryPrice")]
        public IActionResult UpdateDeliveryPrice(int idModeLivraison, string newPrice, string deliveryTime)
        {
            Int32.TryParse(deliveryTime, out int time);
            float.TryParse(newPrice, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float prix);
            _modeLivraisonSrv.UpdatePriceLivraison(idModeLivraison, prix, time);
            return Ok();
        }

        [HttpGet]
        [Route("ConfirmPurchase")]
        public async Task<ActionResult> ConfirmPurchase(string numberOrder, string payment_intent, string payment_intent_client_secret, string redirect_status)
        {
            Commande commande = _commandeSrv.GetCommandeByNumberOrder(numberOrder);

            string token = _modeLivraisonSrv.GetAuthUPS().Result;

            // On initialise les variables
            byte[] etiquette = null;
            string trackingNumber = null;

            commande.StatutId++;

            // On crée la facture
            //CreateInvoice(commande);

            if (commande.ModeLivraisonId != null)
            {
                string name = _modeLivraisonSrv.GetModeLivraisonById((int)commande.ModeLivraisonId).Nom;

                switch (name)
                {
                    case "Mondial Relay":
                        await _modeLivraisonSrv.GetEtiquetteFromMondialRelay();
                        commande.LinkSuivi = "https://www.mondialrelay.fr/suivi-de-colis/?NumeroExpedition=" + trackingNumber;
                        break;
                    case "UPS Standard":
                        (etiquette, trackingNumber) = _modeLivraisonSrv.GetEtiquetteFromUPS(token).Result;
                        commande.LinkSuivi = "https://www.ups.com/track?loc=fr_FR&tracknum=" + trackingNumber;
                        break;
                }

                commande.NumeroCommandeLivreur = trackingNumber;
                commande.BinaryEtiquette = etiquette;
            }

            //_commandeSrv.UpdateStatus(commande.Id);
            _commandeSrv.UpdateCommande(commande);

            // On récupère tous les produits de la commande
            ConfirmPurchaseModel confirmPurchaseModel = new ConfirmPurchaseModel() { Commande = commande };

            foreach (var ligneCommande in commande.LignesCommande)
            {
                Produit produit = _produitSrv.GetProduitById(ligneCommande.ProduitId);
                confirmPurchaseModel.Produits.Add(produit);
            }

            // On supprime le panier
            RemoveAllItemsFromCart(HttpContext);

            return View(confirmPurchaseModel);
        }

        //[Route("stripesecret")]
        [Route("create-payment-intent")]
        [HttpPost]
        public ActionResult Get(string request)
        {
            PaymentIntentCreateRequest paymentIntentCreateRequest = JsonSerializer.Deserialize<PaymentIntentCreateRequest>(request);
            // Configuration de Stripe
            StripeConfiguration.ApiKey = "sk_test_51QaCRUAwc3ZcnwnKhHbrJveMJCMiiDidqRmYtHdW4GoOmELOHras2whfHzz2zGouzU5bF4VfOLXWzahmoGh83vqd00600F7Jn8";

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = CalculateOrderAmount(paymentIntentCreateRequest.Items),
                Currency = "eur",
                // In the latest version of the API, specifying the `automatic_payment_methods` parameter is optional because Stripe enables its functionality by default.
                //AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                //{
                //    Enabled = true,
                //},
                PaymentMethodTypes = new List<string>
                {
                    "card", "klarna", "paypal" // Active les méthodes de paiement : carte bancaire et Klarna
                },
                PaymentMethodOptions = new PaymentIntentPaymentMethodOptionsOptions
                {
                    Klarna = new PaymentIntentPaymentMethodOptionsKlarnaOptions
                    {
                        PreferredLocale = "fr-FR", // Localisation française pour Klarna
                    },
                },
            });

            return Json(new { clientSecret = paymentIntent.ClientSecret, amount = paymentIntent.Amount });
        }

        [HttpPost("GetCommande")]
        public IActionResult GetCommande(string numberOrder)
        {
            try
            {
                var commande = _commandeSrv.GetCommandeByNumberOrder(numberOrder);

                return Json(new { etiquette = commande.BinaryEtiquette });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("UpdateCommandeStatus")]
        public IActionResult UpdateCommandeStatus(string commandeId, int statut)
        {
            // On récupère la commande
            var commande = _commandeSrv.GetCommandeByNumberOrder(commandeId);

            // On met à jour le statut
            commande.StatutId = statut;

            // On met à jour la commande
            _commandeSrv.UpdateCommande(commande);

            return RedirectToAction("Index", "Account");
        }

        private long CalculateOrderAmount(List<Item> items)
        {
            // Calculate the order total on the server to prevent
            // people from directly manipulating the amount on the client
            long total = 0;
            foreach (var item in items)
            {
                string sanitizedInput = item.Amount.Replace(",", ".");

                // Essayer de convertir en décimal
                if (decimal.TryParse(sanitizedInput, System.Globalization.NumberStyles.Any,
                                     System.Globalization.CultureInfo.InvariantCulture, out decimal decimalValue))
                {
                    total += (long)(decimalValue * 100);
                }

            }

            return total;
        }

        private string GenerateOrderNumber()
        {
            string year = DateTime.Now.Year.ToString();
            string countOrder = (_globalDataSrv.Commandes.Count + 1).ToString("D4");

            Random random = new Random();
            int randomNumber = random.Next(100000, 1000000);

            return year + countOrder + randomNumber.ToString();
        }

        private void CreateInvoice(Commande commande)
        {
            Client client = _globalDataSrv.Clients.FirstOrDefault(c => c.Id == commande.ClientId);
            Document document = new Document(); 

            Section section = document.AddSection();

            // On met deux paragraphLeftes côte à côte

            Paragraph paragraphLeft = section.AddParagraph();
            paragraphLeft.Format.Font.Size = 12;
            paragraphLeft.Format.Font.Bold = true;
            // On ajoute sur la même ligne une image et du texte
            string cheminImage = "C:\\Users\\Maxim\\source\\repos\\WOS\\WOS.Front\\wwwroot\\src\\WosLogos\\logoWosBlack.png";
            //string cheminImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "src", "WosLogos", "logoWosBlack.png");
            ImageSource.IImageSource imageSource = ImageSource.FromFile(cheminImage);
            paragraphLeft.AddImage(imageSource).Width = "3cm";
            paragraphLeft.AddText("SNEAKERS");
            paragraphLeft.AddLineBreak();
            paragraphLeft.AddText("Maxime BAILLY");
            paragraphLeft.AddLineBreak();
            paragraphLeft.AddText("257 rue des Blés d'or");
            paragraphLeft.AddLineBreak();
            paragraphLeft.AddText("01000 St Denis lès Bourg");
            paragraphLeft.AddLineBreak();
            paragraphLeft.AddText("France");
            paragraphLeft.AddLineBreak();
            paragraphLeft.AddText("SIRET : 981 978 281 00013");

            paragraphLeft = section.AddParagraph();

            paragraphLeft.Format.Alignment = ParagraphAlignment.Left;

            Paragraph paragraphRight = section.AddParagraph();
            paragraphRight.Format.Alignment = ParagraphAlignment.Right;
            paragraphRight.Format.Font.Size = 12;

            paragraphRight.AddText(client.Prenom + " " + client.Nom);
            paragraphRight.AddLineBreak();
            paragraphRight.AddText(client.Adresses.FirstOrDefault(a => a.Principale).Rue);
            paragraphRight.AddLineBreak();
            paragraphRight.AddText(client.Adresses.FirstOrDefault(a => a.Principale).CodePostal + " " + client.Adresses.FirstOrDefault(a => a.Principale).Ville);
            paragraphRight.AddLineBreak();
            paragraphRight.AddText(client.Adresses.FirstOrDefault(a => a.Principale).Pays);
            paragraphRight.AddLineBreak();


            // On ferme la section et on en ouvre une nouvelle
            section = document.AddSection();
            section.AddParagraph("Facture n°" + commande.NumeroCommande).Format.Font.Size = 12;
            section.AddParagraph("Date : " + DateTime.Now.ToString("dd/MM/yyyy")).Format.Font.Size = 12;
            
            // On ajoute un tableau
            var table = section.AddTable();
            table.Borders.Width = 0.75;
            table.Borders.Color = Colors.Black;
            table.Borders.Visible = true;

            // On ajoute les colonnes
            table.AddColumn("5cm");
            table.AddColumn("3cm");
            table.AddColumn("3cm");
            table.AddColumn("3cm");
            table.AddColumn("3cm");

            // On ajoute les en-têtes
            var row = table.AddRow();
            row.Shading.Color = Colors.LightGray;
            row.Cells[0].AddParagraph("DESCRIPTION");
            row.Cells[1].AddParagraph("QUANTITÉ");
            row.Cells[1].AddParagraph("TAILLE");
            row.Cells[2].AddParagraph("PRIX UNITAIRE");
            row.Cells[3].AddParagraph("MONTANT HT");

            // On ajoute les lignes
            foreach (var ligneCommande in commande.LignesCommande)
            {
                Produit produit = _produitSrv.GetProduitById(ligneCommande.ProduitId);
                row = table.AddRow();
                row.Cells[0].AddParagraph(produit.Nom);
                row.Cells[1].AddParagraph(ligneCommande.Quantite.ToString());
                row.Cells[2].AddParagraph(produit.ProduitTailles.FirstOrDefault(t => t.Id == ligneCommande.ProduitTailleId).Taille);
                row.Cells[3].AddParagraph(produit.ProduitTailles.FirstOrDefault(t => t.Id == ligneCommande.ProduitTailleId).Prix.ToString());
                row.Cells[4].AddParagraph((ligneCommande.Quantite * produit.ProduitTailles.FirstOrDefault(t => t.Id == ligneCommande.ProduitTailleId).Prix).ToString());
            }

            // On récupère le Mode de livraison
            ModeLivraison modeLivraison = _modeLivraisonSrv.GetModeLivraisonById(commande.ModeLivraisonId.Value);

            // On convertit le prix de livraison de float a decimal
            decimal prixLivraison = decimal.Parse(modeLivraison.PrixLivraison.ToString(), CultureInfo.InvariantCulture);

            // On ajoute le total dans une nouvelle section avec le sous-total qui est le total des produits et que le total HT qui est le total avec frais de port
            section = document.AddSection();
            section.AddParagraph("Sous-total : " + commande.MontantTotal + " €").Format.Font.Size = 12;
            section.AddParagraph("Frais de port : " + prixLivraison + " €").Format.Font.Size = 12;
            section.AddParagraph("Total HT : " + (commande.MontantTotal + prixLivraison) + " €").Format.Font.Size = 12;

            // On ajoute une nouvelle section en bas de page pour la TVA et les coordonnées bancaires
            section = document.AddSection();
            section.AddParagraph("TVA non applicable, art. 293 B du CGI").Format.Font.Size = 12;
            section.AddParagraph("Règlement par chèque ou virement bancaire").Format.Font.Size = 12;
            section.AddParagraph("IBAN : FR76 3000 3032 0000 0200 0000 007").Format.Font.Size = 12;
            section.AddParagraph("BIC : SOGEFRPP").Format.Font.Size = 12;
            
            // On sauvegarde le document
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save("C:/Users/Maxim/Downloads/Facture_" + commande.NumeroCommande + ".pdf");

            pdfRenderer.PdfDocument.Close();
        }

        public class Item
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            [JsonPropertyName("amount")]
            public string Amount { get; set; }
        }

        public class PaymentIntentCreateRequest
        {
            [JsonPropertyName("items")]
            public List<Item> Items { get; set; }
        }
    }
}
