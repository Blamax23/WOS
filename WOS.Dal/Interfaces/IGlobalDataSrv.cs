using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IGlobalDataSrv
    {
        Task RefreshCacheAsync(Type type);
        Task RefreshAllCacheAsync();
        public List<Categorie> Categories { get; set; }
        public List<Marque> Marques { get; set; }
        public List<Produit> Produits { get; set; }
        public List<ProduitTaille> ProduitTailles { get; set; }
        public List<ProduitCouleur> ProduitCouleurs { get; set; }
        public List<ProduitImage> ProduitImages { get; set; }
        public List<Client> Clients { get; set; }
        public List<Adresse> Adresses { get; set; }
        public List<Commande> Commandes { get; set; }
        public List<LigneCommande> LignesCommande { get; set; }
        public List<StatutCommande> StatutsCommande { get; set; }
        public List<Avis> Avis { get; set; }
        public List<Admin> Admins { get; set; }
        public List<Question> Questions { get; set; }
        public List<ModeLivraison> ModeLivraisons { get; set; }
        public List<CodePromo> CodePromos { get; set; }
    }
}
