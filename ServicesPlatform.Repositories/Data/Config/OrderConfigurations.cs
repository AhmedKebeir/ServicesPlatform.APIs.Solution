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
    internal class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.Address, X => X.WithOwner());

            builder.HasOne(o => o.User)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Department)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Review)
                .WithOne(o => o.Order)
                .HasForeignKey(o=> o.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(O => O.Status)
                .HasConversion(
                OStatus => OStatus.ToString(),
                OStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), OStatus));

        }
    }
}
