using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WOS.Model;
using System.Text.Json;
using WOS.Dal.Interfaces;
using WOS.Dal.Context;
using WOS.Back.Services;

namespace WOS.Front.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class CategorieController : Controller
    {
        private readonly IGlobalDataSrv _globalDataSrv;
        private readonly ICategorieSrv _categorieSrv;

        public CategorieController(IGlobalDataSrv globalDataSrv, ICategorieSrv categorieSrv)
        {
            _globalDataSrv = globalDataSrv;
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

            _globalDataSrv.RefreshCacheAsync(typeof(Categorie));

            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Route("UpdateTendanceCategory")]
        public ActionResult UpdateTendanceCategory(string id, string tendance)
        {
            try
            {
                Int32.TryParse(id, out int catId);
                bool.TryParse(tendance, out bool tend);

                _categorieSrv.UpdateHomeCategory(catId, tend);

                return Ok(new { errorMessage = "" });
            }
            catch (Exception e)
            {
                return Ok(new { errorMessage = "Erreur dans la modification du statut Tendance." });
            }
        }
    }
}
