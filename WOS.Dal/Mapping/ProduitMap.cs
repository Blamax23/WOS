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
    public class ProduitMap : IEntityTypeConfiguration<Produit>
    {
        public void Configure(EntityTypeBuilder<Produit> builder)
        {
            builder.ToTable("PRODUIT");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(p => p.Nom)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("nom");

            builder.Property(p => p.Description)
                .IsRequired(false)
                .HasColumnName("description");

            builder.Property(p => p.DateCreation)
                .HasDefaultValue(DateTime.Now)
                .HasColumnName("date_creation");

            builder.Property(p => p.Actif)
                .HasDefaultValue(true)
                .HasColumnName("actif");

            builder.Property(p => p.IsTendance)
                .HasDefaultValue(true)
                .HasColumnName("IsTendance");

            builder.HasIndex(p => p.CategorieId).HasDatabaseName("categorie_id");
            builder.HasIndex(p => p.MarqueId).HasDatabaseName("marque_id");
        }
    }
}
