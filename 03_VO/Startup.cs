using APIRest_ASPNET5.Business;
using APIRest_ASPNET5.Business.Implementations;
using APIRest_ASPNET5.Hypermedia.Enricher;
using APIRest_ASPNET5.Hypermedia.Filters;
using APIRest_ASPNET5.Models.Context;
using APIRest_ASPNET5.Repository;
using APIRest_ASPNET5.Repository.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using APIRest_ASPNET5.Filters;

namespace APIRest_ASPNET5
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            var connection = Configuration["MySQLConection:MySQLConectionString"];

            if (Environment.IsDevelopment())
            {
                MigrateDatabase(connection);
            }

            services.AddDbContext<MySQLContext>(options => options.UseMySql(connection));

            //Versioning API
            services.AddApiVersioning();

            //Swagger Support
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "APIRest_ASPNET5",
                        Version = "v1",
                        Description = "Just implementing API exemples",
                        Contact = new OpenApiContact
                        {
                            Name = "Yan Andrey",
                            Url = new Uri("https://github.com/yanandrey")
                        }
                    });
            });

            //Dependency Injection
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IClientBusiness, ClientBusinessImplementation>();
            services.AddScoped<IVehicleBusiness, VehicleBusinessImplementation>();
            services.AddScoped<IFileBusiness, FileBusinessImplementation>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();

            //HATEOAS Support
            var filterOptions = new HyperMediaFilterOptions();
            filterOptions.ContentResponseEnricherList.Add(new ClientEnricher());
            filterOptions.ContentResponseEnricherList.Add(new VehicleEnricher());
            services.AddSingleton(filterOptions);

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Just implementing API exemples - v1");
            });

            var option = new RewriteOptions();
            option.AddRedirect("^$","swagger");
            app.UseRewriter(option);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("DefaultApi","{controller=values}/{id?}");
            });
        }
        private void MigrateDatabase(string connection)
        {
            try
            {
                var evolveConnection = new MySql.Data.MySqlClient.MySqlConnection(connection);
                var evolve = new Evolve.Evolve(evolveConnection, msg => Log.Information(msg))
                {
                    Locations = new List<string> { "db/migrations", "db/dataset" },
                    IsEraseDisabled = true,
                };
                evolve.Migrate();
            }
            catch (Exception ex)
            {
                Log.Error("Something failed", ex);
                throw;
            }
        }
    }
}
