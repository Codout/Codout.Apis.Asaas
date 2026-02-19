using Codout.Apis.Asaas.Models.Common;

namespace Codout.Apis.Asaas.Models.MyAccount
{
    public class CreatePaymentCheckoutConfigRequest
    {
        public string LogoBackgroundColor { get; set; }

        public string InfoBackgroundColor { get; set; }

        public string FontColor { get; set; }

        public bool Enabled { get; set; }

        public AsaasFile LogoFile { get; set; }
    }
}
