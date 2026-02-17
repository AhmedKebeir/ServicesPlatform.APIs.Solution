
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServicesPlatform.APIs.Extensions;
using ServicesPlatform.APIs.Helpers;
using ServicesPlatform.APIs.Middlewares;
using ServicesPlatform.Core.Entities;
using ServicesPlatform.Repositories.Data;
using System.Threading.Tasks;

namespace ServicesPlatform.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region Services
            builder.Services.AddAutoMapper(M => M.AddProfile(typeof(MappingProfiles)));
            builder.Services.AddTransient<UserImageUrlResolver>();
            builder.Services.AddTransient<TechnicianUserImageUrlResolver>();
            builder.Services.AddTransient<UserImageUrlDtoResolver>();
            builder.Services.AddTransient<OrderImageUrlsResolver>();
            builder.Services.AddTransient<DepartmentImageUrlResolver>();
            builder.Services.AddTransient<ReviewImageUrlResolver>();
            #endregion

            #region IdentityUser
            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
       {
           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
       });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", options =>
                {
                    options.WithOrigins("http://localhost:3000", "https://salahli-app-front-3r7rxapvs-ahmedkebeirs-projects.vercel.app", "https://salahli.netlify.app")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            builder.Services.AddIdentityServices(builder.Configuration);
            #endregion

            var app = builder.Build();

            #region Update Datebase
            using var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider;
            var _dbContext = service.GetRequiredService<AppIdentityDbContext>();
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();

            try
            {
                await _dbContext.Database.MigrateAsync();
                var IdentityRole = service.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = service.GetRequiredService<UserManager<AppUser>>();

                await AppIdentityDbContextSeed.RoleSeedAsync(IdentityRole);
                await AppIdentityDbContextSeed.AdminUserSeedAsync(userManager, IdentityRole);
                await AppIdentityDbContextSeed.DepartmentSeedAsync(_dbContext);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during migration");
            }
            #endregion

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();




            app.UseStaticFiles();

            app.UseCors("MyPolicy");
            app.UseRouting();

            app.MapControllers();

            app.UseAuthorization();

            app.UseAuthorization();

            app.Run();
        }
    }
}
