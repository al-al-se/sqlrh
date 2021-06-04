using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace sqlrh_server
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
            string connection = Configuration.GetConnectionString("DefaultConnection");

            services.Configure<PasswordHasherOptions>(Configuration.GetSection(
                                        PasswordHasherOptions.SectionName));

            services.AddScoped<IPasswordHasher<SqlrhUser>,MyPasswordHasher>();

            services.AddDbContext<IUserRepository,UserContext>(options => options.UseSqlite(connection));
  
            services.AddDbContext<IReportRepository,ReportContext>(options => options.UseSqlite(connection));
           
            services.AddDbContext<IExternalDataBaseRepository,ExternalDataBaseContext>(options => options.UseSqlite(connection));
          
            services.Configure<ReportBuilderOptions>(Configuration.GetSection(
                                        ReportBuilderOptions.SectionName));
                                        
            services.AddScoped<IReportBuilderService,ReportBuilderService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/User/Login");
                });
                   
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "sqlrh_server", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "sqlrh_server v1"));
            }

            app.UseHttpsRedirection();

            app.UseRequestResponseLogging();

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
