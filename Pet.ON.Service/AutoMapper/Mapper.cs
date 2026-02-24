using AutoMapper;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Usuario;
using Pet.ON.Domain.Entidade.v1;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pet.ON.Service.AutoMapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Usuario, AdicionarUsuarioReqDto>().ReverseMap();
            CreateMap<Agendamento, BuscarAgendamentoResDto>();
            CreateMap<BuscarAgendamentoResDto, Agendamento>();


            //Unable to cast object of
            //type 'System.Collections.Generic.List`1[Pet.ON.Domain.Dtos.v1.BuscarAgendamentoResDto]'
            //to type 'System.Linq.IQueryable`1[Pet.ON.Domain.Dtos.v1.BuscarAgendamentoResDto]'.

            //CreateMap<ConfiguracaoImportacaoDiv, AtualizarConfiguracaoDivisaoReqDto>().ReverseMap();
            //CreateMap<ConfiguracaoImportacaoDiv, AtualizarConfiguracaoDivisaoRespDto>().ReverseMap();

            //CreateMap<ConfiguracaoImportacaoDiv, BuscarPorSeqConfiguracaoDivisaoRespDto>().ReverseMap();

            //CreateMap<ConfiguracaoImportacaoDiv, BuscarConfigImpErpDivRespDto>()
            //    .ForMember(m => m.Divisao, opt => opt.MapFrom(s => s.Divisao == null ? "" : s.Divisao.Descricao))
            //    .ForMember(m => m.Comprador, opt => opt.MapFrom(s => s.Comprador == null ? "" : s.Comprador.Apelido))
            //    .ForMember(m => m.Tributacao, opt => opt.MapFrom(s => s.Tributacao == null ? "" : s.Tributacao.Descricao))
            //    .ForMember(m => m.FormaAbastecimento, opt => opt.MapFrom(s => s.FormaAbastec == null ? "" : s.FormaAbastec.Descricao));

            //CreateMap<Divisao, BuscarDivisaoRespDto>().ReverseMap();
            //CreateMap<Tributacao, BuscarTributacaoRespDto>().ReverseMap();
            //CreateMap<Comprador, BuscarCompradorRespDto>().ReverseMap();
            //CreateMap<FormaAbastecimento, BuscarFormaAbastecimentoRespDto>().ReverseMap();
            //CreateMap<ListaVenda, BuscarListaVendaRespDto>().ReverseMap();
        }

    }
}
