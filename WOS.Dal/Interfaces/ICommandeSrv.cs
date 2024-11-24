using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface ICommandeSrv
    {
        List<Commande> GetCommandes();
        Commande GetCommandeById(int id);
        List<Commande> GetCommandesByClientId(int id);
        void AddCommande(Commande commande);
    }
}
