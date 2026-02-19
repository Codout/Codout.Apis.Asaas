using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Models.PaymentDunning
{
    public class SimulatedPaymentDunning {
        [JsonPropertyName("payment")]
        public string PaymentId { get; set; }

        public decimal Value { get; set; }

        public PaymentDunningTypeSimulations TypeSimulations { get; set; }
    }
}
