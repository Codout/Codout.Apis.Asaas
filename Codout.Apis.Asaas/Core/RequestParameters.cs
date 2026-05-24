using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Codout.Apis.Asaas.Core.Extension;
using Codout.Apis.Asaas.Core.Utils;

namespace Codout.Apis.Asaas.Core
{
    public class RequestParameters : Dictionary<string, string>
    {
        public void Add(string key, List<string> valueList)
        {
            foreach (var value in valueList)
            {
                Add(key, value);
            }
        }

        public void AddRange(RequestParameters map)
        {
            map.Keys.ToList().ForEach(key => Add(key, map[key]));
        }

        public new void Add(string key, string value)
        {
            if (value != null)
            {
                this[key] = value;
                return;
            }

            Remove(key);
        }

        public void Add(string key, DateTime? value)
        {
            if (value.HasValue)
            {
                Add(key, value.Value.ToApiRequest());
                return;
            }

            Remove(key);
        }

        public void Add(string key, Enum @enum)
        {
            if (@enum != null)
            {
                Add(key, @enum.ToString());
                return;
            }

            Remove(key);
        }

        public void Add(string key, bool? value)
        {
            if (value != null)
            {
                // bool.ToString() retorna "True"/"False" (PascalCase), mas a API Asaas
                // espera "true"/"false" lowercase (padrao JSON/HTTP). Sem o ToLower
                // o filtro e silenciosamente ignorado pela API.
                Add(key, value.Value ? "true" : "false");
                return;
            }

            Remove(key);
        }

        public void Add(string key, decimal? value)
        {
            if (value != null)
            {
                // decimal.ToString() usa cultura corrente: em pt-BR vira "12,5"
                // (virgula) ao inves de "12.5" (ponto). API Asaas (JSON) exige ponto.
                Add(key, value.Value.ToString(CultureInfo.InvariantCulture));
                return;
            }

            Remove(key);
        }

        public new string this[string key]
        {
            get
            {
                if (TryGetValue(key, out string value))
                {
                    return value;
                }

                return null;
            }
            set
            {
                if (ContainsKey(key))
                {
                    Remove(key);
                }

                base.Add(key, value);
            }
        }

        public T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(this[key]))
            {
                return default;
            }

            if (typeof(T).IsEnum || typeof(T).IsNullableEnum())
            {
                return EnumUtils.Parse<T>(this[key].ToString());
            }

            if (typeof(T) == typeof(DateTime?) || typeof(T) == typeof(DateTime))
            {
                return DateTimeUtils.Parse(this[key].ToString()).Cast<T>();
            }

            if (typeof(T) == typeof(bool?) || typeof(T) == typeof(bool))
            {
                return Convert.ToBoolean(this[key]).Cast<T>();
            }

            return (T)(object)this[key];
        }

        public string Build()
        {
            if (Count == 0) return string.Empty;

            string queryString = "?";

            foreach (var key in Keys)
            {
                queryString += $"{key}={Uri.EscapeDataString(this[key])}";

                if (key != Keys.Last())
                {
                    queryString += "&";
                }
            }

            return queryString;
        }
    }
}
