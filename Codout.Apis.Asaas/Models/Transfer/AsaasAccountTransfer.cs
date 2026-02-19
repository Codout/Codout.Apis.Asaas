using Codout.Apis.Asaas.Models.Transfer.Base;
using Codout.Apis.Asaas.Models.Transfer.Enums;

namespace Codout.Apis.Asaas.Models.Transfer
{
    public class AsaasAccountTransfer : BaseTransfer {
        public string WalletId { get; set; }

        public AsaasAccountTransferStatus Status { get; set; }

        public AsaasAccount Account { get; set; }
    }
}
