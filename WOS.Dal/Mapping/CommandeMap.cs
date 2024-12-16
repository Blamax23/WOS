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
    public class CommandeMap : IEntityTypeConfiguration<Commande>
    {
        public void Configure(EntityTypeBuilder<Commande> builder)
        {
            builder.ToTable("COMMANDE");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(c => c.MontantTotal)
                .HasPrecision(10, 2)
                .IsRequired()
                .HasColumnName("montant_total");

            builder.Property(c => c.DateCommande)
                .HasDefaultValue(DateTime.Now)
                .HasColumnName("date_commande");

            builder.Property(c => c.NumeroCommandeLivreur)
                .IsRequired(false)
                .HasColumnName("numero_commande_livreur");

            builder.Property(c => c.LinkSuivi)
                .IsRequired(false)
                .HasColumnName("link_suivi");

            builder.Property(c => c.NumeroCommande)
                .IsRequired(true)
                .HasColumnName("numero_commande");

            builder.HasOne(c => c.Client)
                .WithMany(cl => cl.Commandes)
                .HasForeignKey(c => c.ClientId);

            builder.HasOne(c => c.AdresseLivraison)
                .WithMany(a => a.Commandes)
                .HasForeignKey(c => c.AdresseLivraisonId);

            builder.HasIndex(c => c.ClientId).HasDatabaseName("client_id");
            builder.HasIndex(c => c.StatutId).HasDatabaseName("statut_id");
            builder.HasIndex(c => c.AdresseLivraisonId).HasDatabaseName("adresse_livraison_id");
        }
    }
}
