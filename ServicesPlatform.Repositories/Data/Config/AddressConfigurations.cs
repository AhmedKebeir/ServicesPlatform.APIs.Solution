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
    internal class AddressConfigurations : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasOne(a => a.User)
                 .WithMany(a => a.Addresses)
                 .HasForeignKey(a => a.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(us => new { us.UserId, us.City, us.Center,us.Street })
                .IsUnique();
        }
    }
}
