using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ServicesPlatform.Core;
using ServicesPlatform.Core.Entities;
using ServicesPlatform.Core.Services.Contract;
using ServicesPlatform.Repositories;
using ServicesPlatform.Repositories.Data;
using ServicesPlatform.Services;
using System.Text;

namespace ServicesPlatform.APIs.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,IConfiguration configuration)
        {

            services.AddScoped(typeof(IAuthService), typeof(AuthService));
            services.AddSingleton(typeof(IVerificationCode), typeof(VerificationCode));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IOrderService), typeof(OrderService));
            

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

            })
                .AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:ValidAudience"],
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"])),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromDays(double.Parse(configuration["JWT:DurationInDayes"])),
                    };

                    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // عشان الـ SignalR يعرف التوكن من query string (WebSockets ما بتدعم headers بسهولة)
                            var accessToken = context.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },// 🔐 أهم جزء
                        OnTokenValidated = async context =>
                        {
                            var userManager = context.HttpContext.RequestServices
                                .GetRequiredService<UserManager<AppUser>>();

                            var user = await userManager.GetUserAsync(context.Principal);
                            if (user == null)
                            {
                                context.Fail("Unauthorized");
                                return;
                            }

                            var token = context.Request.Headers["Authorization"]
                                .ToString()
                                .Replace("Bearer ", "");

                            var storedToken = await userManager.GetAuthenticationTokenAsync(
                                user,
                                "ServicesPlatform",
                                "Token"
                            );

                            if (storedToken != token)
                            {
                                context.Fail("Token revoked");
                            }
                        }


                    };
                });

            return services;
        } 
    }
}
