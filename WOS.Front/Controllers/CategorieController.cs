using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WOS.Model;
using System.Text.Json;
using WOS.Dal.Interfaces;
using WOS.Dal.Context;

namespace WOS.Front.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class CategorieController : Controller
    {
        private readonly ICategorieSrv _categorieSrv;

        public CategorieController(ICategorieSrv categorieSrv)
        {
            _categorieSrv = categorieSrv;
        }

        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("AddCategorie")]
        public ActionResult AddCategorie(string name, string description, bool home, string marque)
        {
            Int32.TryParse(marque, out int id);
            Categorie cat = new Categorie()
            {
                Nom = name,
                Description = description,
                IsHome = home,
                IdMarque = id
            };

            _categorieSrv.AddCategorie(cat);

            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Route("UpdateCategorie")]
        public ActionResult UpdateCategorie(string id, bool isHome)
        {
            Int32.TryParse(id, out int idCategorie);

            Categorie cat = _categorieSrv.GetCategorieById(idCategorie);

            _categorieSrv.ChangeStatusCategorie(cat);

            return RedirectToAction("Index", "Account");
        }
    }
}
