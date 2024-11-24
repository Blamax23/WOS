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
    public class MarqueMap : IEntityTypeConfiguration<Marque>
    {
        public void Configure(EntityTypeBuilder<Marque> builder)
        {
            builder.ToTable("MARQUE");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(m => m.Nom)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("nom");

            builder.Property(m => m.Description)
                .IsRequired(false)
                .HasColumnName("description");

            builder.Property(m => m.IsHome)
                .IsRequired(true)
                .HasDefaultValue(false)
                .HasColumnName("IsHome");

            builder.HasMany(m => m.Produits)
                .WithOne(p => p.Marque)
                .HasForeignKey(p => p.MarqueId)
                .IsRequired(false);
        }
    }
}
