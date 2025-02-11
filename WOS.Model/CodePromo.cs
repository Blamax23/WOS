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
    public class CodePromo
    {
        [Key]
        public int Id { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("pourcentage")]
        public int Pourcentage { get; set; }

        [Column("utilisation_count")]
        public int? UtilisationCount { get; set; }

        [Column("validity_date")]
        public DateTime? ValidityDate { get; set; } = DateTime.Now.AddDays(30);

        [Column("is_valid")]
        public bool? IsValid { get; set; }
    }
}