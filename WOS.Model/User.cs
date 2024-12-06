using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class UserWOS
    {
    }

    public class AccountViewModel
    {
        public UserWOS User { get; set; }

        public List<Commande> Commandes { get; set; }

        public List<Produit> Produits { get; set; }

        public List<Marque> Marques { get; set; }

        public List<Categorie> Categories { get; set; }
    }

    public class HomeViewModel
    {
        public List<RowHomeModel> RowHome { get; set; } = new List<RowHomeModel>();
    }

    public class RowHomeModel
    {
        public string Name { get; set; }

        public List<Produit> Produits { get; set; }

    }

    public class ProductViewModel
    {
        public List<Produit> Produits { get; set; }

        public List<Marque> Marques { get; set; }

        public List<Categorie> Categories { get; set; }
    }
}
