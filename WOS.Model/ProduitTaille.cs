using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? PrixPromo { get; set; }

        // Navigation property
        public virtual Produit Produit { get; set; }
        public virtual ICollection<LigneCommande> LignesCommande { get; set; }
    }

    public class StockItem
    {
        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("quantity")]
        public string Quantity { get; set; }

        [JsonPropertyName("price")]
        public string Price { get; set; }

        [JsonPropertyName("priceProm")]
        public string PriceProm { get; set; }
    }

}
