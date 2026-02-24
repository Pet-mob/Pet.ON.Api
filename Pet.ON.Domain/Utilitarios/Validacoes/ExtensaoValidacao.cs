using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pet.ON.Domain.Utilitarios.Validacoes
{
    public class ExtensaoValidacao : RangeAttribute
    {
        public ExtensaoValidacao(double minimum, double maximum) : base(minimum, maximum)
        {
        }

        public ExtensaoValidacao(Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
        }
        public override bool IsValid(object value)
        {
            ErrorMessage = "O campo {0} deve ser um valor entre {1} e {2}.";
            return base.IsValid(value);
        }
    }
}
