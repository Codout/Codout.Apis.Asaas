using System.Collections.Generic;

namespace Codout.Apis.Asaas.Models.Installment;

public class UpdateInstallmentSplitsRequest
{
    public List<InstallmentSplitRequest> Splits { get; set; } = [];
}
