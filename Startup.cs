using System.Net;
using System.Text;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Web;
using System;

namespace DatingApp.API
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
            // services.AddDbContext<DataContext>(x => x.UseMySql
            // (Configuration.GetConnectionString("DefaultConnection")));

            //var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");

                   services.AddDbContext<DataContext>(x => x.UseSqlite
             (Configuration.GetConnectionString("DefaultConnection")));
 
    // services.AddDbContext<DataContext>(options =>
    //     options.UseSqlServer(
    //         sqlConnectionString
    //     )
    // );
    // var ConnectionString = "Data Source=" + HttpContext.Current.Server.MapPath(@"\DatingApp.db");
    var ConnectionString = "Data Source=" + AppDomain.CurrentDomain.GetData("DataDirectory") + "DatingApp.db";
    services.AddDbContext<DataContext>(x => x.UseSqlite(ConnectionString));

            services.AddControllers();
          services.AddMvc().AddNewtonsoftJson(options=> options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddCors();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper(typeof(DatingRepository).Assembly);
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding
                    .ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddScoped<LogUserActivity>();
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

                app.UseExceptionHandler(builder => {
                    builder.Run(async context =>{
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if(error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                app.UseHsts();

            }
            app.UseHttpsRedirection();

            app.UseCors(x => x.WithOrigins("http://localhost:4200").AllowCredentials().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            // app.UseMvc(routes => {
            //     routes.MapSpaFallbackRoute(
            //         name: "spa-fallback",
            //         defaults: new { controller = "Fallback", action = "Index"}
            //     );
            // });
            app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Fallback}/{action=Index}/{id?}");

        endpoints.MapFallbackToController("Index", "Fallback");
    });

            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapControllers();
            // });
        }
    }
}
