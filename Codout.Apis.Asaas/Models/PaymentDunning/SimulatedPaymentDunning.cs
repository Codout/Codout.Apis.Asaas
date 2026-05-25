using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Codout.Apis.Asaas.Models.PaymentDunning;

public class SimulatedPaymentDunning
{
    [JsonPropertyName("payment")]
    public string PaymentId { get; set; }

    public decimal Value { get; set; }

    // Schema retorna ARRAY (simulacao por tipo). Antes do fix B-22m era
    // objeto unico, o que causava InvalidCastException na deserializacao.
    public List<PaymentDunningTypeSimulations> TypeSimulations { get; set; } = [];
}
