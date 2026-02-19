using System;
using Codout.Apis.Asaas.Core;
using Codout.Apis.Asaas.Models.Transfer.Enums;

namespace Codout.Apis.Asaas.Models.Transfer
{
    public class TransferListFilter : RequestParameters
    {
        public DateTime? DateCreated
        {
            get => Get<DateTime?>("dateCreated");
            set => Add("dateCreated", value);
        }

        public TransferType? TransferType
        {
            get => Get<TransferType?>("type");
            set => Add("type", value);
        }
    }
}
