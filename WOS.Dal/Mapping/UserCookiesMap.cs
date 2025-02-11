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
    public class UserCookiesMap : IEntityTypeConfiguration<UserCookies>
    {
        public void Configure(EntityTypeBuilder<UserCookies> builder)
        {
            builder.ToTable("USER_COOKIES");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(c => c.UserId)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("user_id");

            builder.Property(c => c.ConsentGiven)
                .IsRequired()
                .HasColumnName("consent");

            builder.Property(c => c.DateGiven)
                .IsRequired()
                .HasColumnName("date");

            builder.Property(c => c.UserIp)
                .IsRequired()
                .HasMaxLength(45)
                .HasColumnName("user_ip");

            builder.Property(c => c.UserAgent)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("user_agent");
        }
    }
}
