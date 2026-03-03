using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pet.ON.Api.Configuracoes;
using Pet.ON.Infra.Repositorio;
using AutoMapper;
using Pet.ON.Domain.Interfaces.Repositorio;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using System;
using Pet.ON.Api.Hubs;

namespace Pet.ON.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureCors(services);
            ConfigureDatabase(services);
            ConfigureSwagger(services);
            ConfigureAutoMapper(services);
            RegistradorInstanciaConfiguracao.Registrar(services);
            // ✅ Registra o cache de memória para que o AuthController funcione corretamente
            services.AddMemoryCache();
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddResponseCompression();

            // ✅ Adiciona o SignalR
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Service Pet Shop");
            });

            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

                // ✅ Mapeia o hub de agendamento
                endpoints.MapHub<AgendamentoHub>("/hubs/agendamentos");
            });
        }

        private void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            // Configuração de conexão com o banco usando Dapper
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
                        ?? Configuration.GetConnectionString("DefaultConnection");

            // Registrar a conexão como singleton, ou em uma fábrica para escopo de cada requisição
            services.AddScoped<IDbConnection>(sp =>
                new SqlConnection(connectionString));
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service Pet Shop - API", Version = "v1" });
            });
        }

        private void ConfigureAutoMapper(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
