using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IAvisSrv
    {
        void AddAvis(int idProduit, int idClient, string commentaire, double note);

        List<Avis> GetAvisByProduit(int idProduit);

        List<Avis> GetAvisByClient(int idClient);

    }
}
