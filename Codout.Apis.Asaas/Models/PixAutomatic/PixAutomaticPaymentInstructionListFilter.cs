using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.PixAutomatic.Enums;

namespace Codout.Apis.Asaas.Models.PixAutomatic;

public class PixAutomaticPaymentInstructionListFilter : RequestParameters
{
    public string AuthorizationId
    {
        get => this["authorizationId"];
        set => Add("authorizationId", value);
    }

    public string CustomerId
    {
        get => this["customerId"];
        set => Add("customerId", value);
    }

    public string PaymentId
    {
        get => this["paymentId"];
        set => Add("paymentId", value);
    }

    public PixAutomaticPaymentInstructionStatus? Status
    {
        get => Get<PixAutomaticPaymentInstructionStatus?>("status");
        set => Add("status", value);
    }
}
