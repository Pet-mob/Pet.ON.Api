using System;

namespace Pet.ON.Domain.Utilitarios
{
    public static class UtilitarioVerificacao
    {
        /// <summary>
        /// Verifica se a matriz é nula ou se o comprimento é igual a zero
        /// </summary>
        /// <param name="destino">Matriz de inteiro</param>
        /// <returns>Valor Booleano</returns>
        public static bool ENuloOuVazioMatrizInteiro(int[] destino) => destino == null || destino.Length == 0;

        /// <summary>
        /// Verifica se a matriz é nula ou se o comprimento é igual a zero
        /// </summary>
        /// <param name="destino">Matriz de long</param>
        /// <returns>Valor Booleano</returns>
        public static bool ENuloOuVazioMatrizLong(long[] destino) => destino == null || destino.Length == 0;

        /// <summary>
        /// Verifica se a matriz é nula ou se o comprimento é igual a zero
        /// </summary>
        /// <param name="destino">Matriz de inteiro</param>
        /// <returns>Valor Booleano</returns>
        public static bool ENuloOuVazioMatrizInteiroNulo(int?[] destino) => destino == null || destino.Length == 0;

        ///// <summary>
        ///// Verifica se a matriz é nula ou se o comprimento é igual a zero
        ///// </summary>
        ///// <param name="destino">Matriz de string</param>
        ///// <returns>Valor Booleano</returns>
        //public static bool ENuloOuVazioMatrizString(string[] destino) => destino.IsNullOrEmpty();

        /// <summary>
        /// A data contendo valor é verificado se a mesma possui hora/minuto/segundo, caso não haja é inserido de acordo com os valores informados
        /// </summary>
        /// <param name="data"></param>
        /// <param name="horas"></param>
        /// <param name="minutos"></param>
        /// <param name="segundos"></param>
        /// <returns></returns>
        public static DateTime? RetornaDataComHora(
            DateTime? data,
            int horas = 0,
            int minutos = 0,
            int segundos = 0,
            int milisegundos = 0)
        {
            if (data.HasValue && data.Value.TimeOfDay.TotalSeconds == 0)
            {
                data = data.Value.Add(new TimeSpan(horas, minutos, segundos));
            }
            else return data;
            return data;
        }

        /// <summary>
        /// Converte string para o tipo 'int?'
        /// </summary>
        /// <param name="destino"></param>
        /// <returns></returns>
        public static int? ConverteParaNumeroInteiroOuNull(string destino)
        {
            if (int.TryParse(destino, out int n))
            {
                return n;
            }

            return null;
        }

        /// <summary>
        /// Converte objeto para string. Se o objeto for null, converte para string vazia 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ConverteParaStringMesmoSeNull(this object obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }

    }
}
