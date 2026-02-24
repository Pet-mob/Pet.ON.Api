using System.Collections.Generic;
using System.Linq;

namespace Pet.ON.Domain.Utilitarios.Propriedades
{
    public class AtivoInativo
    {
        public const string ATIVO = "A";
        public const string INATIVO = "I";

        public static string BuscaDescricao(string ativoInativo)
        {
            var descricao = new Dictionary<string, string>
            {
                { ATIVO, "Ativo" },
                { INATIVO, "Inativo" }
            };

            if (!descricao.TryGetValue(ativoInativo, out string retornoDescricao)) retornoDescricao = string.Empty;

            return retornoDescricao;
        }

        public static ICollection<string> ListaValores() =>
            new List<string>() { ATIVO, INATIVO };

        public static string ConcatenaValoresDescricoes() =>
            string.Join(" / ", ListaValores().Select(ativoInativo => $"{ativoInativo} - {BuscaDescricao(ativoInativo)}"));
    }
}

