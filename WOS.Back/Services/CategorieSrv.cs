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

        public void ChangeStatusCategorie(Categorie Categorie)
        {
            Categorie newCategorie = _context.Categories.Find(Categorie.Id);
            if (newCategorie != null)
            {
                if (!newCategorie.IsHome.Value)
                    newCategorie.IsHome = true;
                else
                    newCategorie.IsHome = false;

                _context.Categories.Update(newCategorie);

                _context.SaveChanges();
                _globalDataSrv.RefreshCacheAsync(typeof(Categorie));
            }
        }
    }
}
