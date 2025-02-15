using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class Client : UserWOS
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string MotDePasse { get; set; }

        [Required]
        [StringLength(500)]
        public string AncienMotDePasse { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        [StringLength(100)]
        public string Prenom { get; set; }

        [Required]
        public string? Telephone { get; set; }

        public string? VerificationToken { get; set; }

        public DateTime? TokenExpiryDate { get; set; }

        public bool? IsEmailVerified { get; set; }

        public DateTime DateInscription { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Adresse> Adresses { get; set; }
        public virtual ICollection<Commande> Commandes { get; set; }
        public virtual ICollection<Avis> Avis { get; set; }

    }

    public class SignInViewModel
    {
        public Client Client { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class LogInViewModel
    {
        public string Email { get; set; }

        public string MotDePasse { get; set; }

        public string ErrorMessage { get; set; }
    }
}
