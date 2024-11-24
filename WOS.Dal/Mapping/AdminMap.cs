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
    public class AdminMap : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.ToTable("ADMIN");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("email");

            builder.Property(c => c.MotDePasse)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("mot_de_passe");

            builder.Property(c => c.AncienMotDePasse)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("ancien_mot_de_passe");

            builder.Property(c => c.Nom)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("nom");

            builder.Property(c => c.Prenom)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("prenom");

            builder.Property(c => c.Code)
                .HasColumnName("CODE");

            builder.Property(c => c.CodeExpirationDate)
                .HasColumnName("code_expiration_time");

            builder.HasIndex(c => c.Email)
                .IsUnique();
        }
    }
}
