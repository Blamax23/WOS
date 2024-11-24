using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WOS.Dal.Mapping
{
    public class CategorieMap : IEntityTypeConfiguration<Categorie>
    {
        public void Configure(EntityTypeBuilder<Categorie> builder)
        {
            builder.ToTable("CATEGORIE");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd().HasColumnName("Id");

            builder.Property(c => c.Nom)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("nom");

            builder.Property(c => c.Description)
                .IsRequired(false)
                .HasColumnName("description");

            builder.Property(c => c.IdMarque)
                .IsRequired(true)
                .HasColumnName("IdMarque");

            builder.Property(c => c.IsHome)
                .IsRequired(true)
                .HasDefaultValue(false)
                .HasColumnName("IsHome");

            builder.HasMany(c => c.Produits)
                .WithOne(p => p.Categorie)
                .HasForeignKey(p => p.CategorieId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
