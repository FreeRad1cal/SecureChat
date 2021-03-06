﻿using System;
using Auth.Data;
using Auth.Models;
using Auth.Services;
using HealthChecks.UI.Client;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Environment = environment;
            Configuration = configuration;
        }

        IConfiguration Configuration { get; }
        IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).Assembly.FullName;

            services.Configure<AuthSettings>(Configuration);

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseMySql(Configuration["AccountDbConnectionString"],
                    opt =>
                    {
                        opt.MigrationsAssembly(migrationsAssembly);
                        opt.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: Array.Empty<int>());
                    });
            });

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddSignInManager()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
            {
                // Require authentication by default
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddHealthChecks()
                .AddCheck("self-check", () => HealthCheckResult.Healthy())
                .AddDbContextCheck<UsersDbContext>(name: "users-db-check")
                .AddDbContextCheck<ConfigurationDbContext>(name: "configuration-db-check");

            // configure identity server with in-memory stores, keys, clients and resources
            services.AddIdentityServer(options =>
                {
                    options.PublicOrigin = Configuration["PublicOriginUrl"];
                    if (!Environment.IsDevelopment())
                    {
                        options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
                    }
                })
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseMySql(Configuration["AuthDbConnectionString"],
                        opt =>
                        {
                            opt.MigrationsAssembly(migrationsAssembly);
                            opt.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: Array.Empty<int>());
                        });
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseMySql(Configuration["AuthDbConnectionString"],
                        opt =>
                        {
                            opt.MigrationsAssembly(migrationsAssembly);
                            opt.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: Array.Empty<int>());
                        });
                })
                .AddAspNetIdentity<User>();

            services.AddScoped<ILoginService<User>, IdentityLoginService>();
            services.AddScoped<IRedirectUrlGenerator, RedirectUrlGenerator>();
            services.AddScoped<ConfigurationDbContextSeed>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}
