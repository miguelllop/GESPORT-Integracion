using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Interfaces;
using Infoport.GesportPCS.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using wcfPcsEscalas;

namespace Infoport.GesportPCS.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            Log.Information("Iniciando aplicación");
            Configuration = configuration;

            _config = Configuration.GetSection("Configuration").Get<Configuration.Configuration>();

        }

        public IConfiguration Configuration { get; }
        private IConfig _config;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<Configuration.Configuration>(Configuration.GetSection("Configuration"));

            ConfigureIoC(services);

            ConfigureHttpClients(services, _config);

            if (_config.MonitorFilesEnabled)
                services.AddHostedService<FileMonitorService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            ConfigureSwagger(services);
        }

        private void ConfigureIoC(IServiceCollection services)
        {
            services.AddSingleton<IBlockchainClient, BlockchainClient>();
            services.AddSingleton<IBlockchainService, BlockchainService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IMessageBroker, MessageBrokerService>();
            services.AddSingleton(c => new PublicWebServiceClient(PublicWebServiceClient.EndpointConfiguration.BasicHttpBinding_PublicWebService));
            services.AddSingleton<IPCSService, PCSService>();

        }

        private void ConfigureHttpClients(IServiceCollection services, IConfig config)
        {
            Log.Debug("Configuring http clients");

            services.AddHttpClient("health", c =>
                {
                    c.BaseAddress = new Uri(config.HealthEndpoint);
                }
            );
            
            services.AddHttpClient("blockchain", c =>
                {
                    c.BaseAddress = new Uri(config.BlockchainEndpoint);
                }
            );
            services.AddHttpClient("auth", c =>
                {
                    c.BaseAddress = new Uri(config.AuthEndpoint);
                }
            );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            UseSwagger(app);

            //app.UseHttpsRedirection();
            app.UseMvc();
        }

        #region Configuracion swagger

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Gesport PCS API", Version = "v1" });
                c.SchemaRegistryOptions.CustomTypeMappings.Add(typeof(IFormFile), () => new Schema() { Type = "file", Format = "binary" });
                c.AddSecurityDefinition("Bearer",
                new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                { "Bearer", Enumerable.Empty<string>() },
                });
            });
        }

        private void UseSwagger(IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gesport PCS API");
            });
        }

        #endregion
    }
}
