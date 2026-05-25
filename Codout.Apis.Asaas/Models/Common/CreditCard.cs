using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common.Enums;

namespace Codout.Apis.Asaas.Models.Common;

public class CreditCard
{
    [JsonPropertyName("creditCardNumber")]
    public string Number { get; set; }

    [JsonPropertyName("creditCardBrand")]
    public CreditCardBrand? Brand { get; set; }

    [JsonPropertyName("creditCardToken")]
    public string Token { get; set; }
}
