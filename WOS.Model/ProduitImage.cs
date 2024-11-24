using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class ProduitImage
    {
        [Key]
        public int Id { get; set; }

        public int ProduitId { get; set; }

        [Required]
        [StringLength(255)]
        public string Url { get; set; }

        public bool Principale { get; set; }

        public string Name { get; set; }

        // Navigation property
        public virtual Produit Produit { get; set; }
    }
}
