using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class Adresse
    {
        [Key]
        public int Id { get; set; }

        [Column("client_id")]
        public int ClientId { get; set; }

        [StringLength(150)]
        public string? Nom { get; set; }

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

        public bool? PointRelais { get; set; }

        // Navigation properties
        public virtual Client Client { get; set; }
        public virtual ICollection<Commande> Commandes { get; set; }
    }

    public class PointRelais
    {
        public string Numero { get; set; }
        public string Adresse { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
