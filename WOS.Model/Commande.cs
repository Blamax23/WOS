using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class Commande
    {
        [Key]
        public int Id { get; set; }

        [Column("client_id")]
        public int ClientId { get; set; }

        [Column("adresse_livraison_id")]
        public int AdresseLivraisonId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal MontantTotal { get; set; }

        public DateTime DateCommande { get; set; } = DateTime.Now;

        [Column("statut_id")]
        public int StatutId { get; set; }

        // Navigation properties
        public virtual Client Client { get; set; }
        public virtual Adresse AdresseLivraison { get; set; }
        public virtual StatutCommande Statut { get; set; }
        public virtual ICollection<LigneCommande> LignesCommande { get; set; }
    }
}
