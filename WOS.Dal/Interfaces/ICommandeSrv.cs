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
        Commande GetCommandeById(int id);
        List<Commande> GetCommandesByClientId(int id);
        void AddCommande(Commande commande);

        Commande GetCommandeByNumberOrder(string number);
        Commande GetCommandeByNumberOrderDelivery(string number);
        void UpdateStatus(int idCommande);
        void UpdateCommande(Commande commande);

        void AddCodePromo(CodePromo codePromo);
        void DeleteCodePromo(int id);
    }
}
