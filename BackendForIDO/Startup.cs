using BackendForIDO.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Add this using directive
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens; // Add this using directive
using System.Text; // Add this using directive
using BackendForIDO.Repositories;
using BackendForIDO.Services;

namespace BackendForIDO
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Get the database connection string from appsettings.json
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Get the authentication settings from appsettings.json
            string secretKey = _configuration["Authentication:SecretKey"];
            string issuer = _configuration["Authentication:Issuer"];
            string audience = _configuration["Authentication:Audience"];

            // Register the DbContext and specify the database provider (e.g., SQL Server)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register needed Repository and AuthenticationService
            services.AddScoped<UserRepository>();
            services.AddScoped<AuthenticationService>();
            services.AddScoped<TaskRepository>();

            // Register the authentication settings as services
            services.AddSingleton(secretKey);
            services.AddSingleton(issuer);
            services.AddSingleton(audience);

            // Add authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
                    };
                });

            // Add controllers and related services
            services.AddControllers();
            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    // Allow requests from http://localhost:4200
                    builder.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication(); // Add this line to enable authentication
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
