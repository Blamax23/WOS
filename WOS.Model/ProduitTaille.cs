using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class ProduitTaille
    {
        [Key]
        public int Id { get; set; }

        public int ProduitId { get; set; }

        [Required]
        [StringLength(10)]
        public string Taille { get; set; }

        public int Stock { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Prix { get; set; }

        // Navigation property
        public virtual Produit Produit { get; set; }
        public virtual ICollection<LigneCommande> LignesCommande { get; set; }
    }

    public class StockItem
    {
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
