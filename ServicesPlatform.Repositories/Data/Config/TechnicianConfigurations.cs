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
    internal class TechnicianConfigurations : IEntityTypeConfiguration<Technician>
    {
        public void Configure(EntityTypeBuilder<Technician> builder)
        {
            builder.HasOne(t => t.User)
                .WithOne(t => t.Technician)
                .HasForeignKey<Technician>(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Departments)
                .WithOne(t => t.Technician)
                .HasForeignKey(t => t.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(t => t.Order)
                .WithOne(t => t.Technician)
                .HasForeignKey(t => t.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Reviews)
                .WithOne(t => t.Technician)
                .HasForeignKey(t => t.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
