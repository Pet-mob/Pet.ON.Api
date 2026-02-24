using Moq;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using Pet.ON.Service.Servico;

namespace Pet.ON.Teste.CasoDeTeste
{
    public class AgendamentoTeste
    {

        //#region Deve Incluir Com Sucesso

        //[Fact]
        //public async void DeveIncluirComSucesso()
        //{

        //    AdicionarFamiliaExcecaoReqDto dtoReq = new AdicionarFamiliaExcecaoReqDto()
        //    {
        //        IdFamilia = 1580
        //    };

        //    var familiaExcecaoServico = new FamiliaExcecaoServico(
        //                                        _controleNotificacaoMock.Object,
        //                                        _familiaExcecaoRepositorio.Object,
        //                                        _familiaExcecaoValidacaoServico.Object
        //                                        );

        //    await familiaExcecaoServico.Cadastrar(dtoReq);

        //    _familiaExcecaoRepositorio.Verify(f => f.InsertAsync(It.Is<FamiliaExcecao>(arg => arg.IdFamilia == dtoReq.IdFamilia)), Times.Once());
        //}
        //#endregion

        //#region Deve Excluir Com Sucesso

        //[Fact]
        //public async void DeveExcluirComSucesso()
        //{

        //    int idFamilia = 1580;

        //    var familiaExcecaoServico = new FamiliaExcecaoServico(
        //                                        _controleNotificacaoMock.Object,
        //                                        _familiaExcecaoRepositorio.Object,
        //                                        _familiaExcecaoValidacaoServico.Object
        //                                        );

        //    await familiaExcecaoServico.Excluir(idFamilia);

        //    _familiaExcecaoRepositorio.Verify(f => f.DeleteAsync(It.Is<FamiliaExcecao>(arg => arg.IdFamilia == idFamilia)), Times.Once());
        //}
        //#endregion

        //#region Deve Retornar Lista Familia Excecao
        //[Fact]
        //public async void DeveRetornarListaFamiliaExcecao()
        //{
        //    var familiaExcecao = new FamiliaExcecao(1580, DateTime.Now);

        //    _familiaExcecaoRepositorio.Setup(s => s.GetAll()).Returns(() => new[] { familiaExcecao }.AsQueryable());


        //    var familiaExcecaoServico = new FamiliaExcecaoServico(
        //                                        _controleNotificacaoMock.Object,
        //                                        _familiaExcecaoRepositorio.Object,
        //                                        _familiaExcecaoValidacaoServico.Object
        //                                        );

        //    await familiaExcecaoServico.Obter();

        //    _familiaExcecaoRepositorio.Verify(r => r.GetAll(), Times.Once);
        //}
        //#endregion

        //[Fact]
        //public async Task RetornarListaAgendamento()
        //{
        //    // Arrange
        //    var mockRepositorio = new Mock<IAgendamentoRepositorio>();
        //    var mockMapper = new Mock<IMapper>();

        //    var dto = new BuscarAgendamentoReqDto();
        //    var listaAgendamentos = new List<BuscarAgendamentoResDto>
        //    {
        //        new BuscarAgendamentoResDto { IdAgendamento = 1, IdUsuario = 2, IdAnimal = 3 },
        //        new BuscarAgendamentoResDto { IdAgendamento = 2, IdUsuario = 5, IdAnimal = 8 }
        //    };

        //    // Configurando o mock do repositório para retornar a lista esperada
        //    mockRepositorio.Setup(x => x.GetByParameters(dto)).ReturnsAsync(listaAgendamentos);

        //    // Criando a instância real do serviço, injetando o mock
        //    var servico = new AgendamentoServico(mockRepositorio.Object, mockMapper.Object);

        //    // Act
        //    var resultado = await servico.Buscar(dto);

        //    // Assert
        //    Assert.NotNull(resultado);
        //    Assert.Equal(listaAgendamentos.Count, resultado.Count());
        //    Assert.Equal(listaAgendamentos.First().IdAgendamento, resultado.First().IdAgendamento);
        //}
    }

}