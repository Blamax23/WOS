using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class StatutCommande
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Libelle { get; set; }

        // Navigation property
        public virtual ICollection<Commande> Commandes { get; set; }
    }

    public enum StatutCommandeEnum
    {
        EnAttenteDePaiement = 1,
        Payee = 2,
        EnPreparation = 3,
        Expediee = 4,
        Livree = 5,
        Annulee = 6
    }
}
