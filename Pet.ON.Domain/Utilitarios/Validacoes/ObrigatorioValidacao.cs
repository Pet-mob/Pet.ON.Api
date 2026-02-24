using System.ComponentModel.DataAnnotations;

namespace Pet.ON.Domain.Utilitarios.Validacoes
{
    public class ObrigatorioValidacao : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            ErrorMessage = "O campo {0} é obrigatório.";
            return base.IsValid(value);
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = "O campo {0} é obrigatório.";
            return base.IsValid(value, validationContext);
        }
    }
}
