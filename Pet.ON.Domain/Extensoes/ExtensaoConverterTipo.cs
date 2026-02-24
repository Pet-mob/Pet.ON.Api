using System;
using System.Linq;
using System.Text.Json;

namespace Pet.ON.Domain.Extensoes
{
    public static class ExtensaoConverterTipo
    {
        public static string ToText(this object value)
        {
            return value?.ToString() ?? string.Empty;
        }

        public static int ToInt(this object value)
        {
            try
            {
                return string.IsNullOrEmpty(value.ToText()) ? 0 : int.Parse(value.ToText());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static long ToLong(this object value)
        {
            try
            {
                return string.IsNullOrEmpty(value.ToText()) ? 0 : long.Parse(value.ToText());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static decimal ToDecimal(this object value)
        {
            try
            {
                return value != null ? decimal.Parse(value.ToText()) : 0.0M;
            }
            catch (Exception)
            {
                return 0.0M;
            }
        }

        public static DateTime ToDate(this object value)
        {
            try
            {
                return !string.IsNullOrEmpty(value?.ToText()) ? DateTime.Parse(value.ToText()) : DateTime.MinValue;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static string ToDateOrNull(this object value)
        {
            try
            {
                return !string.IsNullOrEmpty(value?.ToString())
                        ? DateTime.Parse(value.ToText()).ToString("dd/MM/yyyy HH:mm:ss")
                        : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static bool ToBool(this object value)
        {
            try
            {
                return value != null && bool.TryParse(value.ToString(), out var valueBool) ? valueBool : value.ToInt() == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static T ToEnum<T>(this string value)
        {
            var type = typeof(T);
            try
            {
                if (!type.IsEnum) throw new ArgumentException($"{type} não é um enum.");

                return (T)Enum.Parse(typeof(T), value);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Não foi possível converter {value} para o tipo: {type}");
            }
        }

        public static string ToJson(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return JsonSerializer.Serialize(obj);
        }

        public static type[] ToArray<type>(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return (type[])obj;
        }
        public static byte[] ToByteArray(string valor)
        {
            if (string.IsNullOrEmpty(valor))
            {
                return null;
            }
            var array = valor.Split(',');
            byte[] byteArray = Convert.FromBase64String(array[1]);

            return byteArray;
        }

        public static string[] ToUpperInArray(this string[] valor)
        {
            var retorno = new string[] { };

            if(valor == null)
                throw new ArgumentNullException(nameof(valor));

            for (int i = 0; i < valor.Length; i++)
            {
                retorno = retorno.Append(valor[i].ToUpper()).ToArray();
            }

            return retorno;
        }
    }
}
