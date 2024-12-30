using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface ICategorieSrv
    {
        List<Categorie> GetCategoriesByHome();

        Categorie GetCategorieById(int id);

        void AddCategorie(Categorie Categorie);

        void ChangeStatusCategorie(Categorie Categorie);
    }
}
