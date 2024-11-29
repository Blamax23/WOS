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

    public static class ProduitCouleurExtensions
    {
        // Mapping couleur -> code hexadécimal
        private static readonly Dictionary<ProduitCouleurEnum, string> ColorHexMap = new()
    {
        { ProduitCouleurEnum.Blanc, "#FFFFFF" },
        { ProduitCouleurEnum.Noir, "#000000" },
        { ProduitCouleurEnum.Bleu, "#0000FF" },
        { ProduitCouleurEnum.Rouge, "#FF0000" },
        { ProduitCouleurEnum.Vert, "#008000" },
        { ProduitCouleurEnum.Jaune, "#FFFF00" },
        { ProduitCouleurEnum.Orange, "#FFA500" },
        { ProduitCouleurEnum.Violet, "#800080" },
        { ProduitCouleurEnum.Rose, "#FFC0CB" },
        { ProduitCouleurEnum.Marron, "#A52A2A" },
        { ProduitCouleurEnum.Gris, "#808080" },
        { ProduitCouleurEnum.Beige, "#F5F5DC" },
        { ProduitCouleurEnum.Turquoise, "#40E0D0" },
        { ProduitCouleurEnum.Kaki, "#BDB76B" },
        { ProduitCouleurEnum.Argent, "#C0C0C0" },
        { ProduitCouleurEnum.Or, "#FFD700" }
    };

        // Méthode pour récupérer le code hexadécimal
        public static string GetHexCode(this ProduitCouleurEnum couleur)
        {
            return ColorHexMap.TryGetValue(couleur, out var hexCode) ? hexCode : "#000000"; // Default: Noir
        }

        public static string GetName(this ProduitCouleurEnum couleur)
        {
            return couleur.GetAttribute<DisplayAttribute>()?.Name ?? couleur.ToString();
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)?.GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }
    }

}
