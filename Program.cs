using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using brevet_tracker.Server.Constants;
using brevet_tracker.Server.Models.Settings;

namespace brevet_tracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        var jwtSettings = context.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
                        if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
                        {
                            throw new InvalidOperationException("JwtSettings configuration is missing or invalid.");
                        }

                        services
                            .AddAuthentication(options =>
                            {
                                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                            })
                            .AddJwtBearer(options =>
                            {
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidIssuer = jwtSettings.Issuer,
                                    ValidAudience = jwtSettings.Audience,
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                                };
                            });

                        services.AddAuthorization(options =>
                        {
                            options.AddPolicy("AdminOnly", policy =>
                                policy.RequireRole(RoleNames.Admin));

                            options.AddPolicy("ParticipantOrAdmin", policy =>
                                policy.RequireRole(RoleNames.Participant, RoleNames.Admin));

                            options.AddPolicy("CanManageBrevets", policy =>
                                policy.RequireRole(RoleNames.Admin));
                        });
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
