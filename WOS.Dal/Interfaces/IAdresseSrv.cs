using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IAdresseSrv
    {
        void UpdateAdresse(Adresse adresse);
        Adresse GetAdresseById(int id);
        List<Adresse> GetAdresseByUserId(int userId);
        void AddAdresse(Adresse adresse);
        void ChangeAdressePrincipale();
        Adresse GetAdressePrincipaleByUserId(int id);
    }
}
