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
    public class CodePromoMap : IEntityTypeConfiguration<CodePromo>
    {
        public void Configure(EntityTypeBuilder<CodePromo> builder)
        {
            builder.ToTable("CODE_PROMO");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(c => c.Nom)
                .HasColumnName("nom");

            builder.Property(c => c.Pourcentage)
                .HasColumnName("pourcentage");

            builder.Property(c => c.UtilisationCount)
                .HasColumnName("utilisation_count");

            builder.Property(c => c.ValidityDate)
                .HasColumnName("validity_date");

            builder.Property(c => c.IsValid)
                .HasColumnName("is_valid");
        }
    }
}
