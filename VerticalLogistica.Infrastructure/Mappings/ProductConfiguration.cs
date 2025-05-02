using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalLogistica.Domain.Entities;

namespace VerticalLogistica.Infrastructure.Mappings
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.ProductId).IsRequired(); // ID lógico do arquivo
            builder.Property(p => p.Value)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.OrderId).IsRequired(); // FK clara
        }

    }

}
