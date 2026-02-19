using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Models.Anticipation
{
    public class SimulateAnticipationRequest
    {
        [JsonPropertyName("installment")]
        public string InstallmentId { get; set; }

        [JsonPropertyName("payment")]
        public string PaymentId { get; set; }
    }
}
