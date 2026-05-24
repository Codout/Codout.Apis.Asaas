using System.Collections.Generic;
using Codout.Apis.Asaas.Core.Interfaces;

namespace Codout.Apis.Asaas.Models.Chargeback;

public class CreateChargebackDisputeRequest
{
    public string Description { get; set; }
    public List<IAsaasFile> Documents { get; set; } = [];
}
