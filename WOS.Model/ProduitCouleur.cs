using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class ProduitCouleur
    {
        [Key]
        public int Id { get; set; }

        public int ProduitId { get; set; }

        [Required]
        [StringLength(50)]
        public string Couleur { get; set; }

        [StringLength(7)]
        public string CodeHex { get; set; }

        // Navigation property
        public virtual Produit Produit { get; set; }
        public virtual ICollection<LigneCommande> LignesCommande { get; set; }
    }

    public enum ProduitCouleurEnum
    {
        [Display(Name = "Blanc")]
        Blanc,
        [Display(Name = "Noir")]
        Noir,
        [Display(Name = "Bleu")]
        Bleu,
        [Display(Name = "Rouge")]
        Rouge,
        [Display(Name = "Vert")]
        Vert,
        [Display(Name = "Jaune")]
        Jaune,
        [Display(Name = "Orange")]
        Orange,
        [Display(Name = "Violet")]
        Violet,
        [Display(Name = "Rose")]
        Rose,
        [Display(Name = "Marron")]
        Marron,
        [Display(Name = "Gris")]
        Gris,
        [Display(Name = "Beige")]
        Beige,
        [Display(Name = "Turquoise")]
        Turquoise,
        [Display(Name = "Kaki")]
        Kaki,
        [Display(Name = "Argent")]
        Argent,
        [Display(Name = "Or")]
        Or
    }
}
