using System.Text;
using API_ElevWiki.Models;
using API_ElevWiki.Interfaces;
using API_ElevWiki.Repository;
using API_ElevWiki.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace API_ElevWiki
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds the Entity Framework context to communicate with the database.
            services.AddDbContextPool<EFContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")));

            // Reads the Kestrel configuration settings from the specified path.
            services.Configure<KestrelServerOptions>(Configuration.GetSection("Kestrel"));

            #region Email services
            // Reads the Email configuration settings from the specified path.
            AppSettingsEmail emailConfig = Configuration.GetSection("AppSettingsEmail").Get<AppSettingsEmail>();

            // Singleton objects are the same for every object and every request
            services.AddSingleton(emailConfig);

            // Scoped objects are the same within a request, but different across different requests
            services.AddScoped<IEmailService, EmailService>();
            #endregion

            #region JWT services
            // Reads the key from the given path
            IConfigurationSection getKey = Configuration.GetSection("AppSettingsJWT");

            // Adds the key to services
            services.Configure<AppSettingsJWT>(getKey);

            // JWT Authentication
            AppSettingsJWT settings = getKey.Get<AppSettingsJWT>();
            byte[] keyHolder = Encoding.UTF8.GetBytes(settings.key);
            char[] key = Encoding.UTF8.GetString(keyHolder).ToCharArray();

            // Adds the authentication service, and enables configuration of the JSON web tokens
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://172.18.3.153:46465",
                    ValidAudience = "http://172.18.3.153:46465",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

            // Transient objects are always different; a new instance is provided to every controller and service.
            services.AddTransient<ITokenService, TokenService>();
            #endregion

            #region Cors services
            // CORS is used to filter allowed networks, since there is role authorization on requests, any network is allowed.
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            #endregion

            #region Mvc services
            // Sets the version of MVC service that gets applied at runtime. 
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // Disables endpoint routing from MVC services, uses legacy routing instead.
            services.AddMvc(options => options.EnableEndpointRouting = false);

            // Applies MVC services
            services.AddMvc();
            #endregion

            services.AddScoped<IDbInfoService, DbInfoService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            // Enables CORS
            app.UseCors("AllowAll");

            // Enables JSON web token authentication
            app.UseAuthentication();

            // Enables legacy routing
            app.UseRouting();

            // Enables MVC routing template
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
