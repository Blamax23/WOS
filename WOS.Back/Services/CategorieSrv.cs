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
    public class CategorieSrv : ICategorieSrv
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;

        public CategorieSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public List<Categorie> GetCategoriesByHome()
        {
            var categories = _globalDataSrv.Categories.Where(c => c.IsHome == true).ToList();
            return categories;
        }

        public Categorie GetCategorieById(int id)
        {
            Categorie Categorie = _globalDataSrv.Categories.Find(c => c.Id == id);
            return Categorie;
        }

        public void AddCategorie(Categorie Categorie)
        {
            _context.Categories.Add(Categorie);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Categorie));
        }

        public void UpdateHomeCategory(int id, bool tendance)
        {
            Categorie cat = _context.Categories.FirstOrDefault(p => p.Id == id);

            if (cat == null)
                throw new Exception("Catégorie introuvable");

            cat.IsHome = tendance;

            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Categorie));
        }
    }
}
