using System.ComponentModel.DataAnnotations;

namespace Pet.ON.Domain.Utilitarios.Validacoes
{
    public class TamanhoMaximoValidacao : MaxLengthAttribute
    {
        public TamanhoMaximoValidacao(int length):base(length)
        {
        }
        public override bool IsValid(object value)
        {
            ErrorMessage = "O campo {0} deve possuir no máximo {1} caractere(s).";
            return base.IsValid(value);
        }
    }
}