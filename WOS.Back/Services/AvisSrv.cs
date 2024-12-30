using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;
using WOS.Dal.Context;
using WOS.Dal.Interfaces;

namespace WOS.Back.Services
{
    public class AvisSrv : IAvisSrv
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;

        public AvisSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public void AddAvis(int idProduit, int idClient, string commentaire, double note)
        {
            Avis avis = new Avis
            {
                ProduitId = idProduit,
                ClientId = idClient,
                Commentaire = commentaire,
                Note = note,
                DateAvis = DateTime.Now
            };

            _context.Avis.Add(avis);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Avis));
        }

        public List<Avis> GetAvisByProduit(int idProduit)
        {
            return _globalDataSrv.Avis.Where(a => a.ProduitId == idProduit).ToList();
        }

        public List<Avis> GetAvisByClient(int idClient)
        {
            return _globalDataSrv.Avis.Where(a => a.ClientId == idClient).ToList();
        }
    }
}
