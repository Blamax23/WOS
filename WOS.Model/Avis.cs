using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class Avis
    {
        [Key]
        public int Id { get; set; }

        public int ClientId { get; set; }
        public int ProduitId { get; set; }

        [Required]
        [Range(0, 5)]
        public double Note { get; set; }

        public string Commentaire { get; set; }

        public string CodeAvis { get; set; }

        public DateTime DateAvis { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Client Client { get; set; }
        public virtual Produit Produit { get; set; }
    }

    public class AvisViewModel
    {
        public List<Produit> Produits { get; set; } = new List<Produit>();

        public string CodeAvis { get; set; }

        public bool IsPosted { get; set; }
    }
}
