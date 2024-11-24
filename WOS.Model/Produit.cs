﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
    }
}