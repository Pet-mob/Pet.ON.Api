using System;
using System.Collections.Generic;
using System.Linq;

namespace Pet.ON.Domain.Utilitarios.Validacoes
{
    public class Validador<T>
    {
        private readonly List<string> _notificacoes = new List<string>();

        public bool EstaValido(T objeto)
        {
            _notificacoes.Clear(); // Limpar notificações antes de validar

            if (objeto == null)
            {
                _notificacoes.Add("Objeto não pode ser nulo.");
                return false;
            }

            // Exemplo de validação básica: verificar propriedades obrigatórias
            var propriedades = typeof(T).GetProperties();
            foreach (var prop in propriedades)
            {
                var valor = prop.GetValue(objeto);
                if (valor == null || (valor is string str && string.IsNullOrEmpty(str)))
                {
                    _notificacoes.Add($"A propriedade '{prop.Name}' não pode ser nula ou vazia.");
                }
            }

            return !_notificacoes.Any();
        }

        public IEnumerable<string> ObterNotificacoes()
        {
            return _notificacoes;
        }
    }

}
