using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class Admin : UserWOS
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(500)]
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

        public int? Code { get; set; }

        public DateTime? CodeExpirationDate { get; set; }
    }
}
