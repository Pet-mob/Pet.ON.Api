using AutoMapper;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Agendamento;
using Pet.ON.Domain.Dtos.v1.Animal;
using Pet.ON.Domain.Dtos.v1.Empresa;
using Pet.ON.Domain.Dtos.v1.Parametros;
using Pet.ON.Domain.Dtos.v1.Usuario;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Entidade.v1.Parametros;

namespace Pet.ON.Api.Configuracoes
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AgendamentoCamposAuxiliares, AgendaDiaResDto>().ReverseMap();
            CreateMap<AgendamentoCamposAuxiliares, BuscarAgendamentoResDto>().ReverseMap();
            CreateMap<Agendamento, AdicionarAgendamentoReqDto>().ReverseMap();
            CreateMap<DashboardAgendamentos, DashboardAgendamentoResDto>().ReverseMap();

            #region Usuario
            CreateMap<Usuario, BuscarUsuarioResDto>().ReverseMap();
            CreateMap<Usuario, AdicionarUsuarioReqDto>().ReverseMap();
            CreateMap<Usuario, AdicionarUsuarioResDto>().ReverseMap();
            #endregion

            #region Empresa
            CreateMap<Empresa, AdicionarEmpresaReqDto>().ReverseMap();
            CreateMap<Empresa, AdicionarEmpresaResDto>().ReverseMap();
            CreateMap<Empresa, AtualizarEmpresaReqDto>().ReverseMap();
            CreateMap<Empresa, AtualizarEmpresaResDto>().ReverseMap();
            CreateMap<Empresa, BuscarEmpresaReqDto>().ReverseMap();
            CreateMap<Empresa, BuscarEmpresaResDto>().ReverseMap();
            CreateMap<HorariosFuncionamento, BuscarHorariosFuncionamentosEmpresaResDto>().ReverseMap();
            CreateMap<HorariosFuncionamento, BuscarHorariosFuncionamentosEmpresaReqDto>().ReverseMap();
            #endregion

            #region Serviço
            CreateMap<Servicos, BuscarServicosReqDto>().ReverseMap();
            CreateMap<Servicos, BuscarServicosResDto>().ReverseMap();
            #endregion

            #region Animal
            CreateMap<Animal, BuscarAnimalReqDto>().ReverseMap();
            CreateMap<Animal, BuscarAnimalResDto>().ReverseMap();
            CreateMap<Animal, AdicionarAnimalReqDto>().ReverseMap();
            CreateMap<Animal, AlterarAnimalReqDto>().ReverseMap();
            #endregion

            CreateMap<ParametroGeral, BuscarParametroResDto>().ReverseMap();
        }
    }
}
