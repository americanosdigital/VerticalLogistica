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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Property(o => o.OrderId).IsRequired(); // ID lógico
            builder.Property(o => o.Date).IsRequired();
            builder.Property(o => o.Total).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasMany(o => o.Products)
                   .WithOne(p => p.Order)
                   .HasForeignKey(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
