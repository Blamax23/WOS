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
    public class ProduitTailleMap : IEntityTypeConfiguration<ProduitTaille>
    {
        public void Configure(EntityTypeBuilder<ProduitTaille> builder)
        {
            builder.ToTable("PRODUIT_TAILLE");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(pt => pt.Taille)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("taille");

            builder.Property(pt => pt.Stock)
                .IsRequired()
                .HasDefaultValue(0)
                .HasColumnName("stock");

            builder.HasOne(pt => pt.Produit)
                .WithMany(p => p.ProduitTailles)
                .HasForeignKey(pt => pt.ProduitId);

            builder.HasIndex(pt => pt.ProduitId).HasDatabaseName("produit_id");
            builder.HasIndex(pt => new { pt.ProduitId, pt.Taille })
                .IsUnique();
        }
    }
}
