using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pet.ON.Api.Notificacoes;
using Pet.ON.Domain.Interfaces.Hubs;
using Pet.ON.Domain.Interfaces.Repositorio;
using Pet.ON.Domain.Interfaces.Repositorio.Parametros;
using Pet.ON.Domain.Interfaces.Servico.v1;
using Pet.ON.Domain.Interfaces.Servico.v1.Parametros;
using Pet.ON.Infra.Repositorio;
using Pet.ON.Infra.Repositorio.Parametros;
using Pet.ON.Service.Servico;
using Pet.ON.Service.Servico.Parametros;

namespace Pet.ON.Api.Configuracoes
{
    public static class RegistrarInstanciaConfiguracao
    {
        public static void Registrar(this IServiceCollection services)
        {

            
            #region Serviço
            services.AddScoped<IParametrosServico, ParametrosServico>();
            services.AddScoped<IAgendamentoServico, AgendamentoServico>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IAnimalServico, AnimalServico>(provider =>
            {
                var animalRepositorio = provider.GetRequiredService<IAnimalRepositorio>();
                var mapper = provider.GetRequiredService<IMapper>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connection = configuration.GetConnectionString("AzureBlobConnection"); // certifique-se de que está igual ao appsettings.json
                var storageService = provider.GetRequiredService<IStorageService>();

                return new AnimalServico(animalRepositorio, mapper, configuration, storageService);
            });

            services.AddScoped<IServicosServico, ServicosServico>();
            services.AddScoped<IUsuarioServico, UsuarioServico>(provider =>
            {
                var mapper = provider.GetRequiredService<IMapper>();
                var empresaServico = provider.GetRequiredService<IEmpresaServico>();
                var usuarioRepositorio = provider.GetRequiredService<IUsuarioRepositorio>();
                var storageService = provider.GetRequiredService<IStorageService>();

                return new UsuarioServico(mapper, empresaServico, usuarioRepositorio, storageService);
            });

            services.AddScoped<IEmpresaServico, EmpresaServico>(provider =>
            {
                var empresaRepositorio = provider.GetRequiredService<IEmpresaRepositorio>();
                var mapper = provider.GetRequiredService<IMapper>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var storageService = provider.GetRequiredService<IStorageService>();
                return new EmpresaServico(empresaRepositorio, mapper, configuration, storageService);
            });
            #endregion

            #region Serviço Validações
            #endregion

            #region Repositório
            services.AddScoped<IAgendamentoRepositorio, AgendamentoRepositorio>();
            services.AddScoped<IAnimalRepositorio, AnimalRepositorio>();
            services.AddScoped<IServicosRepositorio, ServicosRepositorio>();
            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            services.AddScoped<IEmpresaRepositorio, EmpresaRepositorio>();
            services.AddScoped<IParametrosRepositorio, ParametrosRepositorio>();
            #endregion

            #region Hubs
            services.AddScoped<INotificacaoDeAgendamento, NotificacaoDeAgendamentoSignalR>();
            #endregion
        }
    }
}
