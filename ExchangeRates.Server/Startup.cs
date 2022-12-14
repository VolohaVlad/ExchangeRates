using ExchangeRates.Core.Interfaces;
using ExchangeRates.Infrastructure;
using ExchangeRates.Infrastructure.Options;
using ExchangeRates.Infrastructure.Services;
using ExchangeRates.Server.Filters;
using ExchangeRates.Server.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;

namespace ExchangeRates.Server
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
            services.Configure<NbRBOptions>(Configuration.GetSection(NbRBOptions.SectionName));
            services.Configure<BitcoinOptions>(Configuration.GetSection(BitcoinOptions.SectionName));
            services.Configure<JsonRepoOptions>(Configuration.GetSection(JsonRepoOptions.SectionName));
            services.AddScoped<INbRBService, NbRBService>();
            services.AddScoped<IBitcoinService, BitcoinService>();
            services.AddSingleton<IRateRepository, JsonRepository>();
            services.AddScoped<IRateCacheService, RateCacheService>();
            services.AddHostedService<CacheLoader>();
            services.AddMemoryCache();
            services.AddMvc(SetMvcServiceOptions);

            services.AddControllers();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddHttpClient();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //options.Filters.Add(new ApiExceptionFilter(logger));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void SetMvcServiceOptions(MvcOptions options)
        {
            options.Filters.Add(new ApiExceptionFilter(LogManager.GetCurrentClassLogger()));
        }
    }
}
