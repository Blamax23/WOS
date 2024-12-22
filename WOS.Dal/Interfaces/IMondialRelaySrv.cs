using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IMondialRelaySrv
    {
        Task<List<PointRelais>> RechercherPointsRelais(string pays, string ville, string codePostal, string taille = "", string poids = "", string action = "");

        Task ExempleRecherche();
    }
}
