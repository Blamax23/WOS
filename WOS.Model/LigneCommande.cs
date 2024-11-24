using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class LigneCommande
    {
        [Key]
        public int Id { get; set; }

        public int CommandeId { get; set; }
        public int ProduitId { get; set; }
        public int ProduitTailleId { get; set; }
        public int ProduitCouleurId { get; set; }

        [Required]
        public int Quantite { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PrixUnitaire { get; set; }

        // Navigation properties
        public virtual Commande Commande { get; set; }
        public virtual Produit Produit { get; set; }
        public virtual ProduitTaille ProduitTaille { get; set; }
        public virtual ProduitCouleur ProduitCouleur { get; set; }
    }
}
