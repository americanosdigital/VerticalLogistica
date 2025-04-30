using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VerticalLogistica.Domain.Entities;

namespace VerticalLogistica.Infrastructure.Mappings
{
    public class RawOrderConfiguration : IEntityTypeConfiguration<RawOrder>
    {
        public void Configure(EntityTypeBuilder<RawOrder> builder)
        {
            builder.ToTable("RawOrders");

            builder.HasKey(ro => new { ro.UserId, ro.OrderId, ro.ProductId }); // Composto

            builder.Property(ro => ro.UserId)
                .IsRequired();

            builder.Property(ro => ro.UserName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(ro => ro.OrderId)
                .IsRequired();

            builder.Property(ro => ro.ProductId)
                .IsRequired();

            builder.Property(ro => ro.ProductValue)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(ro => ro.PurchaseDate)
                .HasColumnType("datetime")
                .IsRequired();
        }
    }
}
