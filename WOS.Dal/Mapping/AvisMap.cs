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
    public class AvisMap: IEntityTypeConfiguration<Avis>
    {
        public void Configure(EntityTypeBuilder<Avis> builder)
        {
            builder.ToTable("AVIS");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(a => a.Note)
                .IsRequired()
                .HasColumnName("note");

            builder.Property(a => a.Commentaire)
                .IsRequired(false)
                .HasColumnName("commentaire");

            builder.Property(a => a.DateAvis)
                .HasDefaultValue(DateTime.Now)
                .HasColumnName("date_avis");

            builder.HasOne(a => a.Client)
                .WithMany(c => c.Avis)
                .HasForeignKey(a => a.ClientId);

            builder.HasOne(a => a.Produit)
                .WithMany(p => p.Avis)
                .HasForeignKey(a => a.ProduitId);

            builder.HasIndex(a => a.ProduitId).HasDatabaseName("produit_id");
            builder.HasIndex(a => a.ClientId).HasDatabaseName("client_id");
        }
    }
}
