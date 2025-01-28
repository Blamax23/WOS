using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WOS.Model;
using System.Text.Json;
using WOS.Dal.Interfaces;
using WOS.Dal.Context;
using WOS.Back.Services;
using System;

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
        public ActionResult UpdateMarque(string id, string tendance)
        {
            try
            {
                Int32.TryParse(id, out int marqueId);
                bool.TryParse(tendance, out bool tend);

                _marqueSrv.UpdateHomeMarque(marqueId, tend);

                return Ok(new { errorMessage = "" });
            }
            catch (Exception e)
            {
                return Ok(new { errorMessage = "Erreur dans la modification du statut Tendance." });
            }
        }

        [HttpPost]
        [Route("DeleteMarque")]
        public ActionResult DeleteMarque(string id)
        {
            try
            {
                Int32.TryParse(id, out int marqueId);

                _marqueSrv.DeleteMarque(marqueId);

                return Ok(new { errorMessage = "" });
            }
            catch (Exception e)
            {
                return Ok(new { errorMessage = "Erreur dans la suppression de la marque." });
            }
        }
    }
}
