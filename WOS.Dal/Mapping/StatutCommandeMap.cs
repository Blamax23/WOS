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
    public class StatutCommandeMap : IEntityTypeConfiguration<StatutCommande>
    {
        public void Configure(EntityTypeBuilder<StatutCommande> builder)
        {
            builder.ToTable("STATUT_COMMANDE");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(s => s.Libelle)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("libelle");

            builder.HasMany(s => s.Commandes)
                .WithOne(c => c.Statut)
                .HasForeignKey(c => c.StatutId);
        }
    }
}
