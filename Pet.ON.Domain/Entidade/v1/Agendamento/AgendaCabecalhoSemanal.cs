namespace Pet.ON.Domain.Entidade.v1
{
    public class AgendaCabecalhoSemanal
    {
        public string DiaSemana { get; set; } = null!;
        public string DataFormatada { get; set; } = null!;
        // Se quiser, pode incluir IdEmpresa, Data etc, mas para cabeçalho só isso já ajuda

    }
}
