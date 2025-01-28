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
    public class MarqueSrv : IMarqueSrv
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;

        public MarqueSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public List<Marque> GetMarquesByHome()
        {
            List<Marque> marques = _globalDataSrv.Marques.Where(m => m.IsHome == true).ToList();
            return marques;
        }

        public Marque GetMarqueById(int id)
        {
            Marque marque = _globalDataSrv.Marques.Find(m => m.Id == id);
            return marque;
        }

        public void AddMarque(Marque marque)
        {
            Console.WriteLine($"Saving Marque: Nom={marque.Nom}, Description={marque.Description}, IsHome={marque.IsHome}");

            _context.Marques.Add(marque);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log complet de l'exception
                Console.WriteLine($"Erreur lors de la sauvegarde : {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
            _globalDataSrv.RefreshCacheAsync(typeof(Marque));
        }

        public void UpdateHomeMarque(int id, bool tendance)
        {
            Marque marque = _context.Marques.FirstOrDefault(p => p.Id == id);

            if (marque == null)
                throw new Exception("Marque introuvable");

            marque.IsHome = tendance;

            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Marque));
        }

        public void DeleteMarque(int id)
        {
            Marque marque = _context.Marques.FirstOrDefault(p => p.Id == id);

            if (marque == null)
                throw new Exception("Marque introuvable");

            _context.Marques.Remove(marque);
            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Marque));
        }
    }
}
