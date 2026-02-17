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
    internal class Technicians_DepartmentsConfigurations : IEntityTypeConfiguration<Technicians_Departments>
    {
        public void Configure(EntityTypeBuilder<Technicians_Departments> builder)
        {
            builder.HasOne(td => td.Department)
                .WithMany()
                .HasForeignKey(td => td.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasKey(td => new { td.DepartmentId, td.TechnicianId });

            builder.Ignore(td => td.Id);
        }
    }
}
