using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class Marque
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Nom { get; set; }

        public string? Description { get; set; }

        public bool? IsHome { get; set; }

        // Navigation property
        public virtual ICollection<Produit> Produits { get; set; }
    }
}
