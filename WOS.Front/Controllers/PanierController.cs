using Microsoft.AspNetCore.Authorization;
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

        public PanierController(IConfiguration configuration, IProduitSrv produitSrv, IMondialRelaySrv mondialRelaySrv, IModeLivraisonSrv modeLivraisonSrv)
        {
            _configuration = configuration;
            _produitSrv = produitSrv;
            _mondialRelaySrv = mondialRelaySrv;
            _modeLivraisonSrv = modeLivraisonSrv;
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

            if(actualStep == 1)
            {
                _mondialRelaySrv.ExempleRecherche();
                var modesLivraison = _modeLivraisonSrv.GetModeLivraisons();
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

            if(actualStep == 2)
            {
                // On récupère ce qui est dans le panier
                var cartItemsBytes = HttpContext.Session.Get("CartItems");
                var cartItemsJson = System.Text.Encoding.UTF8.GetString(cartItemsBytes);
                var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);
                return PartialView("_ViewFinalCart", cartItems);
            }else if (actualStep == 3)
            {
                // On récupère les modes de livraisons
                var modesLivraison = _modeLivraisonSrv.GetModeLivraisons();
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
        public IActionResult UpdateDeliveryPrice(int idModeLivraison, string newPrice)
        {
            if (float.TryParse(newPrice, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float prix))
            {
                _modeLivraisonSrv.UpdatePriceLivraison(idModeLivraison, prix);
            }
            return Ok();
        }
    }
}
