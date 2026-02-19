using Codout.Apis.Asaas.Core.Interfaces;

namespace Codout.Apis.Asaas.Models.PaymentLink
{
    public class AddPaymentLinkImageRequest
    {
        public bool Main { get; set; }
        public IAsaasFile Image { get; set; }
    }
}
