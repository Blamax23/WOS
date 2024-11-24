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
    public class LigneCommandeMap : IEntityTypeConfiguration<LigneCommande>
    {
        public void Configure(EntityTypeBuilder<LigneCommande> builder)
        {
            builder.ToTable("LIGNE_COMMANDE");

            builder.HasKey(lc => lc.Id);

            builder.Property(lc => lc.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(lc => lc.Quantite)
                .IsRequired()
                .HasColumnName("quantite");

            builder.Property(lc => lc.PrixUnitaire)
                .HasPrecision(10, 2)
                .IsRequired()
                .HasColumnName("prix_unitaire");

            builder.HasOne(lc => lc.Commande)
                .WithMany(c => c.LignesCommande)
                .HasForeignKey(lc => lc.CommandeId);

            builder.HasOne(lc => lc.Produit)
                .WithMany(p => p.LignesCommande)
                .HasForeignKey(lc => lc.ProduitId);

            builder.HasOne(lc => lc.ProduitTaille)
                .WithMany(pt => pt.LignesCommande)
                .HasForeignKey(lc => lc.ProduitTailleId);

            builder.HasOne(lc => lc.ProduitCouleur)
                .WithMany(pc => pc.LignesCommande)
                .HasForeignKey(lc => lc.ProduitCouleurId);

            builder.HasIndex(lc => lc.CommandeId).HasDatabaseName("commande_id");
        }
    }
}
