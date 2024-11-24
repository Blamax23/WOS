using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class Categorie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        public string Description { get; set; }

        public int IdMarque { get; set; }

        public bool IsHome { get; set; }

        // Navigation property
        public virtual ICollection<Produit> Produits { get; set; }
    }
}
