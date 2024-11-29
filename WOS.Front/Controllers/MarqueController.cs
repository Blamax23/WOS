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
    public class MarqueController : Controller
    {
        private readonly IMarqueSrv _marqueSrv;

        public MarqueController(IMarqueSrv marqueSrv)
        {
            _marqueSrv = marqueSrv;
        }

        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("AddMarque")]
        public ActionResult AddMarque(string name, string description, bool home)
        {

            Marque marque = new Marque()
            {
                Nom = name,
                Description = description,
                IsHome = home
            };

            _marqueSrv.AddMarque(marque);

            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [Route("UpdateMarque")]
        public ActionResult UpdateMarque(string id, bool isHome)
        {
            Int32.TryParse(id, out int idCategorie);

            Marque marque = _marqueSrv.GetMarqueById(idCategorie);

            _marqueSrv.ChangeStatusMarque(marque);

            return RedirectToAction("Index", "Account");
        }
    }
}
