using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Repositories.Data
{
    public class AppIdentityDbContext:IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppIdentityDbContext).Assembly);
        }
        //public DbSet<AppUser> Users {  get; set; }
        public DbSet<Technician> Technicianes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Technicians_Departments> Technicians_Departments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order_Images> OrdersImages { get;set; }
        public DbSet<Address> Addresses { get; set; }

    }
}
