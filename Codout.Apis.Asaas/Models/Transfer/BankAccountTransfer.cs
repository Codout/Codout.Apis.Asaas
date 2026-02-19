using Codout.Apis.Asaas.Models.Transfer.Base;
using Codout.Apis.Asaas.Models.Transfer.Enums;

namespace Codout.Apis.Asaas.Models.Transfer
{
    public class BankAccountTransfer : BaseTransfer {
        public decimal NetValue { get; set; }

        public BankAccountTransferStatus Status { get; set; }

        public BankAccount BankAccount { get; set; }
    }
}
