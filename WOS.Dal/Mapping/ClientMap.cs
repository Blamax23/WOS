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
    public class ClientMap : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("CLIENT");

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

            builder.Property(c => c.Telephone)
                .IsRequired()
                .HasColumnName("numero_telephone");

            builder.Property(c => c.DateInscription)
                .HasDefaultValue(DateTime.Now)
                .HasColumnName("date_inscription");

            builder.HasIndex(c => c.Email)
                .IsUnique();

            builder.Property(c => c.IsEmailVerified)
                .HasDefaultValue(false)
                .IsRequired()
                .HasColumnName("is_email_verified");

            builder.Property(c => c.VerificationToken)
                .HasMaxLength(255)
                .IsRequired(false)
                .HasColumnName("verification_token");

            builder.Property(c => c.TokenExpiryDate)
                .HasMaxLength(255)
                .HasColumnName("token_expiry_date");
        }
    }
}
