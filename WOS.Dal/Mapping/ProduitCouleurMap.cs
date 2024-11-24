using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Mapping
{
    public class ProduitCouleurMap : IEntityTypeConfiguration<ProduitCouleur>
    {
        public void Configure(EntityTypeBuilder<ProduitCouleur> builder)
        {
            builder.ToTable("PRODUIT_COULEUR");

            builder.HasKey(pc => pc.Id);

            builder.Property(pc => pc.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(pc => pc.Couleur)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("couleur");

            builder.Property(pc => pc.CodeHex)
                .HasMaxLength(7)
                .HasColumnName("code_hex");

            builder.HasOne(pc => pc.Produit)
                .WithMany(p => p.ProduitCouleurs)
                .HasForeignKey(pc => pc.ProduitId);

            builder.HasIndex(pc => pc.ProduitId).HasDatabaseName("produit_id");
            builder.HasIndex(pc => new { pc.ProduitId, pc.Couleur })
                .IsUnique();
        }
    }
}
