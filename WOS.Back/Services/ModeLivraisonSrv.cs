using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Dal.Context;
using WOS.Dal.Interfaces;
using WOS.Model;

namespace WOS.Back.Services
{
    public class ModeLivraisonSrv : IModeLivraisonSrv
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;

        public ModeLivraisonSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public ModeLivraison GetModeLivraisonById(int id)
        {
            return _globalDataSrv.ModeLivraisons.FirstOrDefault(ml => ml.Id == id);
        }

        public ModeLivraison GetModeLivraisonByName(string name)
        {
            return _globalDataSrv.ModeLivraisons.FirstOrDefault(ml => ml.Nom == name);
        }

        public void UpdatePriceLivraison(int id, float newPrice)
        {
            ModeLivraison modeLivraison = _context.ModeLivraisons.FirstOrDefault(ml => ml.Id == id);

            modeLivraison.PrixLivraison = newPrice;

            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(ModeLivraison));
        }
    }
}
