using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codout.Apis.Asaas.Models.Common;

namespace Codout.Apis.Asaas.Models.Anticipation
{
    public class CreateAnticipationRequest
    {
        [JsonPropertyName("installment")]
        public string InstallmentId { get; set; }

        [JsonPropertyName("payment")]
        public string PaymentId { get; set; }

        public string AgreementSignature { get; set; }

        public List<AsaasFile> Documents { get; set; }
    }
}
