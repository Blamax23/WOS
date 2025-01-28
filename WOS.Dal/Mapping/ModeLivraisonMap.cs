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
    public class ModeLivraisonMap : IEntityTypeConfiguration<ModeLivraison>
    {
        public void Configure(EntityTypeBuilder<ModeLivraison> builder)
        {
            builder.ToTable("MODE_LIVRAISON");

            builder.HasKey(ml => ml.Id);

            builder.Property(ml => ml.Id)
                .HasColumnName("id");

            builder.Property(ml => ml.Nom)
                .HasColumnName("nom")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(ml => ml.PathLogo)
                .HasColumnName("path_logo")
                .HasMaxLength(255);

            builder.Property(ml => ml.JoursLivraisonMini)
                .HasColumnName("jours_livraison_mini");

            builder.Property(ml => ml.PrixLivraison)
                .HasColumnName("prix_livraison");
        }
    }
}

