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

        public MarqueSrv(WOSDbContext context)
        {
            _context = context;
        }

        public List<Marque> GetAllMarques()
        {
            List<Marque> marques = _context.Marques.ToList();
            return marques;
        }

        public List<Marque> GetMarquesByHome()
        {
            List<Marque> marques = _context.Marques.Where(m => m.IsHome == true).ToList();
            return marques;
        }

        public Marque GetMarqueById(int id)
        {
            Marque marque = _context.Marques.Find(id);
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
        }

        public void ChangeStatusMarque(Marque marque)
        {
            Marque newMarque = _context.Marques.Find(marque.Id);
            if (newMarque != null)
            {
                if (!newMarque.IsHome.Value)
                    newMarque.IsHome = true;
                else
                    newMarque.IsHome = false;

                _context.Marques.Update(newMarque);

                _context.SaveChanges();
            }
        }
    }
}
