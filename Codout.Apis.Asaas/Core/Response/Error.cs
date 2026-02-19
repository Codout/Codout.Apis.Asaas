using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Core.Response
{
    public class Error
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
