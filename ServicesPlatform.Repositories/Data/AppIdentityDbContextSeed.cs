using Microsoft.AspNetCore.Identity;
using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Repositories.Data
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task RoleSeedAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = new string[] { "User", "Technician", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        public static async Task AdminUserSeedAsync(UserManager<AppUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            if (_userManager.Users.Count() == 0)
            {
                var adminUser = new AppUser
                {
                    UserName = "admin1",
                    Email = "admin1234@gmail.com",
                    FullName = "Admin1",
                    DisplayName = "Admin1",
                    PhoneNumber = "01000000000",
                    Vefify= true,
                    Bio= "الحمد لله رب العالمين"

                };
                var result = await _userManager.CreateAsync(adminUser, "Admin@1234");

                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        public static async Task DepartmentSeedAsync(AppIdentityDbContext _dbContext)
        {
            if (_dbContext.Departments.Count() <= 0)
            {
                string[] departmentNames = new string[]
                {
                    "السباكة",
                    "الكهرباء",
                    "التكييف والتبريد",
                    "الدهانات",
                    "الغسالات",
                    "النجارة",
                    "صيانة الأجهزة المنزلية",
                    "الحدادة",
                    "الألوميتال",
                    "المصاعد"
                };

                var departments = departmentNames.Select(name => new Department
                {
                    Name = name,
                    Description = $"خدمات {name} العامة والصيانة الدورية.",
                    ImageUrl = ""

                }).ToList();

                await _dbContext.Departments.AddRangeAsync(departments);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
