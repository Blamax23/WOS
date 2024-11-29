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
    public class ProduitImageMap : IEntityTypeConfiguration<ProduitImage>
    {
        public void Configure(EntityTypeBuilder<ProduitImage> builder)
        {
            builder.ToTable("PRODUIT_IMAGE");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(pi => pi.Url)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("url");

            builder.Property(pi => pi.Principale)
                .HasDefaultValue(false)
                .HasColumnName("principale");

            builder.Property(pc => pc.ProduitId)
                .HasColumnName("produit_id");

            builder.HasOne(pi => pi.Produit)
                .WithMany(p => p.ProduitImages)
                .HasForeignKey(pi => pi.ProduitId);

            builder.HasIndex(pi => pi.ProduitId).HasDatabaseName("produit_id");
        }
    }
}
