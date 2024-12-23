using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class ModeLivraison
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nom")]
        [MaxLength(50)]
        public string Nom { get; set; }

        [Column("path_logo")]
        [MaxLength(255)]
        public string? PathLogo { get; set; }

        [Column("prix_livraison")]
        public float PrixLivraison { get; set; }
    }
}
