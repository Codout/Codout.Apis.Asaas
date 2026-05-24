using System;

namespace Codout.Apis.Asaas.Models.AsaasAccount;

public class UpdateAccessTokenRequest
{
    public string Name { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool? Enabled { get; set; }
}
