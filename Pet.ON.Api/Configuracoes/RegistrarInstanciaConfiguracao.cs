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
using System;

namespace Pet.ON.Api.Configuracoes
{
    /// <summary>
    /// Classe para registrar todas as dependências (injeção de dependência) da aplicação.
    /// Segue o padrão de Dependency Injection do ASP.NET Core.
    /// </summary>
    public static class RegistradorInstanciaConfiguracao
    {
        /// <summary>
        /// Registra todos os serviços, repositórios e hubs no container de DI.
        /// </summary>
        public static void Registrar(this IServiceCollection servicos)
        {
            if (servicos == null)
                throw new ArgumentNullException(nameof(servicos));

            // Registrar serviços
            RegistrarServicos(servicos);

            // Registrar repositórios
            RegistrarRepositorios(servicos);

            // Registrar hubs de notificação
            RegistrarHubs(servicos);
        }

        /// <summary>
        /// Registra todos os serviços da camada de negócio.
        /// </summary>
        private static void RegistrarServicos(IServiceCollection servicos)
        {
            // Serviços base
            servicos.AddScoped<IParametrosServico, ParametrosServico>();
            servicos.AddScoped<IAgendamentoServico, AgendamentoServico>();
            servicos.AddScoped<IStorageService, StorageService>();

            // Serviço de Animal com injeção de dependências
            servicos.AddScoped<IAnimalServico, AnimalServico>(provedor =>
            {
                var repositorio = provedor.GetRequiredService<IAnimalRepositorio>();
                var mapper = provedor.GetRequiredService<IMapper>();
                var configuracao = provedor.GetRequiredService<IConfiguration>();
                var servicoArmazenamento = provedor.GetRequiredService<IStorageService>();

                return new AnimalServico(repositorio, mapper, configuracao, servicoArmazenamento);
            });

            // Serviço de Serviços
            servicos.AddScoped<IServicosServico, ServicosServico>();

            // Serviço de Usuário com injeção de dependências
            servicos.AddScoped<IUsuarioServico, UsuarioServico>(provedor =>
            {
                var mapper = provedor.GetRequiredService<IMapper>();
                var servicoEmpresa = provedor.GetRequiredService<IEmpresaServico>();
                var repositorio = provedor.GetRequiredService<IUsuarioRepositorio>();
                var servicoArmazenamento = provedor.GetRequiredService<IStorageService>();

                return new UsuarioServico(mapper, servicoEmpresa, repositorio, servicoArmazenamento);
            });

            // Serviço de Empresa com injeção de dependências
            servicos.AddScoped<IEmpresaServico, EmpresaServico>(provedor =>
            {
                var repositorio = provedor.GetRequiredService<IEmpresaRepositorio>();
                var mapper = provedor.GetRequiredService<IMapper>();
                var servicoArmazenamento = provedor.GetRequiredService<IStorageService>();

                return new EmpresaServico(repositorio, mapper, servicoArmazenamento);
            });
        }

        /// <summary>
        /// Registra todos os repositórios.
        /// </summary>
        private static void RegistrarRepositorios(IServiceCollection servicos)
        {
            servicos.AddScoped<IAgendamentoRepositorio, AgendamentoRepositorio>();
            servicos.AddScoped<IAnimalRepositorio, AnimalRepositorio>();
            servicos.AddScoped<IServicosRepositorio, ServicosRepositorio>();
            servicos.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            servicos.AddScoped<IEmpresaRepositorio, EmpresaRepositorio>();
            servicos.AddScoped<IParametrosRepositorio, ParametrosRepositorio>();
        }

        /// <summary>
        /// Registra todos os hubs de notificação em tempo real.
        /// </summary>
        private static void RegistrarHubs(IServiceCollection servicos)
        {
            servicos.AddScoped<INotificacaoDeAgendamento, NotificacaoDeAgendamentoSignalR>();
        }
    }
}
