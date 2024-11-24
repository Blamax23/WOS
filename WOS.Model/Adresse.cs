using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class Adresse
    {
        [Key]
        public int Id { get; set; }

        public int ClientId { get; set; }

        [Required]
        [StringLength(255)]
        public string Rue { get; set; }

        [Required]
        [StringLength(100)]
        public string Ville { get; set; }

        [Required]
        [StringLength(10)]
        public string CodePostal { get; set; }

        [Required]
        [StringLength(100)]
        public string Pays { get; set; }

        public bool Principale { get; set; }

        // Navigation properties
        public virtual Client Client { get; set; }
        public virtual ICollection<Commande> Commandes { get; set; }
    }
}
