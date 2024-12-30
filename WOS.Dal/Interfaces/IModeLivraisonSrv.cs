using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IModeLivraisonSrv
    {
        void UpdatePriceLivraison(int id, float newPrice);
        ModeLivraison GetModeLivraisonById(int id);
        ModeLivraison GetModeLivraisonByName(string name);
    }
}
