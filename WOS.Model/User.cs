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
    }
}
