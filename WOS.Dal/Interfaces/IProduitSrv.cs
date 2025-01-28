using Microsoft.EntityFrameworkCore;
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
        List<Produit> GetProduitsTendance();
        List<Produit> GetProduitsByMarque(int idMarque);
        List<Produit> GetProduitsByCat(int idCat);
        Produit GetProduitById(int id);
        void AddProduit(Produit produit);
        List<double> GetAllTailles();
        void UpdateProduit(Produit produit);
        public void UpdateActiveProduit(int id, bool actif);
        public void UpdateTendanceProduit(int id, bool tendance);
        void DeleteProduit(int id);
    }
}
