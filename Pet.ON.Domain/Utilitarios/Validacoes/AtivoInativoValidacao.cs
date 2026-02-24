using Pet.ON.Domain.Utilitarios.Propriedades;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Pet.ON.Domain.Utilitarios.Validacoes
{
    public class AtivoInativoValidacao : ValidationAttribute
    {
        public AtivoInativoValidacao() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            if (AtivoInativo.ListaValores().Any(processo => processo == value.ToString())) return ValidationResult.Success;
            ErrorMessage = string.Format("Apenas os valores {0} são aceitos para o campo {1}.", AtivoInativo.ConcatenaValoresDescricoes(), validationContext.DisplayName);
            return new ValidationResult(ErrorMessage);
        }
    }
}
