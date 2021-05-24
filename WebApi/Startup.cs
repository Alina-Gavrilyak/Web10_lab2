using AutoMapper;
using Contracts.Repositories;
using DataAccessServices;
using DataAccessServices.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Services.Mapper;
using Services.Repositories;
using Services.Repositories.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web10_Lab2
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
            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new ProductMappingProfile());
                mc.AddProfile(new RequestDeliveryMappingProfile());
                mc.AddProfile(new ShopMappingProfile());
                mc.AddProfile(new WarehouseMappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            //Temp
            services.AddSingleton<TurnoverRepository>();

            services.AddOptions<RepositoryOptions>().Configure(opt => opt.ConnectionString = Configuration.GetConnectionString("DefaultConnection"));
            services.AddSingleton<IProductRepository, ProductRepository>();

            services.AddScoped<ProductService>();
            services.AddScoped<WarehouseService>();
            services.AddScoped<ShopService>();
            services.AddScoped<RequestDeliveryService>();
            services.AddScoped<ProductShopService>();
            services.AddScoped<ProductWarehouseService>();
            services.AddScoped<DeliveryItemService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web10_Lab2", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web10_Lab2 v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
