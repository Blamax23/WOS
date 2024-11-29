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

        public CategorieSrv(WOSDbContext context)
        {
            _context = context;
        }

        public List<Categorie> GetAllCategories()
        {
            List<Categorie> Categories = _context.Categories.ToList();
            return Categories;
        }

        public Categorie GetCategorieById(int id)
        {
            Categorie Categorie = _context.Categories.Find(id);
            return Categorie;
        }

        public void AddCategorie(Categorie Categorie)
        {
            _context.Categories.Add(Categorie);
            _context.SaveChanges();
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
            }
        }
    }
}
