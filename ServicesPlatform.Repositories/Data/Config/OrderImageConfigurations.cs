using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Repositories.Data.Config
{
    internal class OrderImageConfigurations : IEntityTypeConfiguration<Order_Images>
    {
        public void Configure(EntityTypeBuilder<Order_Images> builder)
        {
            builder.HasOne(i => i.Order)
                .WithMany(i=>i.Images)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasKey(i => new { i.OrderId, i.ImageUrl });
        }
    }
}
