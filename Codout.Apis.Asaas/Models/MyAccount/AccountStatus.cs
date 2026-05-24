using Codout.Apis.Asaas.Models.MyAccount.Enums;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class AccountStatus
{
    public string Id { get; set; }
    public AccountApprovalStatus? CommercialInfo { get; set; }
    public AccountApprovalStatus? Documentation { get; set; }
    public AccountApprovalStatus? General { get; set; }
    public AccountApprovalStatus? BankAccountInfo { get; set; }
}
