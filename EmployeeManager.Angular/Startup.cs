using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManager.Angular.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManager.Angular
{
    public class Startup
    {
        public readonly IConfiguration Config =null;
       

        // This method gets called by the runtime. Use this method to add services to the container.

        public Startup(IConfiguration configuration)
        {
            this.Config = configuration;

        }
        
        
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            services.AddControllersWithViews();
           /// services.AddEntityFrameworkSqlServer();
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(this.Config.GetConnectionString("AppDb")));

            //  services.AddDbContext<AppDbContext>(options => options.UseSqlServer("data source =.; initial catalog = Northwind; integrated security = true"));


            //services.AddCors(options => {
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials());
            //});



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           ClockSkew = TimeSpan.Zero,
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidateLifetime = true,
                           ValidateIssuerSigningKey = true,
                           ValidIssuer = Config["Jwt:Issuer"],
                           ValidAudience = Config["Jwt:Audience"],
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Jwt:Key"]))
                       };
                   });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

             // app.UseCors(builder => builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());

            app.UseCors(builder => builder
                   .AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader());



          //  app.UseCorsMiddleware();
           
           

            app.UseDefaultFiles();
            app.UseStaticFiles();


            app.UseRouting();
            //app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
            endpoints.MapControllers();

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{contoller}/{action}/{id?}");

                endpoints.MapControllerRoute(
                    name: "spa",
                    pattern: "{*url}",
                    defaults: new { Controllers = "Spa", action = "Index" });

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
