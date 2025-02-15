using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace WOS.Model
{
    public class Commande
    {
        [Key]
        public int Id { get; set; }

        [Column("client_id")]
        public int ClientId { get; set; }

        [Column("adresse_livraison_id")]
        public int AdresseLivraisonId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal MontantTotal { get; set; }

        public DateTime DateCommande { get; set; } = DateTime.Now;

        [Column("statut_id")]
        public int StatutId { get; set; }

        [Column("numero_commande_livreur")]
        public string? NumeroCommandeLivreur { get; set; }

        [Column("link_suivi")]
        public string? LinkSuivi { get; set; }

        [Column("numero_commande")]
        public string NumeroCommande { get; set; }

        [Column("mode_livraison_id")]
        public int? ModeLivraisonId { get; set; }

        [Column("binary_etiquette")]
        public byte[]? BinaryEtiquette { get; set; }

        [Column("binary_facture")]
        public byte[]? BinaryFacture { get; set; }

        // Navigation properties
        public virtual Client Client { get; set; }
        public virtual Adresse AdresseLivraison { get; set; }
        public virtual StatutCommande Statut { get; set; }
        public virtual List<LigneCommande> LignesCommande { get; set; } = new List<LigneCommande>();
    }

    public class DeliveryInfo
    {

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("postalCode")]
        public string? ZipCode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("modeName")]
        public string? ModeName { get; set; }

        [JsonPropertyName("parcelShop")]
        public ParcelShop? ParcelShop { get; set; }
    }

    public class ParcelShop
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("postalCode")]
        public string? ZipCode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }

    public class ViewFinalPurchase
    {
        public string OrderNumber { get; set; }
        public List<CartItem> Cart { get; set; }

        public DeliveryInfo DeliveryInfo { get; set; }

        public ModeLivraison ModeLivraison { get; set; }

        public string ModePaiement { get; set; }

        public string TotalPrice { get; set; }

        public string Domain { get; set; }
    }

    public class AmountDto
    {
        public string Amount { get; set; }
    }

    public class ConfirmPurchaseModel
    {
        public Commande Commande { get; set; }
        public List<Produit> Produits { get; set; } = new List<Produit>();
        public bool IsSuccess { get; set; }
    }
}
