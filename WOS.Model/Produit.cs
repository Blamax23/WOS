using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WOS.Model
{
    public class Produit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nom { get; set; }

        public string Description { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public bool Actif { get; set; } = true;

        [Column("categorie_id")]
        public int? CategorieId { get; set; }

        [Column("marque_id")]
        public int? MarqueId { get; set; }
        public bool IsTendance { get; set; }

        // Navigation properties
        public virtual Categorie Categorie { get; set; }
        public virtual Marque Marque { get; set; }
        public virtual ICollection<ProduitTaille> ProduitTailles { get; set; }
        public virtual ICollection<ProduitCouleur> ProduitCouleurs { get; set; }
        public virtual ICollection<ProduitImage> ProduitImages { get; set; }
        public virtual ICollection<Avis> Avis { get; set; }
        public virtual ICollection<LigneCommande> LignesCommande { get; set; }

        public Produit()
        {

        }
        public Produit(Produit other)
        {
            // Copy constructor
            Id = other.Id;
            Nom = other.Nom;
            Description = other.Description;
            DateCreation = other.DateCreation;
            Actif = other.Actif;
            CategorieId = other.CategorieId;
            MarqueId = other.MarqueId;
            IsTendance = other.IsTendance;
            Categorie = other.Categorie;
            Marque = other.Marque;
            ProduitTailles = other.ProduitTailles;
            ProduitCouleurs = other.ProduitCouleurs;
            ProduitImages = other.ProduitImages;
            Avis = other.Avis;
            LignesCommande = other.LignesCommande;
        }
    }

    public class CartItem
    {
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("url")]
        public string ImageUrl { get; set; }
    }

    public class ProductsListViewModel
    {
        public List<Produit> Produits { get; set; }

        public PaginationModel Pagination { get; set; }

    }

    public class PaginationModel
    {
        public int Page { get; set; }

        public int NbPages { get; set; }
    }
}
