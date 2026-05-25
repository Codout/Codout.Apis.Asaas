using System;

namespace Codout.Apis.Asaas.Models.MyAccount;

public class CommercialInfoExpiration
{
    public bool? IsExpired { get; set; }
    public DateTime? ScheduledDate { get; set; }
}
