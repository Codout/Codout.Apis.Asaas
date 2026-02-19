using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Models.PaymentDunning
{
    public class SimulatePaymentDunningRequest
    {
        [JsonPropertyName("payment")]
        public string PaymentId { get; set; }
    }
}
