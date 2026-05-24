using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Models.Finance;

public class Balance
{
    [JsonPropertyName("balance")]
    public decimal Value { get; set; }
}
