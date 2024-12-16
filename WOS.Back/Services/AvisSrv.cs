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

        public AvisSrv(WOSDbContext context)
        {
            _context = context;
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
        }

        public List<Avis> GetAvisByProduit(int idProduit)
        {
            return _context.Avis.Where(a => a.ProduitId == idProduit).ToList();
        }

        public List<Avis> GetAvisByClient(int idClient)
        {
            return _context.Avis.Where(a => a.ClientId == idClient).ToList();
        }
    }
}
