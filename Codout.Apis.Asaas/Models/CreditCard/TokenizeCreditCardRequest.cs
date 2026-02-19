using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common;

namespace Codout.Apis.Asaas.Models.CreditCard;

public class TokenizeCreditCardRequest
{
    [JsonPropertyName("customer")]
    public string Customer { get; set; }

    [JsonPropertyName("creditCard")]
    public CreditCardRequest CreditCard { get; set; }

    [JsonPropertyName("creditCardHolderInfo")]
    public CreditCardHolderInfoRequest CreditCardHolderInfo { get; set; }

    [JsonPropertyName("remoteIp")]
    public string RemoteIp { get; set; }
}