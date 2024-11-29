using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IProduitSrv
    {
        List<Produit> GetProduits();
        Produit GetProduitById(int id);
        void AddProduit(Produit produit);
        List<double> GetAllTailles();
        void UpdateProduit(Produit produit);
    }
}
