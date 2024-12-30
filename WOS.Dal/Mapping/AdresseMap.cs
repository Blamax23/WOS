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
    public class AdresseMap : IEntityTypeConfiguration<Adresse>
    {
        public void Configure(EntityTypeBuilder<Adresse> builder)
        {
            builder.ToTable("ADRESSE");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(a => a.Nom)
                .IsRequired(false)
                .HasMaxLength(150)
                .HasColumnName("nom");

            builder.Property(a => a.Rue)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("rue");

            builder.Property(a => a.Ville)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ville");

            builder.Property(a => a.CodePostal)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("code_postal");

            builder.Property(a => a.Pays)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("pays");

            builder.Property(a => a.Principale)
                .HasDefaultValue(false)
                .HasColumnName("principale");

            builder.Property(a => a.PointRelais)
                .HasDefaultValue(false)
                .HasColumnName("point_relais");

            builder.HasOne(a => a.Client)
                .WithMany(c => c.Adresses)
                .HasForeignKey(a => a.ClientId);

            builder.HasIndex(a => a.ClientId).HasDatabaseName("client_id");
        }
    }
}
